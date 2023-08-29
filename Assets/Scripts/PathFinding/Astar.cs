using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Study.Define.PathFinding;

namespace Study.PathFinding
{
    /// <summary>
    /// 시작 인덱스, 목표인덱스를 넣으면 결과를 List<int>형태로 반환하는 클래스
    /// </summary>
    public class Astar
    {
        public class Node
        {
            private readonly int index;
            public int Index => index;
            
            public Node(int _index)
            {
                index = _index;
            }
            
            // 계산에 필요한 값
            public int G { get; set; }
            public int H { get; private set; }
            public int F => G + H;
            
            public bool IsOpen { get; set; }
            public bool IsClose { get; set; }
            /// <summary> 장애물 여부 </summary>
            public bool IsObstacle { get; set; }

            public Node Parent { get; set; }

            public void Reset()
            {
                G = 0;
                H = 0;
                IsOpen = false;
                IsClose = false;
                Parent = null;
            }

            public void Calc(int _parentIndex, int _targetIndex, int _g)
            {
                G = CalcG(_parentIndex, Index, _g);
                H = CalcH(_targetIndex, Index);
            }

            public static int CalcG(int _parentIndex, int _childIndex, int _g)
            {
                var (parentX, parentY) = IndexToPosition(_parentIndex);
                var (childX, childY) = IndexToPosition(_childIndex);

                if (parentX == childX || parentY == childY) return _g + 10;
                return _g + 14;
            }

            private static int CalcH(int _targetIndex, int _currentIndex)
            {
                var (targetX, targetY) = IndexToPosition(_targetIndex);
                var (currentX, currentY) = IndexToPosition(_currentIndex);

                return (Mathf.Abs(targetX - currentX) + Mathf.Abs(targetY - currentY)) * 10;
            }
        }

        private List<Node> nodeList = new List<Node>(TileTotalCount);
        private List<Node> openList = new List<Node>(TileTotalCount);

        /// <summary>
        /// Astar 초기화
        /// </summary>
        public void Init()
        {
            Node[] arrNode = new Node[TileWidthCount]; 
            for (var y = 0; y < TileHeightCount; ++y)
            {
                for (var x = 0; x < TileWidthCount; ++x)
                {
                    arrNode[x] = new Node(y * TileWidthCount + x);
                }
                
                nodeList.AddRange(arrNode);
            }
        }

        /// <summary>
        /// 길찾기
        /// </summary>
        /// <param name="_startIndex">시작 인덱스</param>
        /// <param name="_targetIndex">도착 인덱스</param>
        public List<int> FindPath(int _startIndex, int _targetIndex)
        {
            if (IsOutOfNode(_startIndex) || IsOutOfNode(_targetIndex)) return null;
            if (_startIndex == _targetIndex) return null;
            
            nodeList.ForEach(node => node.Reset());
            openList.Clear();

            // 탐색을 하는 노드
            var currentNode = nodeList[_startIndex];

            while (currentNode != null)
            {
                if (currentNode.Index == _targetIndex) break;

                currentNode.IsOpen = false;
                currentNode.IsClose = true;

                var nearNode = FindNearNode(currentNode.Index);

                foreach (var node in nearNode)
                {
                    if (node.IsOpen == false)
                    {
                        node.Calc(currentNode.Index, _targetIndex, currentNode.G);
                        node.IsOpen = true;
                        node.Parent = currentNode;
                        openList.Add(node);
                        continue;
                    }

                    var newG = Node.CalcG(currentNode.Index, node.Index, currentNode.G);
                    if (newG > node.G) continue;

                    node.G = newG;
                    node.Parent = currentNode;
                }

                currentNode = GetMinFOpenNode();
            }

            // 경로를 못찾은 경우
            if (currentNode == null) return null;
            
            List<int> result = new List<int>();

            while (currentNode != null)
            {
                result.Add(currentNode.Index);
                currentNode = currentNode.Parent;
            }

            result.Reverse();
            
            return result;
        }

        public void SetObstacle(int _index, bool _value)
        {
            nodeList[_index].IsObstacle = _value;
        }

        private Node GetMinFOpenNode()
        {
            var minF = int.MaxValue;
            Node minNode = null;

            foreach (var node in openList)
            {
                if (node.F >= minF) continue;

                minF = node.F;
                minNode = node;
            }

            if (minNode == null) return null;

            openList.Remove(minNode);
            return minNode;
        }

        private readonly (int, int)[] dtDirect = new (int, int)[]
        {
            (-1, 0), (1, 0), (0, 1), (0, -1)
        };

        List<Node> nearNodeList = new List<Node>(8);
        // bool * 4 [left][right][up][down]
        //           true <- 막혀있지 않음. false <- 막혀있음.
        private bool[] nearNodeFlag = new bool[4];
        private List<Node> FindNearNode(int _currentNodeIndex)
        {
            nearNodeList.Clear();

            var (posX, posY) = IndexToPosition(_currentNodeIndex);

            for (var i = (int)Direct.Left; i < (int)Direct.End; ++i)
            {
                var (dtX, dtY) = dtDirect[i];
                var nodePos = (posX + dtX, posY + dtY);

                nearNodeFlag[i] = !IsObstacle(nodePos);

                if (CanAddOpenList(nodePos) == false) continue;
                
                nearNodeList.Add(nodeList[PositionToIndex(nodePos)]);
            }

            for (var i = (int)DiagnoalDirect.LeftUp; i < (int)DiagnoalDirect.End; ++i)
            {
                if (CanMoveDiagonal(i) == false) continue;

                var (dtX, dtY) = dtDiagonal[i];
                var nodePos = (posX + dtX, posY + dtY);

                if (CanAddOpenList(nodePos) == false) continue;
                
                nearNodeList.Add(nodeList[PositionToIndex(nodePos)]);
            }

            return nearNodeList;
        }

        private readonly (int, int)[] diagonalFlagLookup = new (int, int)[]
        {
            ((int)Direct.Left, (int)Direct.Up),
            ((int)Direct.Right, (int)Direct.Up),
            ((int)Direct.Left, (int)Direct.Down),
            ((int)Direct.Right, (int)Direct.Down)
        };

        private readonly (int, int)[] dtDiagonal = new (int, int)[]
        {
            (-1, 1),
            (1, 1),
            (-1, -1),
            (1, -1)
        };

        /// <summary>
        /// 이 방향의 대각선으로 이동할 수 있는지 여부
        /// </summary>
        /// <param name="_direct">DiagonalDirect를 int로 형변환한 값</param>
        /// <returns></returns>
        private bool CanMoveDiagonal(int _direct)
        {
            var (dir1, dir2) = diagonalFlagLookup[_direct];
            return nearNodeFlag[dir1] && nearNodeFlag[dir2];
        }

        private bool CanAddOpenList((int, int) _pos)
        {
            if (IsOutOfNode(_pos)) return false;
            
            var node = nodeList[PositionToIndex(_pos)];

            if (node.IsClose) return false;
            if (node.IsObstacle) return false;
            
            return true;
        }

        private bool IsObstacle((int, int) _pos)
        {
            if (IsOutOfNode(_pos)) return true;

            return nodeList[PositionToIndex(_pos)].IsObstacle;
        }

        private static (int, int) IndexToPosition(int _index)
        {
            return (_index % TileWidthCount, _index / TileWidthCount);
        }

        private static int PositionToIndex((int, int) _pos)
        {
            var (x, y) = _pos;
            return y * TileWidthCount + x;
        }
        
        private bool IsOutOfNode((int, int) _pos)
        {
            var (x, y) = _pos;

            if (x < 0 || x >= TileWidthCount ||
                y < 0 || y >= TileHeightCount) return true;
            
            return false;
        }

        private bool IsOutOfNode(int _index)
        {
            if (_index < 0 || _index >= TileTotalCount) return true;

            return false;
        }
    }
}