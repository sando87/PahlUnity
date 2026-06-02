using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor.Experimental.GraphView;


namespace PahlUnity.Demo
{
    public class NodeNav
    {
        public Vector2 MinPos { get => new Vector2(Position.x, Position.y); }
        public Vector2 MaxPos { get => new Vector2(Position.x + 1, Position.y + 1); }
        public Vector2Int MinPosInt { get => Position; }
        public Vector2Int MaxPosInt { get => new Vector2Int(Position.x + 1, Position.y + 1); }
        public Vector2 CenterPos { get => new Vector2(Position.x + 0.5f, Position.y + 0.5f); }
        public Vector2 CenterTopPos { get => new Vector2(Position.x + 0.5f, Position.y + 1.0f); }
        public NodeNav RightNode { get => ParentGroup.GetNodeAtIndex(IndexInGroup + 1); }
        public NodeNav LeftNode { get => ParentGroup.GetNodeAtIndex(IndexInGroup - 1); }

        public Vector2Int Position { get; private set; }
        public NodeNavGroup ParentGroup { get; set; }
        public int IndexInGroup { get; set; } = -1;
        public bool IsThin { get; set; } = false;

        public NodeNav(Vector2Int position)
        {
            Position = position;
        }

    }
}