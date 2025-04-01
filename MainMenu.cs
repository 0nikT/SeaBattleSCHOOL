using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfiniteVox
{
    public class MainMenu : MonoBehaviour
    {
        private string _language;
        private int[] _gameSettings = { 1, 1 };

        private void Awake()
        {
            _language = GameSettings.Settings.GetData(true);

            BlackScreen.Base.Hide();
        }

        public void StartGame()
        {
            for (int i = 0; i < _gameSettings.Length; i++)
                PlayerPrefs.SetInt("GameSettings" + i, _gameSettings[i]);

            BlackScreen.Base.Show();

            Invoke(nameof(LoadGame), 1.5f);
        }

        public void ExitGame()
        {
            BlackScreen.Base.Show();

            Invoke(nameof(Exit), 1.5f);
        }

        private void Exit() => Application.Quit();

        private void LoadGame() => SceneManager.LoadSceneAsync("Game");

        public void SetType(int value) => _gameSettings[0] = value;

        public void SetDifficulty(int value) => _gameSettings[1] = value;
    }
}