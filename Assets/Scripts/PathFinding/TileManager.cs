using System;
using System.Collections;
using System.Collections.Generic;
using Study.PathFinding;
using Unity.VisualScripting;
using UnityEngine;

namespace Study.Tile
{
    public class TileManager : MonoBehaviour
    {
        public Astar Astar { get; private set; }

        private static TileManager instance;

        public static TileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TileManager>();
                    instance.Init();
                }

                return instance;
            }
        }

        private void Init()
        {
            Astar = new Astar();
            Astar.Init();
        }

        public int GetTileIndex(Vector3 _position)
        {
            var x = (int)(_position.x / Study.Define.Tile.TileSize % Define.PathFinding.TileWidthCount);
            var y = (int)(_position.y / Study.Define.Tile.TileSize % Define.PathFinding.TileHeightCount);

            return x + y * Define.PathFinding.TileWidthCount;
        }

        public Vector3 GetTilePosition(int _index)
        {
            var x = _index % Define.PathFinding.TileWidthCount;
            var y = _index / Define.PathFinding.TileWidthCount;

            return new Vector3(x * Define.Tile.TileSize, y * Define.Tile.TileSize, 0);
        }
    }
}