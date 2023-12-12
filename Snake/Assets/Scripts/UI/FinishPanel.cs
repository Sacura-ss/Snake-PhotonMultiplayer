using System;
using Snake;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class FinishPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _finishText;
        [SerializeField] private Button _mainMenuButton;

        private void OnEnable()
        {
            _mainMenuButton.onClick.AddListener(LoadMenu);
            if (SavedData.GetMaxCount() > SavedData.GetPlayerCount(0))
                _finishText.text = "Lose with score " + SavedData.GetPlayerCount(0);
            else
                _finishText.text = "Win with score " + SavedData.GetPlayerCount(0);
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