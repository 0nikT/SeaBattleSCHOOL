using System.Collections.Generic;
using UnityEngine;


namespace InfiniteVox
{
    public class ShipGrid : MonoBehaviour
    {
        public Transform[] MissPoints;
        public bool PlayerMove;
        public bool Destroyed;
        public int TilesMax;

        [SerializeField] private Transform _model;
        [SerializeField] private float _boxZ;

        private GameController _gameController;
        private Vector3 _restartPosition;
        private Vector3 _restartRotation;
        private Collider[] _colliders;
        private bool _restartingRotation;
        private int _tiles;
        private int _lives;

        private void Start()
        {
            _gameController = GameController.Base;
            _lives = TilesMax;
        }

        private void Update()
        {
            if (_gameController.Started)
                return;

            if (_gameController.LastShip == this && _gameController.CurrentShip == null)
                CheckPosition(false);
        }

        public bool CheckPosition(bool shuffle)
        {
            bool rotationY = transform.eulerAngles.y == 0f;

            _colliders = Physics.OverlapBox(_model.position, new Vector3(rotationY ? 30f : _boxZ, 5f, rotationY ? _boxZ : 30f) / 2f, Quaternion.identity, LayerMask.GetMask("Ship", "Tile"), QueryTriggerInteraction.Collide);
            List<Tile> tiles = new List<Tile>();

            foreach (Collider collider in _colliders)
            {
                if (collider.TryGetComponent(out ShipGrid shipGrid))
                {
                    if (shipGrid == this)
                    {
                        if (_tiles == TilesMax)
                            continue;
                    }

                    if (!shuffle)
                        Restart();

                    return false;
                }
                else if (shuffle)
                {
                    tiles.Add(collider.GetComponent<Tile>());
                }
            }

            if (shuffle)
                _gameController.RemoveTiles(tiles);

            if (_restartingRotation)
            {
                _restartingRotation = false;
                SetRestartRotation();
            }

            return true;
        }

        private void RestartingRotation()
        {
            if (CheckPosition(false))
                _restartingRotation = true;
        }

        public void TakeDamage()
        {
            _lives--;

            if (_lives <= 0)
            {
                Destroyed = true;

                foreach (Collider collider in _colliders)
                {
                    if (collider.TryGetComponent(out Tile tile))
                    {
                        if (tile.CurrentShip != null)
                            tile.DestroyHit();
                        else
                            tile.HitSprite(false);
                    }
                }

                EnableShip(true);
                _gameController.DestroyShip(TilesMax - 1);
            }
        }

        public void FlickShip()
        {
            if (_model.gameObject.activeInHierarchy)
                return;

            InvokeRepeating(nameof(Flick), 0f, 1f);
        }


    

        private void Flick() => EnableShip(!_model.gameObject.activeSelf);

        public void EnableShip(bool value) => _model.gameObject.SetActive(value);

        public void SetTile(bool add) => _tiles += add ? 1 : -1;

        public void RestartRotation() => Invoke(nameof(RestartingRotation), 0.1f);

        public void SetRestartPosition() => _restartPosition = transform.position;

        public void SetRestartRotation() => _restartRotation = transform.eulerAngles;

        private void Restart() => transform.SetPositionAndRotation(_restartPosition, Quaternion.Euler(_restartRotation));

        public bool IsEnabled() => _model.gameObject.activeSelf;
    }
}