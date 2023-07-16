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
        public static Path AStarPathfinding(IPathfindingRestrictor restrictor, Node start, Node goal)
        {
            if (restrictor == null)
            {
                throw new ArgumentNullException("Restrictor is null");
            }
            if (start == null)
            {
                throw new ArgumentNullException("Starting node is null");
            }
            if (goal == null)
            {
                throw new ArgumentNullException("Goal node is null");
            }
            MinPQ<NodeWithPriority> frontier = new MinPQ<NodeWithPriority>(NodeWithPriority.ByPriorityOrder());
            frontier.Insert(new NodeWithPriority(start, 0));
            Dictionary<Node, Edge> cameFrom = new Dictionary<Node, Edge>();
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

                foreach (Edge next in current.Neighbors)
                {
                    if (!restrictor.IsValidPathAvailable(next))
                    {
                        continue;
                    }
                    float newCost = costSoFar[current] + Grid.CostFrom(current, next.GetOtherNode(current));
                    bool alreadyVisited = costSoFar.ContainsKey(next.GetOtherNode(current));
                    if (!alreadyVisited || newCost < costSoFar[next.GetOtherNode(current)])
                    {
                        costSoFar[next.GetOtherNode(current)] = newCost;
                        float priority = newCost + goal.GetManhattanDistanceTo(next.GetOtherNode(current));
                        frontier.Insert(new NodeWithPriority(next.GetOtherNode(current), priority));
                        cameFrom[next.GetOtherNode(current)] = next;
                    }
                }
            }

            if (!cameFrom.ContainsKey(goal))
            {
                Debug.Log(goal.Coordinates);
                return new Path(new Stack<Edge>());
            }

            Stack<Edge> path = new Stack<Edge>();
            {
                Node currentNode = goal;
                Edge currentEdge = cameFrom[goal];
                while (!currentEdge.ContainsNode(start))
                {
                    path.Push(currentEdge);
                    currentNode = currentEdge.GetOtherNode(currentNode);
                    currentEdge = cameFrom[currentNode];
                }
            }

            return new Path(path);
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
