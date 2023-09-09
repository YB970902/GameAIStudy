using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.Define
{
    public class Tile
    {
        public const float TileSize = 1.0f;
        public const float TileHalfSize = 0.5f;
    }
    
    public class PathFinding
    {
        public const int TileWidthCount = 10;
        public const int TileHeightCount = 10;
        public const int TileTotalCount = TileWidthCount * TileHeightCount;

        public enum Direct
        {
            Left,
            Right,
            Up,
            Down,
            End,
        }

        public enum DiagnoalDirect
        {
            LeftUp,
            RightUp,
            LeftDown,
            RightDown,
            End,
        }
    }
}
