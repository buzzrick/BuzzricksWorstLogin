using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WorstLogin
{
    public class LetterSquare : MonoBehaviour
    {
        public Image Image;
        public TMP_Text Text;
        public float fallSpeed = 10f;
        
        public Action<LetterSquare> OnDestroyRequest;
        private Camera _camera;
        
        public void SetCharacter(char character)
        {
            Text.text = character.ToString();
            _camera = Camera.main;
        }

        private void Update()
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            if (transform.position.y < -_camera.orthographicSize)
            {
                OnDestroyRequest?.Invoke(this);
                OnDestroyRequest = null;
            }
        }
    }
}