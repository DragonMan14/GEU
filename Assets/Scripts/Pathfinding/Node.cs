using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public enum NodeType
    {
        Ground,
        Wall,
        Air,
        CornerOfWallAndGround
    }

    public class Node
    {
        public List<Node> Neighbors { get; private set; }
        public Vector2 Coordinates { get; private set; }

        public Node(Vector2 Coordinates)
        { 
            Neighbors = new List<Node>();
            this.Coordinates = Coordinates;
        }

        #region Neighbors 

        public void AddNeighbor(Node node)
        {
            Neighbors.Add(node);
        }

        public void RemoveNeighbor(Node node)
        {
            Neighbors.Remove(node);
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
            return Neighbors.Contains(node);
        }

        #endregion

        public float GetManhattanDistanceTo(Node node)
        {
            return Mathf.Abs(this.Coordinates.x - node.Coordinates.x) + Mathf.Abs(this.Coordinates.y - node.Coordinates.y);
        }
    }
}
