using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Technical_Solution
{
    internal class AStar
    {
        public static List<Point> FindPath(Dictionary<Point, List<Point>> graph, Point start, Point goal)
        {
            HashSet<Point> nodesToExplore = new HashSet<Point> { start };
            HashSet<Point> exploredNodes = new HashSet<Point>();
            Dictionary<Point, Point> previousNode = new Dictionary<Point, Point>();
            // Cost from start to current node
            Dictionary<Point, double> startToCurent = new Dictionary<Point, double>();
            // Estimated cost from start to goal through current node
            Dictionary<Point, double> startToGoal = new Dictionary<Point, double>();
            startToCurent[start] = 0;
            startToGoal[start] = Heuristic(start, goal);
            while (nodesToExplore.Count > 0)
            {
                Point current = nodesToExplore.First();
                double lowestStartToGoalCost = double.MaxValue;
                foreach (Point P in nodesToExplore)
                {
                    double costForThisPoint;
                    if (startToGoal.ContainsKey(P))
                    {
                        costForThisPoint = startToGoal[P];
                    }
                    else
                    {
                        costForThisPoint = double.MaxValue;
                    }
                    if (costForThisPoint < lowestStartToGoalCost)
                    {
                        lowestStartToGoalCost = costForThisPoint;
                        current = P;
                    }
                }
                if (current == goal)
                {
                    return PathReconstruction(previousNode, current);
                }
                nodesToExplore.Remove(current);
                exploredNodes.Add(current);
                foreach (Point neighbour in graph[current])
                {
                    if (exploredNodes.Contains(neighbour))
                    {
                        continue;
                    }
                    double throughCurrentCost = startToCurent[current] + Heuristic(current, neighbour);
                    if (!nodesToExplore.Contains(neighbour))
                    {
                        nodesToExplore.Add(neighbour);
                    }
                    else
                    {
                        double bestKnownCost;
                        if (startToCurent.ContainsKey(neighbour))
                        {
                            bestKnownCost = startToCurent[neighbour];
                        }
                        else
                        {
                            bestKnownCost = double.MaxValue;
                        }
                        if (throughCurrentCost >= bestKnownCost)
                        {
                            continue;
                        }
                    }
                    previousNode[neighbour] = current;
                    startToCurent[neighbour] = throughCurrentCost;
                    startToGoal[neighbour] = startToCurent[neighbour] + Heuristic(neighbour, goal);
                }
            }
            return null;    
        }

        private static double Heuristic(Point currentPoint, Point GoalPoint)
        {
            return Math.Sqrt(Math.Pow(currentPoint.X - GoalPoint.X, 2) + Math.Pow(currentPoint.Y - GoalPoint.Y, 2));
        }

        private static List<Point> PathReconstruction(Dictionary<Point, Point> previousNode, Point current)
        {
            List<Point> path = new List<Point> { current };
            while (previousNode.ContainsKey(current))
            {
                current = previousNode[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
