using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;


namespace PahlBit
{
    public class PlatformerPathfinder : MonoBehaviour
    {
        [SerializeField] Tilemap GroundTilemap;
        [SerializeField] Tilemap ThinPlatformTilemap;

        Dictionary<Vector2Int, NodeNav> mGroundNodes = new Dictionary<Vector2Int, NodeNav>();
        List<NodeNavGroup> mNodeGroups = new List<NodeNavGroup>();

        void Awake()
        {
            Init(GroundTilemap, ThinPlatformTilemap);
        }

        public void Init(Tilemap tilemap, Tilemap thinTilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            BoundsInt thinBounds = thinTilemap.cellBounds;
            bounds.min = Vector3Int.Min(bounds.min, thinBounds.min);
            bounds.max = Vector3Int.Max(bounds.max, thinBounds.max);
            NodeNavGroup groundNodeGroup = null;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (mGroundNodes.ContainsKey(pos))
                        continue;

                    while (IsGroundTile(tilemap, pos))
                    {
                        if (groundNodeGroup == null)
                        {
                            groundNodeGroup = new NodeNavGroup();
                            groundNodeGroup.IsThinPlatform = false;
                        }

                        NodeNav newNode = new NodeNav(pos);
                        newNode.ParentGroup = groundNodeGroup;
                        newNode.IndexInGroup = groundNodeGroup.GroundNodes.Count;
                        newNode.IsThin = false;
                        groundNodeGroup.GroundNodes.Add(newNode);
                        mGroundNodes[pos] = newNode;
                        pos.x++;
                    }

                    while (IsThinTile(thinTilemap, pos))
                    {
                        if (groundNodeGroup == null)
                        {
                            groundNodeGroup = new NodeNavGroup();
                            groundNodeGroup.IsThinPlatform = true;
                        }

                        NodeNav newNode = new NodeNav(pos);
                        newNode.ParentGroup = groundNodeGroup;
                        newNode.IndexInGroup = groundNodeGroup.GroundNodes.Count;
                        newNode.IsThin = true;
                        groundNodeGroup.GroundNodes.Add(newNode);
                        mGroundNodes[pos] = newNode;
                        pos.x++;
                    }

                    if (groundNodeGroup != null)
                    {
                        LinkGroups(groundNodeGroup);
                        mNodeGroups.Add(groundNodeGroup);
                        groundNodeGroup = null;
                    }
                }
            }

            InitTransitions();
        }

        void InitTransitions()
        {
            foreach (var group in mNodeGroups)
            {
                group.InitTransitions();
            }
        }

        bool IsGroundTile(Tilemap tilemap, Vector2Int position)
        {
            Vector3Int pos = new Vector3Int(position.x, position.y, 0);
            return tilemap.HasTile(pos) && !tilemap.HasTile(pos + Vector3Int.up);
        }
        bool IsThinTile(Tilemap tilemap, Vector2Int position)
        {
            Vector3Int pos = new Vector3Int(position.x, position.y, 0);
            return tilemap.gameObject.layer == LayerID.ThinPlatform && tilemap.HasTile(pos);
        }

        void LinkGroups(NodeNavGroup newGroup)
        {
            Rect newRect = newGroup.GetRect();
            newRect.min -= new Vector2(14, 9);
            newRect.max += new Vector2(14, 9);
            foreach (NodeNavGroup group in mNodeGroups)
            {
                Rect groupRect = group.GetRect();
                if (newRect.Overlaps(groupRect))
                {
                    newGroup.LinkedGroups.Add(group);
                    group.LinkedGroups.Add(newGroup);
                }
            }
        }

        public List<PathInfo> FindPossiblePaths(NodeNav startNode, float moveSpeed)
        {
            NodeNavGroup currentGroup = startNode.ParentGroup;
            List<PathInfo> possiblePaths = new List<PathInfo>();
            foreach (var transition in currentGroup.Transitions)
            {
                if (transition.TransitionType == NodeTransitionType.JustJumpUp)
                {
                    bool isPossibleJump = JumpSimulationTable.IsPossibleJump(
                        startPos: transition.StartNode.Position,
                        destPos: transition.EndNode.Position,
                        horizontalMoveSpeed: moveSpeed,
                        out int requiredJumpLevel
                    );

                    if (isPossibleJump)
                    {
                        NodeNav endNode = transition.EndNode.ParentGroup.GetNodeAtWorldPosX(startNode.Position.x);
                        bool isNoNeedToMove = endNode != null;

                        PathInfo pathInfo = new PathInfo();
                        pathInfo.Transition = transition;
                        pathInfo.JumpForce = JumpSimulationTable.JumpLevelToForce(requiredJumpLevel);
                        pathInfo.IsNoNeedToMove = isNoNeedToMove;
                        possiblePaths.Add(pathInfo);
                    }
                }
                else if (transition.TransitionType == NodeTransitionType.DropDown)
                {
                    NodeNav endNode = transition.EndNode.ParentGroup.GetNodeAtWorldPosX(startNode.Position.x);
                    bool isNoNeedToMove = endNode != null;

                    PathInfo pathInfo = new PathInfo();
                    pathInfo.Transition = transition;
                    pathInfo.IsNoNeedToMove = isNoNeedToMove;
                    possiblePaths.Add(pathInfo);
                }
                else
                {
                    bool isPossibleJump = JumpSimulationTable.IsPossibleJump(
                        startPos: transition.StartNode.Position,
                        destPos: transition.EndNode.Position,
                        horizontalMoveSpeed: moveSpeed,
                        out int requiredJumpLevel
                    );

                    if (isPossibleJump && !transition.IsBlocked(requiredJumpLevel))
                    {
                        PathInfo pathInfo = new PathInfo();
                        pathInfo.Transition = transition;
                        pathInfo.JumpForce = JumpSimulationTable.JumpLevelToForce(requiredJumpLevel);
                        possiblePaths.Add(pathInfo);
                    }
                }
            }
            return possiblePaths;
        }

        public PathInfo FindPathAround(NodeNav startNode, float moveSpeed, int aroundNodeRange)
        {
            NodeNav currentNode = startNode;
            if (currentNode == null)
                return null;

            List<PathInfo> possiblePaths = FindPossiblePaths(currentNode, moveSpeed);

            NodeNavGroup currentGroup = startNode.ParentGroup;
            float curPosWeight = GetMinWeight(currentGroup);
            List<PathInfo> nextPaths = new List<PathInfo>();
            foreach (var path in possiblePaths)
            {
                float weight = GetMinWeight(path.Transition.EndNode.ParentGroup);
                if (weight <= aroundNodeRange)
                {
                    nextPaths.Add(path);
                }
                else if (weight <= curPosWeight)
                {
                    nextPaths.Add(path);
                }
            }
            return nextPaths.ExGetRandom();
        }

        public PathInfo FindMinPath(NodeNav startNode, float moveSpeed)
        {
            NodeNav currentNode = startNode;
            if (currentNode == null)
                return null;

            List<PathInfo> possiblePaths = FindPossiblePaths(currentNode, moveSpeed);

            PathInfo selectedPath = null;
            float minWeight = float.MaxValue;
            foreach (var path in possiblePaths)
            {
                float weight = GetMinWeight(path.Transition.EndNode.ParentGroup);
                if (weight < minWeight)
                {
                    minWeight = weight;
                    selectedPath = path;
                }
            }
            return selectedPath;
        }

        public NodeNav GetCurrentGroundNode(Vector2 worldPos)
        {
            Vector2Int nodePos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y) - 1);
            if (mGroundNodes.ContainsKey(nodePos))
                return mGroundNodes[nodePos];
            return null;
        }
        public NodeNav GetCurrentGroundNode(Rect worldArea)
        {
            Vector2 centerBottom = new Vector2(worldArea.center.x, worldArea.yMin);
            NodeNav node = GetCurrentGroundNode(centerBottom);
            if (node != null)
                return node;

            Vector2 rightBottom = new Vector2(worldArea.xMax, worldArea.yMin);
            node = GetCurrentGroundNode(rightBottom);
            if (node != null)
                return node;

            Vector2 leftBottom = new Vector2(worldArea.xMin, worldArea.yMin);
            node = GetCurrentGroundNode(leftBottom);
            if (node != null)
                return node;

            return null;
        }
        public NodeNav GetNode(Vector2 worldPos)
        {
            Vector2Int nodePos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));
            if (mGroundNodes.ContainsKey(nodePos))
                return mGroundNodes[nodePos];
            return null;
        }
        public float GetMinWeight(NodeNavGroup nodeGroup)
        {
            float minWeight = float.MaxValue;
            foreach (var node in nodeGroup.GroundNodes)
            {
                Vector2Int nodeUpPos = node.Position + new Vector2Int(0, 1);
                PlayerDepthInfo depthInfo = InGameManager.Instance.Engine.DepthManager.GetPlayerDepthInfoAtPos(nodeUpPos);
                if (depthInfo == null)
                    continue;

                float curWeight = depthInfo.GetWeight();
                if (curWeight < minWeight)
                    minWeight = curWeight;
            }

            return minWeight;
        }
    }

    public class PathInfo
    {
        public NodeTransition Transition { get; set; }
        public float JumpForce { get; set; }
        public bool IsNoNeedToMove { get; set; } = false;
    }
}