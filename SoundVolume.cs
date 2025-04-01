using UnityEngine;

namespace InfiniteVox
{
    public class SoundVolume : MonoBehaviour
    {
        private AudioSource _sound;

        private void Awake() => _sound = GetComponent<AudioSource>();

        private void Start() => SetSoundVolume(AudioController.Base.SoundsVolume);

        public void SetSoundVolume(float value) => _sound.volume = value;
    }
}