using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Study.Tester.PathFinding
{
    public class PathFindingTileTester : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        public int Index { get; private set; }

        public bool IsObstacle { get; set; }

        public void Init(int _index)
        {
            Index = _index;
        }

        public void SetColor(Color _color)
        {
            spriteRenderer.color = _color;
        }
    }
}