using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace InfiniteVox
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Base;
        public float SoundsVolume = 1f;

        [SerializeField] private AudioClip MainTheme;
        private AudioClip Music;
        private AudioClip[] BattleThemes;
        private AudioSource _music;

        private void Awake()
        {
            if (Base == null)
            {
                Base = this;
                DontDestroyOnLoad(gameObject);
                _music = GetComponent<AudioSource>();
                SceneManager.sceneLoaded += OnSceneLoaded;
                LoadMusic();
                LoadBattleThemes();
                PlayInitialMusic();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadMusic()
        {
            Music = Resources.Load<AudioClip>("Audio/Music");
        }

        private void LoadBattleThemes()
        {
            BattleThemes = Resources.LoadAll<AudioClip>("Audio/BattleThemes");
        }

        private void PlayInitialMusic()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                PlayMainTheme();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MainMenu")
            {
                PlayMainTheme();
            }
            else if (scene.name == "Game")
            {
                PlayRandomBattleTheme();
            }
        }

        private void PlayMainTheme()
        {
            StopAllCoroutines();
            _music.Stop();
            _music.clip = MainTheme;
            _music.loop = true;
            _music.Play();
        }

        private void PlayRandomBattleTheme()
        {
            if (BattleThemes == null || BattleThemes.Length == 0)
            {
                return;
            }

            AudioClip selectedClip = BattleThemes[Random.Range(0, BattleThemes.Length)];

            StopAllCoroutines();
            _music.Stop();
            _music.clip = selectedClip;
            _music.loop = false;
            _music.Play();

            StartCoroutine(PlayMusicAfterBattleTheme(selectedClip.length));
        }

        private IEnumerator PlayMusicAfterBattleTheme(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (SceneManager.GetActiveScene().name == "Game" && Music != null)
            {
                _music.Stop();
                _music.clip = Music;
                _music.loop = true;
                _music.Play();
            }
        }

        public void SetMusicVolume(float value) => _music.volume = value;

        public void SetSoundsVolume(float value)
        {
            SoundsVolume = value;
            foreach (SoundVolume soundVolume in FindObjectsOfType<SoundVolume>())
            {
                soundVolume.SetSoundVolume(value);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
