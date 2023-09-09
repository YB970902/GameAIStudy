using System;
using System.Collections;
using System.Collections.Generic;
using Study.PathFinding;
using UnityEngine;

namespace Study.Tile
{
    public class TileMaker : MonoBehaviour
    {
        [SerializeField] private Player prefabPlayer;
        [SerializeField] private Tile prefabTile;

        private Player player;
        private Tile[] tiles = new Tile[Define.PathFinding.TileTotalCount];

        private void Start()
        {
            player = Instantiate(prefabPlayer);
            player.Init();
            player.transform.position = TileManager.Instance.GetTilePosition(0);
            for (int i = 0; i < Define.PathFinding.TileTotalCount; ++i)
            {
                var tile = Instantiate(prefabTile);
                tiles[i] = tile;
                tile.transform.position = new Vector3(
                    i % Define.PathFinding.TileWidthCount,
                    i / Define.PathFinding.TileWidthCount, 
                    0.0f);
                tile.SetObstacle(false);
            }

            Camera.main.orthographicSize = Define.PathFinding.TileHeightCount / 2.0f + 2.0f;
            Camera.main.transform.position += new Vector3(
                Define.PathFinding.TileWidthCount / 2.0f,
                Define.PathFinding.TileHeightCount / 2.0f, 
                0);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var tile = GetClickedTile();
                if (tile == null) return;
                var isObstacle = !tile.IsObstacle;
                tile.SetObstacle(isObstacle);
                TileManager.Instance.Astar.SetObstacle(TileManager.Instance.GetTileIndex(tile.transform.position), isObstacle);
            }

            if (Input.GetMouseButtonDown(1))
            {
                var tile = GetClickedTile();
                if (tile == null) return;
                player.MoveTo(tile.transform.position);
            }
        }

        private Tile GetClickedTile()
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var x = (int)(worldPosition.x / Define.Tile.TileSize);
            var y = (int)(worldPosition.y / Define.Tile.TileSize);

            if (x < 0 || y < 0 ||
                x >= Define.PathFinding.TileWidthCount ||
                y >= Define.PathFinding.TileHeightCount) return null;

            return tiles[x + y * Define.PathFinding.TileWidthCount];
        }
    }
}