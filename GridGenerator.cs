using UnityEngine;

namespace InfiniteVox
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private bool _playerMove;
        [SerializeField] private Tile _tile;

        private const int GridSize = 6; // Размер сетки (7x7)
        private const float TileSize = 10f; // Размер одного тайла

        private void Awake()
        {
            Vector3 startPosition = transform.position;

            for (int z = 0; z < GridSize; z++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    Vector3 position = startPosition + new Vector3(x * TileSize, 0f, -z * TileSize);
                    Tile tile = Instantiate(_tile.gameObject, position, transform.rotation, transform).GetComponent<Tile>();
                    tile.PlayerMove = _playerMove;
                }
            }
        }
    }
}