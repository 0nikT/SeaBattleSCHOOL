using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfiniteVox
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings Settings;

        [SerializeField] private Language _language;
        [SerializeField] private Device _device;

        private string _languageStr;
        private string _deviceStr;

        private void Awake()
        {
            Settings = this;
            DontDestroyOnLoad(gameObject);

            string language = string.Empty;
            string device = string.Empty;

            switch (_language)
            {
                case Language.Russian:
                    language = "ru";
                    break;
                case Language.English:
                    language = "en";
                    break;
            }

            switch (_device)
            {
                case Device.Desktop:
                    device = "desktop";
                    break;
                case Device.Mobile:
                    device = "mobile";
                    break;
                case Device.Tablet:
                    device = "tablet";
                    break;
            }

            _languageStr = language;
            _deviceStr = device;

            Invoke(nameof(LoadMainMenu), 1.5f);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }

        public string GetData(bool data) => data ? _languageStr : _deviceStr;
    }

    [System.Serializable]
    public enum Language
    {
        Russian,
        English,
    }

    [System.Serializable]
    public enum Device
    {
        Desktop,
        Mobile,
        Tablet
    }
}