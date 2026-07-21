using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WorstLogin
{
    public class LoginForm : MonoBehaviour
    {
        [SerializeField] private LetterCollector _letterCollector;
        [SerializeField] private LetterGenerator _letterGenerator;
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _passwordText;

        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _resetButton;
        
        private string _userName;
        private string _password;
        
        private void Awake()
        {
            _letterCollector.OnLetterCollected += HandleLetterCollected;
            _loginButton.onClick.AddListener(HandleLoginButtonClicked);
            _resetButton.onClick.AddListener(HandleResetButtonClicked);
            HandleResetButtonClicked();
        }

        private void HandleResetButtonClicked()
        {
            _userName = string.Empty;
            _password = string.Empty;
            _userNameText.text = string.Empty;
            _passwordText.text = string.Empty;
        }

        private void HandleLoginButtonClicked()
        {
            _letterGenerator.SetLetterCollection(LetterSet.Alpha);
        }

        private void HandleLetterCollected(char character)
        {
            _userName += character;
            _userNameText.text = _userName;
        }
    }
}
