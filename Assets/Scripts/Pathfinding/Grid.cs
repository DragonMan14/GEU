using DataStructures;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Profiling.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        // Bounding boxes should all be continuously connected
        [SerializeField] private List<Rectangle> _boundingBoxes;
        [SerializeField] private float distanceBetweenNodes;

        [Header("Gizmos")]
        [SerializeField] private bool _drawBoundingBoxes = true;
        [SerializeField] private bool _drawNodes = true;
        [SerializeField] private bool _drawEdges = true;

        private List<Node> _nodes = new List<Node>();

        private Stack<Node> path;
        public Transform starting;
        public Transform ending;

        private void Awake()
        {
            _nodes = new List<Node>();
            InitializeGrid();
        }

        private void Update()
        {
                Node c1 = GetNodeClosestTo(starting.position);
                Node c2 = GetNodeClosestTo(ending.position);
                Pathfinder pathfinder = new Pathfinder();
                path = pathfinder.AStarPathfinding(c1, c2);
        }

        private bool CoordinatesAreInBound(Vector2 coordinates)
        {
            // Loop through each bounding box
            foreach (Rectangle boundingBox in _boundingBoxes)
            {
                // If one box contains the coordinates, return true 
                if (boundingBox.Contains(coordinates)) 
                {
                    return true;
                }
            }
            // Else false
            return false;
        }

        private Node GetNodeWithCoordinates(Vector2 coordinates)
        {
            return _nodes.Find(node => node.Coordinates.Equals(coordinates));
        }

        private void InitializeGrid()
        {
            if (_boundingBoxes.Count <= 0) 
            {
                throw new Exception("Bounding box not assigned");
            }
            if (distanceBetweenNodes <= 0)
            {
                throw new Exception("Distance between nodes must be a positive number");
            }
            // A queue to store nodes you still need to check the neighbors of
            Queue<Node> nodesToInitialize = new Queue<Node>();
            // A list of coordinates that you've already visited
            List<Vector2> AlreadyVisitedCoordinates = new List<Vector2>();
            // The starting node starts at the bottom right corner of the first bounding box
            Node startingNode = new Node(_boundingBoxes[0].BottomLeftCorner);
            // Add the starting node to the overall list of nodes, the nodes to initialize, and the already visited coordinates
            _nodes.Add(startingNode);
            AlreadyVisitedCoordinates.Add(startingNode.Coordinates);
            nodesToInitialize.Enqueue(startingNode);

            // While you haven't initialized all the nodes
            while(nodesToInitialize.Count > 0)
            {
                // Dequeue a node because you have now visited it, and store it in current node
                Node currentNode = nodesToInitialize.Dequeue();
                // Loop through all the adjacent coordinates at intervals of the distance between nodes
                for (int i = 0; i < 4; i++)
                {
                    var newCoordinates = i switch
                    {
                        0 => new Vector2(currentNode.Coordinates.x + distanceBetweenNodes, currentNode.Coordinates.y),// East neighbor
                        1 => new Vector2(currentNode.Coordinates.x - distanceBetweenNodes, currentNode.Coordinates.y),// West neighbor
                        2 => new Vector2(currentNode.Coordinates.x, currentNode.Coordinates.y + distanceBetweenNodes),// North neighbor
                        3 => new Vector2(currentNode.Coordinates.x, currentNode.Coordinates.y - distanceBetweenNodes),// South neighbor
                        _ => Vector2.negativeInfinity,
                    };
                    // If the neighbor has already been visited
                    if (AlreadyVisitedCoordinates.Contains(newCoordinates))
                    {
                        // Find the corresponding node
                        Node neighbor = GetNodeWithCoordinates(newCoordinates);
                        // If it's not already connected with the current node, connect it
                        if (!neighbor.IsConnectedWith(currentNode))
                        {
                            neighbor.ConnectNode(currentNode);
                        }
                        continue;
                    }
                    // Else if the coordinates are in bound of the grid
                    if (CoordinatesAreInBound(newCoordinates))
                    {
                        // Create a new node at the new coordinates
                        Node nextNode = new Node(newCoordinates);
                        // Connect the next node to the current node
                        nextNode.ConnectNode(currentNode);
                        // Add it to all the lists and queues
                        _nodes.Add(nextNode);
                        nodesToInitialize.Enqueue(nextNode);
                        AlreadyVisitedCoordinates.Add(newCoordinates);
                    }
                }
            }
        }

        private Node GetNodeClosestTo(Vector2 coordinates)
        {
            float minDistance = float.MaxValue;
            Node closestNode = null;
            foreach (Node node in _nodes)
            {
                float distanceToCoordinates = Vector2.Distance(coordinates, node.Coordinates);
                if (distanceToCoordinates < minDistance)
                {
                    closestNode = node;
                    minDistance = distanceToCoordinates;
                }
            }
            return closestNode;
        }

        public static float CostFrom(Node node1, Node node2)
        {
            return 1;
        }

        public void OnDrawGizmos()
        {
            // Draw bounding boxes
            if (_drawBoundingBoxes)
            {
                Gizmos.color = Color.cyan;
                foreach (Rectangle rect in _boundingBoxes)
                {
                    Gizmos.DrawWireCube(rect.GetCenter(), rect.Dimensions);
                }
            }
            
            // Draw nodes
            if (_drawNodes)
            {
                foreach (Node node in _nodes)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(node.Coordinates, 0.2f);

                    // Draw edges
                    if (_drawEdges)
                    {
                        Gizmos.color = Color.green;
                        foreach (Node node2 in node.Neighbors)
                        {
                            Gizmos.DrawLine(node.Coordinates, node2.Coordinates);
                        }
                    }
                }
            }

            if (path != null)
            {
                Gizmos.color = Color.green;
                Stack<Node> copy = new Stack<Node>( new Stack<Node>(path) );
                Node current = copy.Pop();
                foreach(Node next in path)
                {
                    Gizmos.DrawLine(current.Coordinates, next.Coordinates);
                    current = next;
                }
            }
        }

        public void OnValidate()
        {
            // Ensures that in the inspector, the coordinates of the bottom left corner cannot exceed the top right corner
            foreach (Rectangle rect in _boundingBoxes)
            {
                float minX = rect.BottomLeftCorner.x;
                float minY = rect.BottomLeftCorner.y;
                float maxX = rect.TopRightCorner.x;
                float maxY = rect.TopRightCorner.y;

                minX = Mathf.Min(minX, maxX);
                minY = Mathf.Min(minY, maxY);
                maxX = Mathf.Max(minX, maxX);
                maxY = Mathf.Max(minY, maxY);

                rect.BottomLeftCorner = new Vector2(minX, minY);
                rect.TopRightCorner = new Vector2(maxX, maxY);
            }
        }
    }
}
