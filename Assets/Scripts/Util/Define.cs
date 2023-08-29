using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.Define
{
    public class PathFinding
    {
        public const int TileWidthCount = 30;
        public const int TileHeightCount = 30;
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
