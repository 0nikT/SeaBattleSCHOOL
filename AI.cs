using System.Collections.Generic;
using UnityEngine;

namespace InfiniteVox
{
    public class AI : MonoBehaviour
    {
        private List<Tile> _shootTiles = new List<Tile>();
        private GameController _gameController;
        private ShipGrid _shipGrid;
        private int _difficulty;
        private int _hitCount;

        private void OnEnable()
        {
            _gameController = GameController.Base;
            _difficulty = _gameController.GetDifficulty();
        }

        private void Update()
        {
            if (_shipGrid)
            {
                if (_shipGrid.Destroyed)
                    _shipGrid = null;
            }
        }

        public void Move() => Invoke(nameof(Shot), 1f);

        private void Shot()
        {
            if (_gameController.Win)
                return;

            List<Tile> tiles = new List<Tile>();
            List<Tile> addTiles = new List<Tile>();
            bool hit = _difficulty == 3 && Random.Range(0, 101) <= 50;

            addTiles.AddRange(_shipGrid ? _shootTiles : FindObjectsOfType<Tile>());

            foreach (Tile addTile in addTiles)
            {
                if (addTile.Hit || addTile.PlayerMove || (hit && addTile.CurrentShip == null))
                    continue;

                tiles.Add(addTile);
            }

            Tile tile = tiles[Random.Range(0, tiles.Count)];

            if (_difficulty > 1 && tile.CurrentShip != null)
            {
                if (tile.CurrentShip.TilesMax > 1)
                {
                    _shootTiles.Clear();

                    if (_shipGrid == null)
                    {
                        _shipGrid = tile.CurrentShip;
                        _hitCount = 0;
                    }

                    _hitCount++;
                    _shootTiles.AddRange(tile.GetShootTiles(_hitCount > 1));
                }

            }

            if (_difficulty == 1)
            {            ShipGrid[] shipGrids = FindObjectsOfType<ShipGrid>();
            foreach (ShipGrid shipGrid in shipGrids)
                    shipGrid.FlickShip();
 
            }

            _gameController.Shot(tile.transform);
        }
        
    }
}