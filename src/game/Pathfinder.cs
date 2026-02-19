using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Mathematics;

namespace fiveSeconds
{

    public class Pathfinder
    {
        private readonly Tile[][] map;
        private readonly int width;
        private readonly int height;

        private readonly int[,] gCost;
        private readonly int[,] parentX;
        private readonly int[,] parentY;
        private readonly bool[,] closed;
        private readonly bool[,] inOpen;

        private readonly MinHeap openHeap;

        public Pathfinder(Tile[][] map)
        {
            this.map = map;
            height = map.Length;
            width = map[0].Length;

            gCost = new int[width, height];
            parentX = new int[width, height];
            parentY = new int[width, height];
            closed = new bool[width, height];
            inOpen = new bool[width, height];

            openHeap = new MinHeap(width * height);
        }

        public List<Vector2i>? FindPath(Vector2i start, Vector2i goal)
        {
            ResetState();

            gCost[start.X, start.Y] = 0;
            parentX[start.X, start.Y] = start.X;
            parentY[start.X, start.Y] = start.Y;

            int h = Heuristic(start.X, start.Y, goal.X, goal.Y);

            openHeap.Push(start.X, start.Y, h);
            inOpen[start.X, start.Y] = true;

            while (openHeap.Count > 0)
            {
                var current = openHeap.Pop();
                int cx = current.X;
                int cy = current.Y;

                if (closed[cx, cy])
                    continue;

                if (cx == goal.X && cy == goal.Y)
                    return ReconstructPath(goal);

                closed[cx, cy] = true;

                ExploreNeighbor(cx + 1, cy, cx, cy, goal);
                ExploreNeighbor(cx - 1, cy, cx, cy, goal);
                ExploreNeighbor(cx, cy + 1, cx, cy, goal);
                ExploreNeighbor(cx, cy - 1, cx, cy, goal);
            }

            return null;
        }

        private void ExploreNeighbor(int nx, int ny, int px, int py, Vector2i goal)
        {
            if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                return;

            if (!map[ny][nx].Walkable)
                return;

            if (closed[nx, ny])
                return;

            int tentativeG = gCost[px, py] + 1;

            if (!inOpen[nx, ny] || tentativeG < gCost[nx, ny])
            {
                gCost[nx, ny] = tentativeG;
                parentX[nx, ny] = px;
                parentY[nx, ny] = py;

                int f = tentativeG + Heuristic(nx, ny, goal.X, goal.Y);

                openHeap.Push(nx, ny, f);
                inOpen[nx, ny] = true;
            }
        }

        private List<Vector2i> ReconstructPath(Vector2i goal)
        {
            var path = new List<Vector2i>();

            int x = goal.X;
            int y = goal.Y;

            while (true)
            {
                path.Add(new Vector2i(x, y));

                int px = parentX[x, y];
                int py = parentY[x, y];

                if (x == px && y == py)
                    break;

                x = px;
                y = py;
            }

            path.Reverse();
            return path;
        }

        private int Heuristic(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        private void ResetState()
        {
            Array.Clear(closed, 0, closed.Length);
            Array.Clear(inOpen, 0, inOpen.Length);
            openHeap.Clear();
        }

        public static void Test(Tile[][] map)
        {
            Pathfinder pathfinder = new Pathfinder(map);

            int iterations = 10_000;

            Vector2i start = (0, 0);
            Vector2i goal = (20, 20);

            // Warmup (JIT)
            pathfinder.FindPath(start, goal);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                pathfinder.FindPath(start, goal);
            }

            stopwatch.Stop();

            Console.WriteLine(
                $"Total: {stopwatch.Elapsed.TotalMilliseconds} ms\n" +
                $"Per call: {stopwatch.Elapsed.TotalMicroseconds / iterations} mcs");
        }
    }

    internal class MinHeap
    {
        private struct HeapNode
        {
            public int X;
            public int Y;
            public int F;
        }

        private readonly HeapNode[] heap;
        public int Count { get; private set; }

        public MinHeap(int capacity)
        {
            heap = new HeapNode[capacity];
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Push(int x, int y, int f)
        {
            int i = Count++;
            heap[i] = new HeapNode { X = x, Y = y, F = f };
            HeapifyUp(i);
        }

        public (int X, int Y) Pop()
        {
            var root = heap[0];
            heap[0] = heap[--Count];
            HeapifyDown(0);
            return (root.X, root.Y);
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (heap[i].F >= heap[parent].F)
                    break;

                Swap(i, parent);
                i = parent;
            }
        }

        private void HeapifyDown(int i)
        {
            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int smallest = i;

                if (left < Count && heap[left].F < heap[smallest].F)
                    smallest = left;

                if (right < Count && heap[right].F < heap[smallest].F)
                    smallest = right;

                if (smallest == i)
                    break;

                Swap(i, smallest);
                i = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            var tmp = heap[a];
            heap[a] = heap[b];
            heap[b] = tmp;
        }
    }

}