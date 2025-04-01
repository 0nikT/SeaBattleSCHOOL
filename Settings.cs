using UnityEngine;
using UnityEngine.UI;

namespace InfiniteVox
{
    public class Settings : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private Slider _mSlider;
        [SerializeField] private Text _mValueText;
        [SerializeField] private Image _mButton;
        [SerializeField] private Sprite _mOn, _mOff;

        private float _musicValue = 1f;

        [Space(10f)]

        [Header("Sound")]
        [SerializeField] private Slider _sSlider;
        [SerializeField] private Text _sValueText;
        [SerializeField] private Image _sButton;
        [SerializeField] private Sprite _sOn, _sOff;

        private float _soundsValue = 1f;

        [Header("Quality")]
        [SerializeField] private Slider _qSlider;
        [SerializeField] private Text _qValueText;
        [SerializeField] private RectTransform _qButton;

        private int _qualityValue;

        private string _language;

        private void Awake()
        {
            _language = GameSettings.Settings.GetData(true);

            _musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _soundsValue = PlayerPrefs.GetFloat("SoundsVolume", 1f);
            _qualityValue = PlayerPrefs.GetInt("QualityValue", GameSettings.Settings.GetData(false) == "desktop" ? 5 : 2);

            UpdateMusic();
            UpdateSounds();
            UpdateQuality();
        }

        public void ChangeSettings(int settings)
        {
            if (settings == 1)
            {
                _musicValue = _musicValue > 0f ? 0f : 1f;

                UpdateMusic();
            }
            else if (settings == 2)
            {
                _soundsValue = _soundsValue > 0f ? 0f : 1f;

                UpdateSounds();
            }
            else
            {
                _qualityValue = (_qualityValue + 1) % 6;

                UpdateQuality();
            }

            Save(settings);
        }

        public void SetMusic(float value)
        {
            _musicValue = value;

            UpdateMusic();
            Save(1);
        }

        public void SetSounds(float value)
        {
            _soundsValue = value;

            UpdateSounds();
            Save(2);
        }

        public void SetQuality(float value)
        {
            _qualityValue = (int)value;

            UpdateQuality();
            Save(3);
        }

        private void UpdateMusic()
        {
            _mSlider.value = _musicValue;
            _mValueText.text = $"{Mathf.FloorToInt(_musicValue / 1f * 100f)}%";
            _mButton.sprite = _musicValue > 0f ? _mOn : _mOff;

            AudioController.Base.SetMusicVolume(_musicValue);
        }

        private void UpdateSounds()
        {
            _sSlider.value = _soundsValue;
            _sValueText.text = $"{Mathf.FloorToInt(_soundsValue / 1f * 100f)}%";
            _sButton.sprite = _soundsValue > 0f ? _sOn : _sOff;

            AudioController.Base.SetSoundsVolume(_soundsValue);
        }

        private void UpdateQuality()
        {
            _qSlider.value = _qualityValue;
            _qButton.anchoredPosition = new Vector3(50f, 0f, 0f);

            string text = string.Empty;

            switch (_qualityValue)
            {
                case 0:
                    if (_language == "ru")
                        text = "Очень Низкое";
                    else if (_language == "tr")
                        text = "Çok Düşük";
                    else
                        text = "Very Low";

                    _qButton.anchoredPosition = new Vector3(30f, 0f, 0f);
                    break;
                case 1:
                    if (_language == "ru")
                        text = "Низкое";
                    else if (_language == "tr")
                        text = "Düşük";
                    else
                        text = "Low";
                    break;
                case 2:
                    if (_language == "ru")
                        text = "Среднее";
                    else if (_language == "tr")
                        text = "Orta";
                    else
                        text = "Medium";
                    break;
                case 3:
                    if (_language == "ru")
                        text = "Высокое";
                    else if (_language == "tr")
                        text = "Yüksek";
                    else
                        text = "High";
                    break;
                case 4:
                    if (_language == "ru")
                        text = "Очень Высокое";
                    else if (_language == "tr")
                        text = "Çok Yüksek";
                    else
                        text = "Very High";
                    break;
                case 5:
                    if (_language == "ru")
                        text = "Ультра";
                    else
                        text = "Ultra";
                    break;
            }

            _qValueText.text = text;

            QualitySettings.SetQualityLevel(_qualityValue);
        }

        private void Save(int settings)
        {
            if (settings == 1)
                PlayerPrefs.SetFloat("MusicVolume", _musicValue);
            else if (settings == 2)
                PlayerPrefs.SetFloat("SoundsVolume", _soundsValue);
            else
                PlayerPrefs.SetInt("QualityValue", _qualityValue);
        }
    }
}