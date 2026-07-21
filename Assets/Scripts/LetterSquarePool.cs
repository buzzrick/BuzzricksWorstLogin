using UnityEngine;
using UnityEngine.Pool;

namespace WorstLogin
{
    public class LetterSquarePool : MonoBehaviour
    {
        private ObjectPool<LetterSquare> _letterSquarePool;
        [SerializeField] private LetterSquare _letterSquarePrefab;

        private void Awake()
        {
            _letterSquarePool = new ObjectPool<LetterSquare>(
                CreateLetterSquare,
                OnGetLetterSquare,
                OnReleaseLetterSquare,
                OnDestroyLetterSquare,
                true,   // set true during development to add extra error checking, but false in production
                26);
        }

        private LetterSquare CreateLetterSquare()
        {
            return GameObject.Instantiate(_letterSquarePrefab);
        }

        private void OnGetLetterSquare(LetterSquare letterSquare)
        {
            letterSquare.gameObject.SetActive(true);
        }

        private void OnReleaseLetterSquare(LetterSquare letterSquare)
        {
            letterSquare.gameObject.SetActive(false);
        }

        private void OnDestroyLetterSquare(LetterSquare letterSquare)
        {
            GameObject.Destroy(letterSquare.gameObject);
        }

        public LetterSquare Get()
        {
            return _letterSquarePool.Get();
        }

        public void Release(LetterSquare letterSquare)
        {
            _letterSquarePool.Release(letterSquare);
        }
    }
}