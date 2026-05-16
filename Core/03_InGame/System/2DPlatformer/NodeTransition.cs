using System;
using UnityEngine;


namespace PahlUnity
{
    public class NodeTransition
    {
        public NodeTransitionType TransitionType { get; set; }
        public NodeNav StartNode { get; set; }
        public NodeNav EndNode { get; set; }
        public bool[] IsBlockedJumping = new bool[4];
        public bool IsBlocked(int jumpLevel) { return IsBlockedJumping[jumpLevel - 1]; }
    }

    public enum NodeTransitionType
    {
        None,
        WalkAndFall, // 그냥 바로 아래칸에 지면이 있는 경우 걸어가다가 떨이지는 동작
        JustJumpUp, // 좌우 이동 없이 점프만 하는 동작(주로 얇은 지형 위로 이동시)
        MovingJump, // 이동하면서 점프
        JumpAndMove, // 점프하고 최고위치 도달시 이동(주로 바로 붙어있는 높은 지형 이동시)
        DropDown, // 얇은지형에서 아래칸으로 이동시
        Dash, // 대쉬 이동시
        DoubleJump, // 더블점프 이동시
    }
}