using UnityEngine;

namespace InfiniteVox
{
    public class BlackScreen : MonoBehaviour
    {
        public static BlackScreen Base;

        [SerializeField] private GameObject _screen;

        private Animator _animator;

        private void Awake()
        {
            Base = this;
            DontDestroyOnLoad(gameObject);
            _animator = GetComponent<Animator>();
        }

        public void Show()
        {
            _screen.SetActive(true);
            _animator.SetTrigger("On");
        }

        public void Hide() => _animator.SetTrigger("Off");

        public void DisableScreen() => _screen.SetActive(false);
    }
}