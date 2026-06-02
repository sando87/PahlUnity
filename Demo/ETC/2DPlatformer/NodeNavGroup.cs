using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;


namespace PahlUnity.Demo
{
    public class NodeNavGroup
    {
        public bool IsThinPlatform { get; set; } = false;
        public List<NodeNav> GroundNodes = new List<NodeNav>();
        public List<NodeNavGroup> LinkedGroups = new List<NodeNavGroup>();
        public List<NodeTransition> Transitions = new List<NodeTransition>();

        public NodeNav MostLeftNode { get => GroundNodes[0]; }
        public NodeNav MostRightNode { get => GroundNodes[GroundNodes.Count - 1]; }

        public void InitTransitions()
        {
            Transitions.Clear();
            foreach (var linkedGroup in LinkedGroups)
            {
                List<NodeTransition> transitions = new List<NodeTransition>();
                bool hasTransitions = CreateTransitions(linkedGroup, transitions);
                if (!hasTransitions)
                    continue;

                Transitions.AddRange(transitions);
            }
        }

        // 이게 핵심 알고리즘...
        // 주변 지형과의 위치나 높이에 따라 이동 가능 판단 및 이동 속성 부여
        // 정적으로 미리 계산해 둘 수 있는 정보는 모두 여기서 처리해서 저장해 둠
        // 주변 지형의 복잡도에 따라 구현 난이도 상승..
        bool CreateTransitions(NodeNavGroup targetGroup, List<NodeTransition> transitions)
        {
            RectInt targetRect = targetGroup.GetRectInt();
            RectInt myRect = GetRectInt();

            // case 1: 타겟이 완전히 왼쪽으로 나가 있는 경우
            if (targetRect.xMax <= myRect.xMin)
            {
                NodeNav startNode = MostLeftNode;
                NodeNav endNode = targetGroup.MostRightNode;

                if (startNode.Position.x - 1 == endNode.Position.x) // 바로 옆칸에 붙어있는 경우
                {
                    if (startNode.Position.y > endNode.Position.y) // 바로 옆칸인데 아래에 지면이 있는경우
                    {
                        NodeTransition transition = new NodeTransition();
                        transition.StartNode = startNode;
                        transition.EndNode = endNode;
                        transition.TransitionType = NodeTransitionType.WalkAndFall;
                        transitions.Add(transition);
                    }
                    else // 바로 옆칸인데 위로 지면이 있는경우
                    {
                        NodeNav nextNode = startNode.RightNode;
                        if (nextNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = nextNode;
                            transition.EndNode = endNode;
                            transition.TransitionType = NodeTransitionType.MovingJump;
                            CalcJumpCastBlocked(transition);
                            transitions.Add(transition);
                        }
                        else
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = endNode;
                            transition.TransitionType = NodeTransitionType.JumpAndMove;
                            transitions.Add(transition);
                        }
                    }
                }
                else
                {
                    NodeTransition transition = new NodeTransition();
                    transition.StartNode = startNode;
                    transition.EndNode = endNode;
                    transition.TransitionType = NodeTransitionType.MovingJump;
                    CalcJumpCastBlocked(transition);
                    transitions.Add(transition);
                }
            }
            // case 2: 타겟이 완전히 오른쪽으로 나가 있는 경우
            else if (myRect.xMax <= targetRect.xMin)
            {
                NodeNav startNode = MostRightNode;
                NodeNav endNode = targetGroup.MostLeftNode;

                if (startNode.Position.x + 1 == endNode.Position.x) // 바로 옆칸에 붙어있는 경우
                {
                    if (startNode.Position.y > endNode.Position.y) // 바로 옆칸인데 아래에 지면이 있는경우
                    {
                        NodeTransition transition = new NodeTransition();
                        transition.StartNode = startNode;
                        transition.EndNode = endNode;
                        transition.TransitionType = NodeTransitionType.WalkAndFall;
                        transitions.Add(transition);
                    }
                    else // 바로 옆칸인데 위로 지면이 있는경우
                    {
                        NodeNav nextNode = startNode.LeftNode;
                        if (nextNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = nextNode;
                            transition.EndNode = endNode;
                            transition.TransitionType = NodeTransitionType.MovingJump;
                            CalcJumpCastBlocked(transition);
                            transitions.Add(transition);
                        }
                        else
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = endNode;
                            transition.TransitionType = NodeTransitionType.JumpAndMove;
                            transitions.Add(transition);
                        }
                    }
                }
                else
                {
                    NodeTransition transition = new NodeTransition();
                    transition.StartNode = startNode;
                    transition.EndNode = endNode;
                    transition.TransitionType = NodeTransitionType.MovingJump;
                    CalcJumpCastBlocked(transition);
                    transitions.Add(transition);
                }
            }
            // case 3: 타겟이 나의 왼쪽만 겹치는 경우
            else if (targetRect.xMin < myRect.xMin && myRect.xMin < targetRect.xMax && targetRect.xMax <= myRect.xMax)
            {
                if (targetRect.center.y > myRect.center.y) // 타겟지형이 위쪽일 경우
                {
                    if (targetGroup.IsThinPlatform)
                    {
                        int targetMostRightNodeWorldPosX = targetGroup.MostRightNode.Position.x;
                        NodeNav startNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX);
                        if (startNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = targetGroup.MostRightNode;
                            transition.TransitionType = NodeTransitionType.JustJumpUp;
                            transitions.Add(transition);
                        }
                    }
                    else
                    {
                        int targetMostRightNodeWorldPosX = targetGroup.MostRightNode.Position.x;
                        NodeNav startNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX + 2);
                        if (startNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = targetGroup.MostRightNode;
                            transition.TransitionType = NodeTransitionType.MovingJump;
                            CalcJumpCastBlocked(transition);
                            transitions.Add(transition);
                        }
                        else
                        {
                            startNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX + 1);
                            if (startNode != null)
                            {
                                NodeTransition transition = new NodeTransition();
                                transition.StartNode = startNode;
                                transition.EndNode = targetGroup.MostRightNode;
                                transition.TransitionType = NodeTransitionType.JumpAndMove;
                                transitions.Add(transition);
                            }
                        }
                    }
                }
                else // 타겟지형이 아래쪽일 경우
                {
                    if (IsThinPlatform)
                    {
                        int targetMostRightNodeWorldPosX = targetGroup.MostRightNode.Position.x;
                        NodeNav startNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX);
                        if (startNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = targetGroup.MostRightNode;
                            transition.TransitionType = NodeTransitionType.DropDown;
                            transitions.Add(transition);
                        }
                    }
                    else
                    {
                        int myMostLeftNodeWorldPosX = MostLeftNode.Position.x;
                        NodeNav endNode = targetGroup.GetNodeAtWorldPosX(myMostLeftNodeWorldPosX - 1);
                        if (endNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostLeftNode;
                            transition.EndNode = endNode;
                            transition.TransitionType = NodeTransitionType.WalkAndFall;
                            transitions.Add(transition);
                        }
                    }
                }
            }
            // case 4: 타겟이 나의 오른쪽만 겹치는 경우
            else if (myRect.xMin <= targetRect.xMin && targetRect.xMin < myRect.xMax && myRect.xMax < targetRect.xMax)
            {
                if (targetRect.center.y > myRect.center.y) // 타겟지형이 위쪽일 경우
                {
                    if (targetGroup.IsThinPlatform)
                    {
                        int targetMostLeftNodeWorldPosX = targetGroup.MostLeftNode.Position.x;
                        NodeNav startNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX);
                        if (startNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = targetGroup.MostLeftNode;
                            transition.TransitionType = NodeTransitionType.JustJumpUp;
                            transitions.Add(transition);
                        }
                    }
                    else
                    {
                        int targetMostLeftNodeWorldPosX = targetGroup.MostLeftNode.Position.x;
                        NodeNav startNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX - 2);
                        if (startNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = targetGroup.MostLeftNode;
                            transition.TransitionType = NodeTransitionType.MovingJump;
                            CalcJumpCastBlocked(transition);
                            transitions.Add(transition);
                        }
                        else
                        {
                            startNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX - 1);
                            if (startNode != null)
                            {
                                NodeTransition transition = new NodeTransition();
                                transition.StartNode = startNode;
                                transition.EndNode = targetGroup.MostLeftNode;
                                transition.TransitionType = NodeTransitionType.JumpAndMove;
                                transitions.Add(transition);
                            }
                        }
                    }
                }
                else // 타겟지형이 아래쪽일 경우
                {
                    if (IsThinPlatform)
                    {
                        int targetMostLeftNodeWorldPosX = targetGroup.MostLeftNode.Position.x;
                        NodeNav startNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX);
                        if (startNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startNode;
                            transition.EndNode = targetGroup.MostLeftNode;
                            transition.TransitionType = NodeTransitionType.DropDown;
                            transitions.Add(transition);
                        }
                    }
                    else
                    {
                        int myMostRightNodeWorldPosX = MostRightNode.Position.x;
                        NodeNav endNode = targetGroup.GetNodeAtWorldPosX(myMostRightNodeWorldPosX + 1);
                        if (endNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostRightNode;
                            transition.EndNode = endNode;
                            transition.TransitionType = NodeTransitionType.WalkAndFall;
                            transitions.Add(transition);
                        }
                    }
                }
            }
            // case 5: 타겟이 내 안에 포함된 있는 경우
            else if (myRect.xMin <= targetRect.xMin && targetRect.xMax <= myRect.xMax)
            {
                if (myRect.center.y < targetRect.center.y)
                {
                    if (targetGroup.IsThinPlatform)
                    {
                        int targetMostLeftNodeWorldPosX = targetGroup.MostLeftNode.Position.x;
                        NodeNav startLeftNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX);
                        if (startLeftNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startLeftNode;
                            transition.EndNode = targetGroup.MostLeftNode;
                            transition.TransitionType = NodeTransitionType.JustJumpUp;
                            transitions.Add(transition);
                        }

                        int targetMostRightNodeWorldPosX = targetGroup.MostRightNode.Position.x;
                        NodeNav startRightNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX);
                        if (startRightNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startRightNode;
                            transition.EndNode = targetGroup.MostRightNode;
                            transition.TransitionType = NodeTransitionType.JustJumpUp;
                            transitions.Add(transition);
                        }
                    }
                    else
                    {
                        int targetMostLeftNodeWorldPosX = targetGroup.MostLeftNode.Position.x;
                        NodeNav startLeftNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX - 2);
                        if (startLeftNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startLeftNode;
                            transition.EndNode = targetGroup.MostLeftNode;
                            transition.TransitionType = NodeTransitionType.MovingJump;
                            CalcJumpCastBlocked(transition);
                            transitions.Add(transition);
                        }
                        else
                        {
                            startLeftNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX - 1);
                            if (startLeftNode != null)
                            {
                                NodeTransition transition = new NodeTransition();
                                transition.StartNode = startLeftNode;
                                transition.EndNode = targetGroup.MostLeftNode;
                                transition.TransitionType = NodeTransitionType.JumpAndMove;
                                transitions.Add(transition);
                            }
                        }


                        int targetMostRightNodeWorldPosX = targetGroup.MostRightNode.Position.x;
                        NodeNav startRightNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX + 2);
                        if (startRightNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startRightNode;
                            transition.EndNode = targetGroup.MostRightNode;
                            transition.TransitionType = NodeTransitionType.MovingJump;
                            CalcJumpCastBlocked(transition);
                            transitions.Add(transition);
                        }
                        else
                        {
                            startRightNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX + 1);
                            if (startRightNode != null)
                            {
                                NodeTransition transition = new NodeTransition();
                                transition.StartNode = startRightNode;
                                transition.EndNode = targetGroup.MostRightNode;
                                transition.TransitionType = NodeTransitionType.JumpAndMove;
                                transitions.Add(transition);
                            }
                        }
                    }
                }
                else
                {
                    if (IsThinPlatform)
                    {
                        int targetMostLeftNodeWorldPosX = targetGroup.MostLeftNode.Position.x;
                        NodeNav startLeftNode = GetNodeAtWorldPosX(targetMostLeftNodeWorldPosX);
                        if (startLeftNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startLeftNode;
                            transition.EndNode = targetGroup.MostLeftNode;
                            transition.TransitionType = NodeTransitionType.DropDown;
                            transitions.Add(transition);
                        }

                        int targetMostRightNodeWorldPosX = targetGroup.MostRightNode.Position.x;
                        NodeNav startRightNode = GetNodeAtWorldPosX(targetMostRightNodeWorldPosX);
                        if (startRightNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = startRightNode;
                            transition.EndNode = targetGroup.MostRightNode;
                            transition.TransitionType = NodeTransitionType.DropDown;
                            transitions.Add(transition);
                        }
                    }
                }
            }
            // case 6: 타겟안에 내가 포함된 경우
            else if (targetRect.xMin <= myRect.xMin && myRect.xMax <= targetRect.xMax)
            {
                if (myRect.center.y > targetRect.center.y) // 타겟지형이 아래쪽일 경우 좌우로 떨어지는 경우만 처리
                {
                    if (IsThinPlatform)
                    {
                        int myMostLeftNodeWorldPosX = MostLeftNode.Position.x;
                        NodeNav endLeftNode = targetGroup.GetNodeAtWorldPosX(myMostLeftNodeWorldPosX);
                        if (endLeftNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostLeftNode;
                            transition.EndNode = endLeftNode;
                            transition.TransitionType = NodeTransitionType.DropDown;
                            transitions.Add(transition);
                        }

                        int myMostRightNodeWorldPosX = MostRightNode.Position.x;
                        NodeNav endRightNode = targetGroup.GetNodeAtWorldPosX(myMostRightNodeWorldPosX);
                        if (endRightNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostRightNode;
                            transition.EndNode = endRightNode;
                            transition.TransitionType = NodeTransitionType.DropDown;
                            transitions.Add(transition);
                        }
                    }
                    else
                    {
                        int myMostLeftNodeWorldPosX = MostLeftNode.Position.x;
                        NodeNav endLeftNode = targetGroup.GetNodeAtWorldPosX(myMostLeftNodeWorldPosX - 1);
                        if (endLeftNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostLeftNode;
                            transition.EndNode = endLeftNode;
                            transition.TransitionType = NodeTransitionType.WalkAndFall;
                            transitions.Add(transition);
                        }

                        int myMostRightNodeWorldPosX = MostRightNode.Position.x;
                        NodeNav endRightNode = targetGroup.GetNodeAtWorldPosX(myMostRightNodeWorldPosX + 1);
                        if (endRightNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostRightNode;
                            transition.EndNode = endRightNode;
                            transition.TransitionType = NodeTransitionType.WalkAndFall;
                            transitions.Add(transition);
                        }
                    }
                }
                else
                {
                    if (targetGroup.IsThinPlatform)
                    {
                        int myMostLeftNodeWorldPosX = MostLeftNode.Position.x;
                        NodeNav endLeftNode = targetGroup.GetNodeAtWorldPosX(myMostLeftNodeWorldPosX);
                        if (endLeftNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostLeftNode;
                            transition.EndNode = endLeftNode;
                            transition.TransitionType = NodeTransitionType.JustJumpUp;
                            transitions.Add(transition);
                        }

                        int myMostRightNodeWorldPosX = MostRightNode.Position.x;
                        NodeNav endRightNode = targetGroup.GetNodeAtWorldPosX(myMostRightNodeWorldPosX);
                        if (endRightNode != null)
                        {
                            NodeTransition transition = new NodeTransition();
                            transition.StartNode = MostRightNode;
                            transition.EndNode = endRightNode;
                            transition.TransitionType = NodeTransitionType.JustJumpUp;
                            transitions.Add(transition);
                        }
                    }
                }
            }
            else
            {
                LOG.log("플랫폼 라우팅 초기화 경고 : 예상치 못한 지형 케이스 발생");
                LOG.log(myRect);
                LOG.log(targetRect);
            }
            return transitions.Count > 0;
        }

        public Rect GetRect()
        {
            Rect rect = new Rect();
            rect.min = GroundNodes[0].MinPos;
            rect.max = GroundNodes[GroundNodes.Count - 1].MaxPos;
            return rect;
        }
        public RectInt GetRectInt()
        {
            RectInt rect = new RectInt();
            rect.min = GroundNodes[0].MinPosInt;
            rect.max = GroundNodes[GroundNodes.Count - 1].MaxPosInt;
            return rect;
        }

        public NodeNav GetNodeAtIndex(int index)
        {
            if (index < 0 || index >= GroundNodes.Count)
                return null;
            return GroundNodes[index];
        }

        public NodeNav GetNodeAtWorldPosX(int worldPosX)
        {
            int localIndex = worldPosX - MostLeftNode.Position.x;
            return GetNodeAtIndex(localIndex);
        }

        void CalcJumpCastBlocked(NodeTransition transition)
        {
            if (transition.TransitionType != NodeTransitionType.MovingJump)
                return;

            // float moveSpeed = 7;
            // 나중에 시작위치에서 도착위치까지의 최소 이동속도, 점프레벨 계산 후 jumpcast미리 해봐서 가능여부 미리 해두는 방식으로 변경 예정..
            // 지금은 일단 JumpCast는 모두 무시하고 전부 가능하다고 판단(임시)
            for (int jumpLevel = 1; jumpLevel <= 4; jumpLevel++)
            {
                transition.IsBlockedJumping[jumpLevel - 1] = false;

                // RaycastHit2D hitInfo = JumpSimulationTable.JumpCast(transition.StartNode.CenterTopPos, moveSpeed, jumpLevel);
                // if (hitInfo.collider != null)
                // {
                //     Vector2 nodeInnerPos = hitInfo.point - (hitInfo.normal * 0.1f);
                //     Rect targetNodeGroupArea = transition.EndNode.ParentGroup.GetRect();
                //     transition.IsBlockedJumping[jumpLevel - 1] = !targetNodeGroupArea.Contains(nodeInnerPos);
                // }
            }
        }

    }
}