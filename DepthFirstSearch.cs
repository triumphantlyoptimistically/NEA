using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Solution
{
    internal class DepthFirstSearch
    {
        public static List<Point> FindPath(Dictionary<Point, List<Point>> graph, HashSet<Point> exploredNodes, Point node, Point goal)
        {
            if (exploredNodes.Contains(node))
            {
                return null;
            }
            exploredNodes.Add(node);
            if (node == goal)
            {
                return new List<Point> { node };
            }
            if (graph.ContainsKey(node))
            {
                foreach (Point neighbour in graph[node])
                {
                    List<Point> path = FindPath(graph, exploredNodes, neighbour, goal);
                    if (path != null)
                    {
                        path.Insert(0, node);
                        return path;
                    }
                }
            }
            return null;
        }
    }
}
