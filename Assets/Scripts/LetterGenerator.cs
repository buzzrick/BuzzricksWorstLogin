using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runaway.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WorstLogin
{
    public class LetterGenerator : MonoBehaviour
    {
        [SerializeField] private LetterSquarePool _letterSquarePool;
        private List<LetterSquare> _letterSquares = new();
        private Vector3 _screenBottomLeft;
        private Vector3 _screenTopRight;
        private CancellationTokenSource _letterCreationLoopCancellationSource = new();
        private const float SpawnRate = 5f;
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
        }

        public void SetLetterCollection(LetterSet letterSet)
        {
            _letterCreationLoopCancellationSource.Cancel();
            _letterCreationLoopCancellationSource = new CancellationTokenSource();
            LetterCreationLoop(letterSet, false, _letterCreationLoopCancellationSource.Token).Forget();
        }
        
        private void CleanupAllLetters()
        {
            foreach (var letterSquare in _letterSquares)
            {
                _letterSquarePool.Release(letterSquare);
            }
            _letterSquares.Clear();
        }

        private async UniTask LetterCreationLoop(LetterSet letterSet, bool randomised, CancellationToken cancellationToken)
        {
            //randomised = true;
            CleanupAllLetters();
            char[] letters =
                randomised
                    ? LetterSetCollections.Collections[letterSet].Shuffle().ToArray()
                    : LetterSetCollections.Collections[letterSet];
			int[] indexShuffler = new int[letters.Length];
            for (int i = 0; i < indexShuffler.Length; i++)
            {
                indexShuffler[i] = i;
            }
            indexShuffler = indexShuffler.Shuffle().ToArray();

            int letterCount = letters.Length;
            int currentLetterIndex = 0;
            _spawnDelay = SpawnRate / letterCount;
            
            Debug.Log($"Starting letter creation loop with {letterCount} letters, randomised: {randomised}, _spawnDelay: {_spawnDelay}");
            
            float nextSpawnTime = Time.time;

            while (!cancellationToken.IsCancellationRequested)
            {
                while (Time.time >= nextSpawnTime && !cancellationToken.IsCancellationRequested)
                {
                    var letterSquare = _letterSquarePool.Get();
                    int letterIndex = indexShuffler[currentLetterIndex];
                    char currentLetter = letters[letterIndex];
                    letterSquare.SetCharacter(currentLetter);
                    letterSquare.OnDestroyRequest += DestroyLetterSquare;
                    _letterSquares.Add(letterSquare);
                    if (randomised)
                    {
                        letterSquare.transform.position =
                            new Vector3(Random.Range(_screenBottomLeft.x, _screenTopRight.x), _screenTopRight.y, 0);
                    }
                    else
                    {
                        float letterSpacing = (_screenTopRight.x - _screenBottomLeft.x) / (letterCount - 1);
                        letterSquare.transform.position = new Vector3(_screenBottomLeft.x + (letterIndex * letterSpacing), _screenTopRight.y, 0);
                    }
    
                    currentLetterIndex = (currentLetterIndex + 1) % letterCount;
                    nextSpawnTime += _spawnDelay;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        private void DestroyLetterSquare(LetterSquare letterSquare)
        {
            _letterSquares.Remove(letterSquare);
            _letterSquarePool.Release(letterSquare);
        }
    }
}