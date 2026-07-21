using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runaway.Extensions;
using UnityEngine;

namespace WorstLogin
{
    public class LetterGenerator : MonoBehaviour
    {
        [SerializeField] private LetterSquarePool _letterSquarePool;
        private List<LetterSquare> _letterSquares = new();
        private Vector3 _screenBottomLeft;
        private Vector3 _screenTopRight;
        private CancellationTokenSource _letterCreationLoopCancellationSource = new();
        private const float SpawnRate = 4f;
        private float _spawnDelay = 0.25f;

        private void Awake()
        {
            CalculateScreenBounds();
            SetLetterCollection(LetterSet.Alpha);
        }

        private void CalculateScreenBounds()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found.");
                return;
            }
            
            _screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            _screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
            
            //  shrink the edges of the screen to make it easier to see the letters, by about 5%    
            float shrinkFactor = 0.05f;
            float width = _screenTopRight.x - _screenBottomLeft.x;
            _screenBottomLeft.x += width * shrinkFactor;
            _screenTopRight.x -= width * shrinkFactor;
        }

        public void SetLetterCollection(LetterSet letterSet)
        {
            CleanupAllLetters();
            _letterCreationLoopCancellationSource.Cancel();
            _letterCreationLoopCancellationSource = new CancellationTokenSource();
            LetterCreationLoop(letterSet, false, _letterCreationLoopCancellationSource.Token).Forget();
        }
        
        private void CleanupAllLetters()
        {
            var allLetters = new List<LetterSquare>(_letterSquares);
            foreach (var letterSquare in allLetters)
            {
                letterSquare.OnDestroyRequest -= DestroyLetterSquare;
                _letterSquarePool.Release(letterSquare);
            }
            _letterSquares.Clear();
        }

        private async UniTask LetterCreationLoop(LetterSet letterSet, bool randomised, CancellationToken cancellationToken)
        {
            CleanupAllLetters();
            
            char[] letters = LetterSetCollections.Collections[letterSet];
            int letterCount = letters.Length;
            
            // Create an array of indices and shuffle it to randomize the spawn order.
            int[] spawnOrder = new int[letterCount];
            for (int i = 0; i < letterCount; i++)
            {
                spawnOrder[i] = i;
            }
            //spawnOrder = spawnOrder.Shuffle().ToArray();

            _spawnDelay = SpawnRate / letterCount;
            float letterSpacing = (_screenTopRight.x - _screenBottomLeft.x) / (letterCount - 1);
            
            Debug.Log($"Starting letter creation loop with {letterCount} letters. Spawn delay: {_spawnDelay}s");

            int spawnIndex = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                // Get the randomised letter to spawn next.
                int letterIndex = spawnOrder[spawnIndex];
                char currentLetter = letters[letterIndex];

                LetterSquare letterSquare = CreateLetter(currentLetter);

                // Position the letter based on its original, unshuffled index.
                letterSquare.transform.position = new Vector3(_screenBottomLeft.x + (letterIndex * letterSpacing), _screenTopRight.y, 0);

                // Wait for the spawn delay before spawning the next letter.
                await UniTask.Delay(TimeSpan.FromSeconds(_spawnDelay), cancellationToken: cancellationToken);

                spawnIndex++;
                if (spawnIndex >= letterCount)
                {
                    // Reshuffle for the next loop.
                    spawnIndex = 0;
                    if (randomised)
                    {
                        //spawnOrder = spawnOrder.Shuffle().ToArray();
                    }
                }
            }
        }

        private LetterSquare CreateLetter(char currentLetter)
        {
            var letterSquare = _letterSquarePool.Get();
            Debug.Log($"Spawning letter: {currentLetter}", letterSquare);
            letterSquare.Character = currentLetter;
            letterSquare.OnDestroyRequest += DestroyLetterSquare;
            _letterSquares.Add(letterSquare);
            return letterSquare;
        }

        private void DestroyLetterSquare(LetterSquare letterSquare)
        {
            letterSquare.OnDestroyRequest -= DestroyLetterSquare;
            _letterSquares.Remove(letterSquare);
            _letterSquarePool.Release(letterSquare);
        }
    }
}