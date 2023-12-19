using System;
using DefaultNamespace.Saving;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class FinishPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _winnerNameText;
        [SerializeField] private TMP_Text _winnerScoreText;
        [SerializeField] private Button _mainMenuButton;

        private void OnEnable()
        {
            _mainMenuButton.onClick.AddListener(LoadMenu);
        }

        private void Start()
        {
            _winnerNameText.text = "Winner: " + SavedData.WinnerName;
            _winnerScoreText.text = "Score: " + SavedData.WinnerScore;
        }

        private void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void OnDisable()
        {
            _mainMenuButton.onClick.RemoveListener(LoadMenu);
        }
    }
}