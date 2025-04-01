using UnityEngine;

namespace InfiniteVox
{
    public class MissleObj : MonoBehaviour
    {
        public GameObject Target;

        private bool _used;
        private GameController _gameController;
        private TrailRenderer _trail;
        private Rigidbody _rigidbody;
        private Vector3 _velocity;

        private void Awake()
        {
            _gameController = GameController.Base;
            _rigidbody = GetComponent<Rigidbody>();
            _trail = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            if (_gameController.Paused)
            {
                if (!_used)
                {
                    _used = true;
                    _velocity = _rigidbody.velocity;
                    _rigidbody.isKinematic = true;
                    _trail.time = Mathf.Infinity;
                }
            }
            else
            {
                if (_used)
                {
                    _used = false;
                    _rigidbody.isKinematic = false;
                    _rigidbody.velocity = _velocity;
                    _trail.time = 0.5f;
                }
            }
        }

        private void OnDestroy() => _trail.enabled = false;
    }
}