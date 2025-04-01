using UnityEngine;
using UnityEngine.UI;

namespace InfiniteVox
{
    public class TextLanguage : MonoBehaviour
    {
        [SerializeField, TextArea(0, 10)] private string _ru, _en, _tr;

        private string _language;
        private Text _text;

        private void Awake()
        {
            _language = GameSettings.Settings.GetData(true);
            _text = GetComponent<Text>();
        }

        private void Start()
        {
            string text;

            if (_language == "ru")
                text = _ru;
            else
                text = _en;

            _text.text = text;
        }
    }
}