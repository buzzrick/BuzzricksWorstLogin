#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WorstLogin
{
    [RequireComponent(typeof(Collider2D))]
    public class LetterCollector : MonoBehaviour
    {
        public float moveSpeed = 10f;

        private Camera _mainCamera;
        private float _minX, _maxX, _minY;
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private Rigidbody2D _rigidBody;
        
        public event Action<char> OnLetterCollected;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            var objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
            var objectHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
            
            _minX = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + objectWidth;
            _maxX = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - objectWidth;
            _minY = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0.05f, 0)).y + objectHeight;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Letter"))
            {
                LetterSquare? letter = other.gameObject.GetComponent<LetterSquare>();
                if (letter != null)
                {
                    Debug.Log($"Letter collected: {letter.Character}");
                    OnLetterCollected?.Invoke(letter.Character);
                    letter.RequestDestroy();
                }
            }
        }
        
        

        private void FixedUpdate()
        {
            var moveInput = _moveAction.ReadValue<Vector2>().x;
            var currentPosition = transform.position;
            currentPosition.x += moveInput * moveSpeed * Time.fixedDeltaTime;
            currentPosition.x = Mathf.Clamp(currentPosition.x, _minX, _maxX);
            currentPosition.y = _minY;
            
            _rigidBody.MovePosition(currentPosition);
        }
    }
}
