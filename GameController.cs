using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InfiniteVox
{
    public class GameController : MonoBehaviour
    {
        public static GameController Base;
        public bool Paused;
        public bool Started;
        public bool PlayerMove;
        public bool Shooting;
        public bool Win;
        public float RotateTime;
        public ShipGrid CurrentShip;
        public ShipGrid LastShip;

        [Space(10f)]

        [SerializeField] private GameObject[] _ships;
        [SerializeField] private GameObject[] _ships2;
        [SerializeField] private Transform _shipsParent1, _shipsParent2;
        [SerializeField] private GameObject _grid1, _grid2;
        [SerializeField] private GameObject _warning, _warningUI, _aiUI;
        [SerializeField] private RectTransform[] _placeShipsUis;
        [SerializeField] private Text _playerText, _playerText1, _playerText2;
        [SerializeField] private Transform _moveArrow;
        [SerializeField] private GameObject _placeShipsUI, _gameUI;
        [SerializeField] private GameObject _shufflePanel;
        [SerializeField] private GameObject _mobileUI;
        [SerializeField] private GameObject _pcUI;
        [SerializeField] private GameObject _pause;
        [SerializeField] private GameObject _tutorialText, _pauseText;
        [SerializeField] private AI _ai;
        [SerializeField] private Animation _winAnimation;
        [SerializeField] private Missle _shootMissle;
        [SerializeField] private AudioSource _shootSound, _waterSound, _hitSound, _winSound, _mainTheme;

        [Space(10f)]

        [SerializeField] private List<RemainingShip> _remainingShips1 = new List<RemainingShip>();
        [SerializeField] private List<RemainingShip> _remainingShips2 = new List<RemainingShip>();

        private string _device;
        private string _language;
        private string _loadScene;
        private bool _continue;
        private int _index;
        private int _gridShips1 = 5;
        private int _gridShips2 = 5;
        private int[] _gameSettings = { 1, 1 };
        private Tile _greenTile;
        private List<Tile> _tiles = new List<Tile>();
        private List<ShipGrid> _allShips = new List<ShipGrid>();
        private AudioClip[] _destroySounds;
        private AudioSource _audioSource;

        private void Awake()
{
    Base = this;


    _destroySounds = Resources.LoadAll<AudioClip>("DestroySounds");
    _audioSource = gameObject.AddComponent<AudioSource>();

    BlackScreen blackScreen = BlackScreen.Base;
    _device = GameSettings.Settings.GetData(false);
    _language = GameSettings.Settings.GetData(true);
    _loadScene = string.Empty;

    bool desktop = _device == "desktop";
    _pcUI.SetActive(desktop);
    _pauseText.SetActive(desktop);
    _mobileUI.SetActive(!desktop);

    for (int i = 0; i < _gameSettings.Length; i++)
        _gameSettings[i] = PlayerPrefs.GetInt("GameSettings" + i, 1);

    for (int i = 0; i < 2; i++)
    {
        int spawnIndex = 4;
        int spawn = 1;

        for (int j = 0; j < 5; j++)
        {
            GameObject ship = spawnIndex == 3 ? _ships2[Random.Range(0, _ships2.Length)] : _ships[4 - spawnIndex];
            ShipGrid shipGrid = Instantiate(ship, new Vector3(-400f, ship.transform.position.y, 0f), Quaternion.identity, i == 0 ? _shipsParent1 : _shipsParent2).GetComponent<ShipGrid>();
            shipGrid.PlayerMove = i == 1;
            spawn--;

            if (spawn <= 0)
            {
                spawnIndex--;
                spawn = spawnIndex;
            }
        }
    }

    _warning.transform.SetParent(blackScreen.transform);

    UpdatePlayerText(1);

    blackScreen.Hide();
}
        private void Start()
        {
            Shuffle();
        }

        private void Update()
        {
            if (_device == "desktop" && Input.GetKeyDown(KeyCode.Escape) && _loadScene == string.Empty)
                SetPause(!Paused);

            if (Started)
            {
                if (_greenTile != null && _device != "desktop")
                {
                    if (!Input.GetMouseButton(0))
                    {
                        _greenTile.SetColor(Color.white);
                        RemoveGreenTile();
                    }
                }

                if (Input.GetKeyDown(KeyCode.X) && _pcUI.activeInHierarchy && !Paused)
                {
                    _pauseText.SetActive(false);
                    _tutorialText.SetActive(false);
                    _pcUI.SetActive(false);
                }

                return;
            }

            if (!Input.GetMouseButton(0) && CurrentShip != null)
            {
                LastShip = CurrentShip;
                CurrentShip = null;
            }

            if (RotateTime > 0f)
                RotateTime -= Time.deltaTime;
        }

        public void DestroyShip(int ship)
        {
            RemainingShip remainingShip;
            int playerWin = 0;

            if (PlayerMove)
            {
                remainingShip = _remainingShips1[ship];
                _gridShips1--;

                if (_gridShips1 <= 0)
                    playerWin = 2;
            }
            else
            {
                remainingShip = _remainingShips2[ship];
                _gridShips2--;

                if (_gridShips2 <= 0)
                    playerWin = 1;
            }

            remainingShip.RemainingShips--;

            if (remainingShip.RemainingShips > 0)
            {
                remainingShip.RemainingText.text = $"x{remainingShip.RemainingShips}";
            }
            else
            {
                remainingShip.Cross.SetActive(true);
                remainingShip.RemainingText.gameObject.SetActive(false);
            }

            if (playerWin > 0)
            {
                Shooting = true;
                Win = true;

                if (playerWin == 1)
                {
                    _playerText1.color = Color.green;
                    _playerText2.color = Color.red;
                }
                else
                {
                    _playerText1.color = Color.red;
                    _playerText2.color = Color.green;
                }

                Tile[] tiles = FindObjectsOfType<Tile>();

                foreach (Tile tile in tiles)
                {
                    if (tile.PlayerMove == PlayerMove)
                        tile.SetColor(Color.green);
                    else
                        tile.SetColor(Color.red);
                }

                ShipGrid[] shipGrids = FindObjectsOfType<ShipGrid>();

                foreach (ShipGrid shipGrid in shipGrids)
                    shipGrid.FlickShip();

                _winAnimation.gameObject.SetActive(true);
                _winAnimation.Play("Player" + playerWin);

                _moveArrow.gameObject.SetActive(false);
                _winSound.Play();
            }
            else
            {
               _hitSound.Stop();
               if (_destroySounds.Length > 0)
{
    AudioClip randomDestroySound = _destroySounds[Random.Range(0, _destroySounds.Length)];
    _audioSource.PlayOneShot(randomDestroySound, 2.0f);
}

            }
        }

        private void ChangePlayerMove()
        {
            PlayerMove = !PlayerMove;

            Color color1 = new Color(0f, 0.8352941f, 1f, 1f);
            Color color2 = Color.green;

            if (PlayerMove)
            {
                _playerText1.color = color1;
                _playerText2.color = color2;

                AIShot();
            }
            else
            {
                _playerText1.color = color2;
                _playerText2.color = color1;
            }

            _moveArrow.localScale = new Vector3(_moveArrow.localScale.x * -1f, 1f, 1f);
        }

        public void Shot(Transform target)
        {
            Shooting = true;
            

            Tile[] addTiles = FindObjectsOfType<Tile>();
            List<Tile> tiles = new List<Tile>();

            foreach (Tile addTile in addTiles)
            {
                if (_gameSettings[0] == 1 && addTile.CurrentShip == null && !addTile.PlayerMove )
                    continue;
                
                if (addTile.PlayerMove == PlayerMove && !addTile.Hit)
                    tiles.Add(addTile);
            }

            _shootMissle.transform.position = tiles[Random.Range(0, tiles.Count)].transform.position;
            _shootMissle.Shot(target);

            _shootSound.Play();
        }

        public void Hit(bool value)
        {
            Shooting = false;

            if (value)
            {
                _hitSound.Play();
                AIShot();
            }
            else
            {
                _waterSound.Play();
                ChangePlayerMove();
            }

            if (_tutorialText.activeInHierarchy)
                _tutorialText.SetActive(false);
        }

        private void AIShot()
        {
            if (_gameSettings[0] == 2 || !PlayerMove)
                return;

            Shooting = true;
            _ai.Move();
        }

        public void SetGreenTile(Tile tile) => _greenTile = tile;

        public void RemoveGreenTile() => _greenTile = null;

        public void SetRotateTime() => RotateTime = 0.5f;

        public void SetPause(bool value)
        {
            if (_shufflePanel.activeInHierarchy || _continue)
                return;

            Paused = value;

            if (_device != "desktop")
                _mobileUI.SetActive(!value);

            _pause.SetActive(value);
        }

        public void LoadScene(bool scene)
        {
            BlackScreen.Base.Show();
            _loadScene = scene ? "Game" : "MainMenu";

            Invoke(nameof(Load), 1.5f);
        }

        public void Continue()
        {
            _continue = true;
            Paused = true;

            BlackScreen.Base.Show();

            Invoke(nameof(LoadContinue), 1.5f);
        }

        private void LoadContinue()
        {
            if (_grid2.activeInHierarchy)
            {
                Started = true;

                _continue = false;
                Paused = false;

                _placeShipsUI.SetActive(false);
                _gameUI.SetActive(true);
                _grid1.SetActive(true);
                _grid2.SetActive(true);

                ShipGrid[] ships = FindObjectsOfType<ShipGrid>();
                bool type = _gameSettings[0] == 1;

                foreach (ShipGrid ship in ships)
                    ship.EnableShip(type && !ship.PlayerMove);

                Transform cameraTransform = Camera.main.transform;
                cameraTransform.position = new Vector3(0f, 110f, -80f);

                if (type)
                    _ai.gameObject.SetActive(true);

                BlackScreen.Base.Hide();
            }
            else
            {
                _grid1.SetActive(false);
                _grid2.SetActive(true);

                if (_gameSettings[0] == 1)
                {
                    _continue = false;
                    _aiUI.SetActive(true);

                    Shuffle();
                }
                else
                {
                    _warningUI.SetActive(true);

                    foreach (RectTransform placeShipUI in _placeShipsUis)
                        placeShipUI.anchoredPosition = new Vector2(placeShipUI.anchoredPosition.x * -1f, placeShipUI.anchoredPosition.y);

                    Transform cameraTransform = Camera.main.transform;
                    cameraTransform.position = new Vector3(cameraTransform.position.x * -1f, cameraTransform.position.y, cameraTransform.position.z);

                    UpdatePlayerText(2);
                }
            }
        }

        public void ContinuePlayer()
        {
            _warningUI.SetActive(false);
            _continue = false;

            BlackScreen.Base.Hide();
            Shuffle();
        }

        private void UpdatePlayerText(int player)
        {
            string text;

            if (_language == "ru")
                text = "Игрок";
            else if (_language == "tr")
                text = "Oyuncu";
            else
                text = "Player";

            _playerText.text = $"{text} {player}";
        }

        public void SetCurrentShip(ShipGrid ship)
        {
            if (CurrentShip != null || Started)
                return;

            CurrentShip = ship;
            ship.SetRestartPosition();
        }

        public void Shuffle() => StartCoroutine(Shuffling());

        public void RemoveTiles(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
                _tiles.Remove(tile);
        }

        private IEnumerator Shuffling()
        {
            _shufflePanel.SetActive(true);
            Paused = true;

            CurrentShip = null;
            LastShip = null;

            ShipGrid[] ships = FindObjectsOfType<ShipGrid>();
            _tiles.Clear();
            _tiles.AddRange(FindObjectsOfType<Tile>());
            _allShips.AddRange(ships);

            foreach (ShipGrid shipGrid in ships)
                shipGrid.transform.position = new Vector3(-400f, shipGrid.transform.position.y, 0f);

            for (_index = 0; _index < ships.Length; _index++)
            {
                Vector3 position = _tiles[Random.Range(0, _tiles.Count)].transform.position;

                ShipGrid ship = _allShips[0];
                ship.transform.SetPositionAndRotation(new Vector3(position.x, ship.transform.position.y, position.z), Quaternion.Euler(new Vector3(0f, Random.Range(0, 2) == 0 ? 0f : 90f, 0f)));

                StartCoroutine(CheckPosition(ship));

                yield return new WaitForSeconds(0.2f);
            }

            foreach (ShipGrid shipGrid in ships)
                shipGrid.SetRestartRotation();

            _shufflePanel.SetActive(false);
            Paused = false;

            if (_gameSettings[0] == 1 && _aiUI.activeInHierarchy)
            {
                _aiUI.SetActive(false);

                LoadContinue();
            }
        }

        private IEnumerator CheckPosition(ShipGrid ship)
        {
            yield return new WaitForSeconds(0.1f);

            if (ship.CheckPosition(true))
                _allShips.Remove(ship);
            else
                _index--;
        }

        private void Load() => SceneManager.LoadSceneAsync(_loadScene);

        public int GetDifficulty() => _gameSettings[1];
    }

    [System.Serializable]
    public class RemainingShip
    {
        public GameObject Cross;
        public Text RemainingText;
        public int RemainingShips;
    }
}