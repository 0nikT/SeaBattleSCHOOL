using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InfiniteVox
{
    public class Tile : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public ShipGrid CurrentShip;
        public bool PlayerMove;
        public bool Hit;

        [Space(10f)]

        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private GameObject _waterEffect;
        [SerializeField] private GameObject _destroyEffect;
        [SerializeField] private SpriteRenderer _hit;
        [SerializeField] private Sprite[] _hitSprites;
        [SerializeField] private List<Transform> _points = new List<Transform>();

        private bool _drag;
        private bool _used;
        private GameController _gameController;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _gameController = GameController.Base;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_gameController.Started)
                return;

            Collider[] ships = Physics.OverlapSphere(transform.position, 0.1f, LayerMask.GetMask("Ship"), QueryTriggerInteraction.Ignore);

            if (ships.Length > 0)
            {
                if (!_used)
                {
                    _used = true;

                    CurrentShip = ships[0].GetComponent<ShipGrid>();
                    CurrentShip.SetTile(true);
                }
            }
            else
            {
                if (_used)
                {
                    _used = false;

                    CurrentShip.SetTile(false);
                    CurrentShip = null;
                }
            }
        }

        public void SetColor(Color color)
        {
            if (_gameController.Win && CurrentShip != null)
            {
                if (!CurrentShip.IsEnabled())
                {
                    _spriteRenderer.color = Color.cyan;
                    return;
                }
            }

            _spriteRenderer.color = color;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CurrentShip == null || _gameController.Paused || _gameController.Started)
                return;

            _drag = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_gameController.Started)
                return;

            _drag = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_gameController.Paused || Hit)
                return;

            if (_gameController.Started)
            {
                if (_gameController.PlayerMove != PlayerMove && !_gameController.Shooting)
                {
                    _gameController.Shot(transform);
                    SetColor(Color.white);
                }
            }
            else if (!_drag && CurrentShip != null && _gameController.RotateTime <= 0f)
            {
                CurrentShip.transform.rotation = Quaternion.Euler(0f, CurrentShip.transform.eulerAngles.y == 90f ? 0f : 90f, 0f);
                CurrentShip.RestartRotation();
                _gameController.SetRotateTime();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_gameController.Paused || Hit)
                return;

            if (_gameController.Started)
            {
                if (_gameController.PlayerMove != PlayerMove && !_gameController.Shooting)
                {
                    SetColor(Color.green);
                    _gameController.SetGreenTile(this);
                }

                return;
            }

            ShipGrid ship = _gameController.CurrentShip;

            if (ship != null)
                ship.transform.position = new Vector3(transform.position.x, ship.transform.position.y, transform.position.z);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_gameController.Paused || CurrentShip == null || _gameController.Started)
                return;

            _gameController.SetCurrentShip(CurrentShip);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_gameController.Started || _gameController.Win)
                return;

            SetColor(Color.white);
            _gameController.RemoveGreenTile();
        }

        public void HitSprite(bool hit)
        {
            if (Hit)
                return;

            Hit = true;

            _hit.sprite = _hitSprites[hit ? 1 : 0];
            _hit.gameObject.SetActive(true);
        }

        public void DestroyHit() => _destroyEffect.SetActive(true);

        public List<Tile> GetShootTiles(bool hit)
        {
            List<Tile> tiles = new List<Tile>();
            List<Transform> points = new List<Transform>();
            List<Transform> shipTiles = new List<Transform>();

            if (hit)
            {
                Tile[] addShipTiles = FindObjectsOfType<Tile>();

                foreach (Tile addShipTile in addShipTiles)
                {
                    if (addShipTile.Hit || addShipTile.PlayerMove || addShipTile.gameObject == gameObject || addShipTile.CurrentShip == null)
                        continue;

                    if (addShipTile.CurrentShip == CurrentShip)
                        shipTiles.Add(addShipTile.transform);
                }
            }

            for (int i = 0; i < (hit ? 2 : 1); i++)
                points.AddRange(hit ? i == 0 ? shipTiles : CurrentShip.MissPoints : _points);

            foreach (Transform point in points)
            {
                Collider[] colliders = Physics.OverlapSphere(point.position, 1f, LayerMask.GetMask("Tile"), QueryTriggerInteraction.Collide);

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out Tile tile))
                    {
                        if (tile.Hit)
                            continue;

                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MissleObj missleObj))
            {
                if (missleObj.Target == gameObject)
                {
                    bool hit = CurrentShip != null;

                    HitSprite(hit);
                    _gameController.Hit(hit);

                    if (CurrentShip != null)
                        CurrentShip.TakeDamage();

                    if (hit)
                        _hitEffect.SetActive(true);
                    else
                        _waterEffect.SetActive(true);

                    Destroy(other.gameObject);
                }
            }
        }
    }
}