using System;
using System.Collections;
using System.Collections.Generic;
using Study.PathFinding;
using Unity.VisualScripting;
using UnityEngine;

using static Study.Define.PathFinding;
using Random = UnityEngine.Random;

namespace Study.Tester.PathFinding
{
    public class PathFindingTester : MonoBehaviour
    {
        public enum TesterState
        {
            None,
            MoveStartIndex,
            MoveTargetIndex,
            SetObstacle,
        }
        
        private Color defaultColor = Color.white;
        private Color obstacleColor = Color.black;
        private Color startColor = Color.red;
        private Color targetColor = Color.blue;
        private Color pathColor = Color.gray;

        [SerializeField] PathFindingTileTester prefabTile;

        [SerializeField] private int obstacleCount;

        private List<PathFindingTileTester> tileList = new List<PathFindingTileTester>(TileTotalCount);

        private int startIndex;
        private int targetIndex;

        private TesterState state;

        private Astar astar;
        
        void Start()
        {
            astar = new Astar();
            astar.Init();
            startIndex = 0;
            targetIndex = TileTotalCount - 1;
            state = TesterState.None;
            
            var cam = Camera.main;
            var camPos = cam.transform.position;
            camPos.x = TileWidthCount / 2.0f;
            camPos.y = TileHeightCount / 2.0f;
            cam.transform.position = camPos;
            cam.orthographicSize = TileHeightCount / 2.0f + 2.0f;
            
            PathFindingTileTester[] arrNode = new PathFindingTileTester[TileWidthCount]; 
            for (var y = 0; y < TileHeightCount; ++y)
            {
                for (var x = 0; x < TileWidthCount; ++x)
                {
                    var tile = Instantiate(prefabTile);
                    tile.Init(y * TileWidthCount + x);
                    tile.SetColor(defaultColor);
                    tile.transform.position = new Vector3(x, y, 0);
                    arrNode[x] = tile;
                }
                
                tileList.AddRange(arrNode);
            }

            ClearTile();
        }

        private void Update()
        {
            // 1번 : 시작 위치 변경
            // 2번 : 도착 위치 변경
            // 3번 : 장애물 설치
            // 4번 : 경로 탐색
            // 5번 : 클리어
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                state = TesterState.MoveStartIndex;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                state = TesterState.MoveTargetIndex;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                state = TesterState.SetObstacle;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                var path = astar.FindPath(startIndex, targetIndex);
                foreach (var index in path)
                {
                    tileList[index].SetColor(pathColor);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ClearTile();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                for (int i = 0; i < obstacleCount; ++i)
                {
                    var index = Random.Range(0, TileTotalCount - 1);
                    if (index == startIndex || index == targetIndex) continue;
                    
                    tileList[index].IsObstacle = true;
                    astar.SetObstacle(index, true);
                }
                
                ClearTile();
            }
            
            
            if (Input.GetMouseButtonDown(0))
            {
                if (state == TesterState.None) return;
                
                var hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hitInfo == false) return;

                var tile = hitInfo.transform.GetComponent<PathFindingTileTester>();

                switch (state)
                {
                    case TesterState.MoveStartIndex:
                        startIndex = tile.Index;
                        break;
                    case TesterState.MoveTargetIndex:
                        targetIndex = tile.Index;
                        break;
                    case TesterState.SetObstacle:
                        tile.IsObstacle = !tile.IsObstacle;
                        astar.SetObstacle(tile.Index, tile.IsObstacle);
                        break;
                    default:
                        return;
                }
                
                ClearTile();
                state = TesterState.None;
            }
        }

        private void ClearTile()
        {
            foreach (var tile in tileList)
            {
                if (tile.Index == startIndex)
                {
                    tile.SetColor(startColor);
                    continue;
                }

                if (tile.Index == targetIndex)
                {
                    tile.SetColor(targetColor);
                    continue;
                }

                if (tile.IsObstacle)
                {
                    tile.SetColor(obstacleColor);
                    continue;
                }
                
                tile.SetColor(defaultColor);
            }
        }
    }
}