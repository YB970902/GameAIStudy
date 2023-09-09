using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.Tile
{
    public class Player : MonoBehaviour
    {
        private List<int> path;
        private int currentPathIndex;

        [SerializeField] private float moveSpeed = 10;

        /// <summary> 이동 중인지 여부 플래그 </summary>
        private bool isMove;

        public void Init()
        {
            path = null;
            isMove = false;
            currentPathIndex = 0;
        }

        public void MoveTo(Vector3 _targetPosition)
        {
            var startIndex = TileManager.Instance.GetTileIndex(transform.position);
            var targetIndex = TileManager.Instance.GetTileIndex(_targetPosition);

            path = TileManager.Instance.Astar.FindPath(startIndex, targetIndex);
            currentPathIndex = 1;
            isMove = path != null && path.Count > 1;
        }

        private void Update()
        {
            if (isMove == false) return;

            var nextPath = TileManager.Instance.GetTilePosition(path[currentPathIndex]);
            var dir = (nextPath - transform.position).normalized;
            
            transform.position += moveSpeed * Time.deltaTime * dir;

            var nowDir = (nextPath - transform.position).normalized;

            if (Vector3.Dot(dir, nowDir) <= 0)
            {
                ++currentPathIndex;
                if (currentPathIndex == path.Count) isMove = false;
            }
        }
    }
}