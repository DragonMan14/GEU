using DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pathfinding
{
    public class Pathfinder
    {
        public Stack<Node> AStarPathfinding(IPathfindingRestrictor restrictor, Node start, Node goal)
        {
            MinPQ<NodeWithPriority> frontier = new MinPQ<NodeWithPriority>(NodeWithPriority.ByPriorityOrder());
            frontier.Insert(new NodeWithPriority(start, 0));
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
            cameFrom.Add(start, null);
            costSoFar.Add(start, 0);

            while (!frontier.IsEmpty())
            {
                Node current = frontier.DelMin().Node;

                if (current == goal)
                {
                    break;
                }

                foreach (Node next in current.Neighbors)
                {
                    if (!restrictor.IsValidPathAvailable(current, next))
                    {
                        continue;
                    }
                    float newCost = costSoFar[current] + Grid.CostFrom(current, next);
                    bool alreadyVisited = costSoFar.ContainsKey(next);
                    if (!alreadyVisited || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + goal.GetManhattanDistanceTo(next);
                        frontier.Insert(new NodeWithPriority(next, priority));
                        cameFrom[next] = current;
                    }
                }
            }

            if (!cameFrom.ContainsKey(goal))
            {
                return null;
            }

            Stack<Node> path = new Stack<Node>();
            {
                Node current = goal;
                while (current != start)
                {
                    path.Push(current);
                    current = cameFrom[current];
                }
            }

            return path;
        }

        private class NodeWithPriority : IComparable<NodeWithPriority>
        {
            public Node Node { get; private set; }
            public float Priority { get; private set; }

            public NodeWithPriority(Node node, float priority) 
            {
                this.Node = node;
                this.Priority = priority;
            }

            public int CompareTo(NodeWithPriority other)
            {
                return this.Priority.CompareTo(other.Priority);
            }

            public static IComparer<NodeWithPriority> ByPriorityOrder()
            {
                return new PriorityOrder();
            }

            private class PriorityOrder : IComparer<NodeWithPriority>
            {
                public int Compare(NodeWithPriority node1, NodeWithPriority node2)
                {
                    return node1.Priority.CompareTo(node2.Priority);
                }
            }

        }
    }
}
