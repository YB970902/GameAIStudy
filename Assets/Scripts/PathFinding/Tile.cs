using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.Tile
{
    public class Tile : MonoBehaviour
    {
        private static readonly Color DefaultColor = Color.white;
        private static readonly Color ObstacleColor = Color.black;

        [SerializeField] private SpriteRenderer spriteRenderer;

        public bool IsObstacle { get; private set; }
        private void Start()
        {
            spriteRenderer.color = DefaultColor;
        }

        public void SetObstacle(bool _isObstacle)
        {
            IsObstacle = _isObstacle;
            spriteRenderer.color = _isObstacle ? ObstacleColor : DefaultColor;
        }
    }
}