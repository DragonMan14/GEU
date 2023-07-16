using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Pathfinding
{
    public enum NodeType
    {
        Ground,
        Wall,
        Ceiling,
        Air,
        // The corner of a wall and the ceiling or ground
        Corner,
        Obstacle
    }

    public class Node
    {
        public List<Edge> Neighbors { get; private set; }
        public Vector2 Coordinates { get; private set; }
        public List<NodeType> NodeTypes { get; private set; }

        public Node(Vector2 Coordinates, float distanceBetweenNodes)
        { 
            Neighbors = new List<Edge>();
            this.Coordinates = Coordinates;
            NodeTypes = InitializeNodeTypes(distanceBetweenNodes);
        }

        private List<NodeType> InitializeNodeTypes(float distanceBetweenNodes)
        {
            LayerMask environment = LayerMask.GetMask("Environment");
            List<NodeType> nodeTypes = new List<NodeType>();
            // Raycast from the coordinates of this node
            RaycastHit2D obstacleRay = Physics2D.Raycast(Coordinates, Vector2.down, distanceBetweenNodes, environment);
            // If the distance of the ray is 0, and the raycast hit something
            if (obstacleRay.distance == 0 && obstacleRay.collider != null)
            {
                // Add obstacle to the list and immediately return
                nodeTypes.Add(NodeType.Obstacle);
                return nodeTypes;
            }
            // Otherwise, the node is not in an obstacle, and it is in the air
            else
            {
                nodeTypes.Add(NodeType.Air);
            }
            // Raycast in all four cardinal and check for the other node types
            RaycastHit2D groundRay = Physics2D.Raycast(Coordinates, Vector2.down, distanceBetweenNodes, environment);
            // If the ground ray hit something add NodeType.Ground
            if (groundRay.collider != null)
            {
                nodeTypes.Add(NodeType.Ground);
            }

            RaycastHit2D rightWallRay = Physics2D.Raycast(Coordinates, Vector2.right, distanceBetweenNodes, environment);
            RaycastHit2D leftwallRay = Physics2D.Raycast(Coordinates, Vector2.left, distanceBetweenNodes, environment);
            // If either of the wall rays hit something, add NodeType.Wall
            if (rightWallRay.collider != null || leftwallRay.collider != null)
            {
                nodeTypes.Add(NodeType.Wall);
            }

            RaycastHit2D ceilingRay = Physics2D.Raycast(Coordinates, Vector2.up, distanceBetweenNodes, environment);
            // If the ceiling ray hits something, add NodeType.Ceiling
            if (ceilingRay.collider != null)
            {
                nodeTypes.Add(NodeType.Ceiling);
            }

            // Check if the node is a corner or not
            // If so far, the node only contains NodeType.Air, it has a possiblity to be a corner
            bool onlyAir = nodeTypes.Contains(NodeType.Air) && nodeTypes.Count == 1;
            if (onlyAir)
            {
                float distance = (float) Math.Sqrt(2 * Math.Pow(distanceBetweenNodes, 2));
                // Raycast in the diagonal directions
                // If one of those raycasts hit something, then the node is a corner
                for (int i = 0; i < 4; i++)
                {
                    Vector2 direction = i switch
                    {
                        0 => new Vector2(-1f, -1f),
                        1 => new Vector2(-1f, 1f),
                        2 => new Vector2(1f, -1f),
                        _ => new Vector2(1f, 1f),
                    };
                    RaycastHit2D ray = Physics2D.Raycast(Coordinates, direction, distance, environment);
                    if (ray.collider != null)
                    {
                        nodeTypes.Add(NodeType.Corner);
                    }
                }
            }
            
            return nodeTypes;
        }

        #region Neighbors 

        public void AddNeighbor(Node node)
        {
            Neighbors.Add(new Edge(this, node));
        }

        public void RemoveNeighbor(Node node)
        {
            Edge edge = Neighbors.Find(e => e.ContainsNode(node));
            while (edge != null)
            {
                Neighbors.Remove(edge);
                edge = Neighbors.Find(e => e.ContainsNode(node));
            }
        }

        public void ConnectNode(Node node)
        {
            this.AddNeighbor(node);
            node.AddNeighbor(this);
        }

        public void DisconnectNode(Node node)
        {
            this.RemoveNeighbor(node);
            node.RemoveNeighbor(this);
        }

        public bool IsConnectedWith(Node node)
        {
            Edge edge = Neighbors.Find(e => e.ContainsNode(node));
            return edge != null;
        }

        #endregion

        public float GetManhattanDistanceTo(Node node)
        {
            return Mathf.Abs(this.Coordinates.x - node.Coordinates.x) + Mathf.Abs(this.Coordinates.y - node.Coordinates.y);
        }

        public bool ContainsNodeType(NodeType type)
        {
            return NodeTypes.Contains(type);
        }

        public bool ContainsNodeType(List<NodeType> validNodeTypes)
        {
            foreach (NodeType type in NodeTypes)
            {
                if (validNodeTypes.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
