using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;
using System.Data;


namespace PahlUnity
{
    public class PlayerDepthManager : MonoBehaviour
    {
        const int DepthWidthRange = 20;
        const int DepthHeightRange = 12;

        public static int FrameCounter = 0;

        // 방향 벡터 (상하좌우)
        Vector2Int[] mDirections =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        [SerializeField] Tilemap _Tilemap = null;

        BaseObject mPlayer = null;
        ObjectBody2D mPlayerBody = null;
        Health mPlayerHealth = null;
        PlayerController2D mPlayerController = null;

        // BFS용 큐
        Queue<Vector2Int> mQueue = new Queue<Vector2Int>();

        // 방문 체크 (depth 기록 여부)
        Dictionary<Vector2Int, int> mVisited = new Dictionary<Vector2Int, int>();

        Dictionary<Vector2Int, PlayerDepthInfo> mPlayerDepthInfo = new Dictionary<Vector2Int, PlayerDepthInfo>();

        public void SetPlayer(BaseObject player)
        {
            FrameCounter = 0;
            mPlayer = player;
            mPlayerBody = mPlayer.GetComp<ObjectBody2D>();
            mPlayerHealth = mPlayer.GetComp<Health>();
            mPlayerController = mPlayer.GetComp<PlayerController2D>();
        }

        void Update()
        {
            if (mPlayer != null && !mPlayerHealth.IsDead && mPlayerController.IsGrounded)
            {
                FrameCounter++;
                UpdatePlayerDepth(mPlayerBody.Center);
            }
        }

        public void UpdatePlayerDepth(Vector2 playerPosition)
        {
            if (_Tilemap == null)
                return;

            // 월드 → 그리드 좌표
            Vector3Int playerCell3D = _Tilemap.WorldToCell(playerPosition);
            Vector2Int playerCell = new Vector2Int(playerCell3D.x, playerCell3D.y);

            mQueue.Clear();
            mVisited.Clear();

            // 시작점
            mQueue.Enqueue(playerCell);
            mVisited[playerCell] = 0;

            {
                // 적 이동 분포를 고르게 하기 위해 플레이어 위치 기준 세로칸이 모두 0 depth로 시작하게 함.
                // 그래야 적들이 플레이어를 따라올때 퍼져서 오기 때문..
                // Vector2Int upPos = playerCell + new Vector2Int(0, 1);
                // while (!IsBlocked(upPos) && Mathf.Abs(upPos.y - playerCell.y) < DepthHeightRange)
                // {
                //     mQueue.Enqueue(upPos);
                //     mVisited[upPos] = 0;
                //     upPos.y++;
                // }

                // Vector2Int downPos = playerCell + new Vector2Int(0, -1);
                // while (!IsBlocked(downPos) && Mathf.Abs(downPos.y - playerCell.y) < DepthHeightRange)
                // {
                //     mQueue.Enqueue(downPos);
                //     mVisited[downPos] = 0;
                //     downPos.y--;
                // }
            }

            while (mQueue.Count > 0)
            {
                Vector2Int current = mQueue.Dequeue();
                int currentDepth = mVisited[current];

                // 범위 제한
                int dx = Mathf.Abs(current.x - playerCell.x);
                int dy = Mathf.Abs(current.y - playerCell.y);

                if (dx > DepthWidthRange || dy > DepthHeightRange)
                    continue;

                // 막힌 칸은 기록도, 확산도 안 함
                if (IsBlocked(current))
                    continue;

                // Depth 정보 업데이트
                if (!mPlayerDepthInfo.TryGetValue(current, out PlayerDepthInfo info))
                {
                    info = new PlayerDepthInfo(current);
                    mPlayerDepthInfo.Add(current, info);
                }
                info.UpdateDepth(currentDepth, FrameCounter);

                // 이웃 탐색
                foreach (var dir in mDirections)
                {
                    Vector2Int next = current + dir;

                    if (mVisited.ContainsKey(next))
                        continue;

                    mVisited[next] = currentDepth + 1;
                    mQueue.Enqueue(next);
                }
            }
        }

        bool IsBlocked(Vector2Int pos)
        {
            Vector3Int pos3D = new Vector3Int(pos.x, pos.y, 0);
            return _Tilemap.HasTile(pos3D);
        }

        public PlayerDepthInfo GetPlayerDepthInfoAtPos(Vector2Int pos)
        {
            if (mPlayerDepthInfo.TryGetValue(pos, out PlayerDepthInfo info))
            {
                return info;
            }
            return null;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // DebugDrawDepthInfo();
        }
#endif

        void DebugDrawDepthInfo()
        {
            Camera mainCam = Camera.main;
            foreach (var kvp in mPlayerDepthInfo)
            {
                Vector2Int pos = kvp.Key;
                PlayerDepthInfo info = kvp.Value;

                Vector2 centerPos = pos + new Vector2(0.5f, 0.5f);
                Vector3 screenPos = mainCam.WorldToScreenPoint(centerPos);
                if (screenPos.z > 0)
                {
                    UnityEditor.Handles.Label(centerPos, info.GetWeight().ExFloorToInt().ToString());
                }
            }
        }

    }

    public class PlayerDepthInfo
    {
        const int MaxDepth = 30;

        public Vector2Int Position { get; private set; }
        float mDirtyTime = 0f;
        int mDepth = 0;
        int mFrameCounter = 0;
        public float ElapsedTime => Time.time - mDirtyTime;
        public bool IsNew { get => mFrameCounter == PlayerDepthManager.FrameCounter; }

        public PlayerDepthInfo(Vector2Int position)
        {
            Position = position;
        }

        public void UpdateDepth(int depth, int frameCounter)
        {
            mDepth = depth;
            mFrameCounter = frameCounter;
            mDepth.ExSetMaximum(MaxDepth);
            mDirtyTime = Time.time;
        }
        public float GetWeight()
        {
            if (IsNew)
            {
                return mDepth;
            }
            else
            {
                return MaxDepth + (ElapsedTime * 10f);
            }
        }
    }
}