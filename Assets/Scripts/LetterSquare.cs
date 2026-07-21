using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WorstLogin
{
    [RequireComponent(typeof(Collider2D))]
    public class LetterSquare : MonoBehaviour
    {
        public Image Image;
        public TMP_Text Text;
        public float fallSpeed = 10f;
        private char _character;

        public event Action<LetterSquare> OnDestroyRequest;
        private Camera _camera;
        private Rigidbody2D _rigidBody;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _camera = Camera.main;
        }


        public char Character
        {
            get
            {
                return _character; 
            }
            set
            {
                _character = value;
                Text.text = _character.ToString();
            }
        }

        private void FixedUpdate()
        {
            var newPosition = _rigidBody.position + Vector2.down * (fallSpeed * Time.fixedDeltaTime);
            _rigidBody.MovePosition(newPosition);
            
            if (newPosition.y < -_camera.orthographicSize)
            {
                RequestDestroy();
            }
        }

        public void RequestDestroy()
        {
            OnDestroyRequest?.Invoke(this);
        }

        public override string ToString()
        {
            return $"LetterSquare: {Character}";
        }
    }
}