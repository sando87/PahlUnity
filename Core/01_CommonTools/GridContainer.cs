using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public enum Direction { DownLeft, Down, DownRight, Left, Center, Right, UpLeft, Up, UpRight };

    public class GridContainer<T>
    {
        private readonly Dictionary<Vector2Int, T> mGridContainer = null;

        // List보다 배열이 더 적합
        // 방향은 고정 데이터이므로 readonly static 추천
        private static readonly Vector2Int[] mDirectionList =
        {
            new Vector2Int(-1, -1), // DownLeft
            new Vector2Int(0, -1),  // Down
            new Vector2Int(1, -1),  // DownRight
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(0, 0),   // Center
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 1),  // UpLeft
            new Vector2Int(0, 1),   // Up
            new Vector2Int(1, 1),   // UpRight
        };

        public float GridSize { get; private set; }
        public Vector2 WorldBase { get; private set; }

        public int Count => mGridContainer.Count;


        public GridContainer(float gridSize, Vector2 worldBase, int capacity = 0)
        {
            GridSize = Mathf.Max(0.0001f, gridSize);
            WorldBase = worldBase;

            if (capacity > 0)
                mGridContainer = new Dictionary<Vector2Int, T>(capacity);
            else
                mGridContainer = new Dictionary<Vector2Int, T>();
        }

        public Vector2Int DirectToVector2Int(Direction dir)
        {
            return mDirectionList[(int)dir];
        }

        public Direction Vector2IntToDirect(Vector2Int vector)
        {
            Vector2Int off = vector - new Vector2Int(-1, -1);
            int idx = off.x + (off.y * 3);

            if (idx < 0 || idx >= mDirectionList.Length)
                throw new Exception($"Invalid direction vector : {vector}");

            return (Direction)idx;
        }

        public T Get(Vector2Int index)
        {
            if (mGridContainer.TryGetValue(index, out T info))
                return info;

            return default;
        }

        public bool TryGet(Vector2Int index, out T value)
        {
            return mGridContainer.TryGetValue(index, out value);
        }

        public void Set(Vector2Int index, T info)
        {
            mGridContainer[index] = info;
        }

        public bool Contains(Vector2Int index)
        {
            return mGridContainer.ContainsKey(index);
        }

        public void Remove(Vector2Int index)
        {
            mGridContainer.Remove(index);
        }

        public void Clear()
        {
            mGridContainer.Clear();
        }

        public Vector2Int ToGridIndex(Vector2 worldPos)
        {
            Vector2 local = worldPos - WorldBase;

            return new Vector2Int(
                Mathf.FloorToInt(local.x / GridSize),
                Mathf.FloorToInt(local.y / GridSize));
        }

        public Vector2 ToWorldGridCenter(Vector2Int index)
        {
            return WorldBase + new Vector2(
                (index.x + 0.5f) * GridSize,
                (index.y + 0.5f) * GridSize);
        }

        public void GetGrids(Rect area, List<Vector2Int> rets)
        {
            rets.Clear();

            int minX = Mathf.FloorToInt((area.xMin - WorldBase.x) / GridSize);
            int minY = Mathf.FloorToInt((area.yMin - WorldBase.y) / GridSize);

            int maxX = Mathf.FloorToInt((area.xMax - WorldBase.x) / GridSize);
            int maxY = Mathf.FloorToInt((area.yMax - WorldBase.y) / GridSize);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    rets.Add(new Vector2Int(x, y));
                }
            }
        }

        public IEnumerable<Vector2Int> GetGridsEnum(Rect area)
        {
            int minX = Mathf.FloorToInt((area.xMin - WorldBase.x) / GridSize);
            int minY = Mathf.FloorToInt((area.yMin - WorldBase.y) / GridSize);

            int maxX = Mathf.FloorToInt((area.xMax - WorldBase.x) / GridSize);
            int maxY = Mathf.FloorToInt((area.yMax - WorldBase.y) / GridSize);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        public void GetNeighbors4Way(Vector2Int center, List<Vector2Int> rets)
        {
            rets.Clear();

            rets.Add(center + mDirectionList[(int)Direction.Down]);
            rets.Add(center + mDirectionList[(int)Direction.Left]);
            rets.Add(center + mDirectionList[(int)Direction.Right]);
            rets.Add(center + mDirectionList[(int)Direction.Up]);
        }

        public IEnumerable<Vector2Int> GetNeighbors4WayEnum(Vector2Int center)
        {
            yield return center + mDirectionList[(int)Direction.Down];
            yield return center + mDirectionList[(int)Direction.Left];
            yield return center + mDirectionList[(int)Direction.Right];
            yield return center + mDirectionList[(int)Direction.Up];
        }

        public void GetNeighbors8Way(Vector2Int center, List<Vector2Int> rets)
        {
            rets.Clear();

            for (int i = 0; i < mDirectionList.Length; i++)
            {
                if (i == (int)Direction.Center)
                    continue;

                rets.Add(center + mDirectionList[i]);
            }
        }

        public IEnumerable<Vector2Int> GetNeighbors8WayEnum(Vector2Int center)
        {
            for (int i = 0; i < mDirectionList.Length; i++)
            {
                if (i == (int)Direction.Center)
                    continue;

                yield return center + mDirectionList[i];
            }
        }
    }
}