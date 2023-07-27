using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pathfinding
{
    public enum EdgeType
    {
        Walk,
        Climb,
        Fly,
        Jump,
        Drop,
        Obstacle
    }

    public class Edge
    {
        private readonly Node _node1;
        private readonly Node _node2;
        public readonly EdgeType EdgeType;

        public Edge(Node node1, Node node2) 
        {
            _node1 = node1;
            _node2 = node2;
            EdgeType = InitializeEdgeType();
        }

        private EdgeType InitializeEdgeType()
        {
            // If one of the nodes is an obstacle, the edge type is obstacle
            if (ContainsNodeType(NodeType.Obstacle))
            {
                return EdgeType.Obstacle;
            }
            // Connect ground nodes via a walking edge
            if (_node1.Coordinates.y == _node2.Coordinates.y && _node1.ContainsNodeType(NodeType.Ground) && _node2.ContainsNodeType(NodeType.Ground))
            {
                return EdgeType.Walk;
            }
            // Connect wall nodes via climbing edge
            if (_node1.Coordinates.x == _node2.Coordinates.x && _node1.ContainsNodeType(NodeType.Wall) && _node2.ContainsNodeType(NodeType.Wall))
            {
                return EdgeType.Climb;
            }
            // Connect ceiling nodes via climbing edge
            if (_node1.Coordinates.y == _node2.Coordinates.y && _node1.ContainsNodeType(NodeType.Ceiling) && _node2.ContainsNodeType(NodeType.Ceiling))
            {
                return EdgeType.Climb;
            }
            // If one of them is a corner and the other is a wall, and the corner has the same x axis as the wall, connect them via climbing edge
            if (_node1.Coordinates.x == _node2.Coordinates.x && ContainsNodeTypes(NodeType.Corner, NodeType.Wall))
            {
                return EdgeType.Climb;
            }
            // If one of them is a corner and the other is a ceiling, and the corner has the same y axis as the wall, connect them via climbing edge
            if (_node1.Coordinates.y == _node2.Coordinates.y && ContainsNodeTypes(NodeType.Corner, NodeType.Ceiling))
            {
                return EdgeType.Climb;
            }
            // If one of them is a corner and the other is a ground, and the corner has the same y axis as the ground, connect them via walking edge
            if (_node1.Coordinates.y == _node2.Coordinates.y && ContainsNodeTypes(NodeType.Corner, NodeType.Ground))
            {
                return EdgeType.Walk;
            }
            // If all of the previous options were not true, connect via flying edge
            return EdgeType.Fly;
        }

        public Node GetNode1()
        {
            return _node1;
        }

        public Node GetNode2()
        {
            return _node2;
        }

        public Node GetOtherNode(Node node)
        {
            if (node != _node1 && node != _node2)
            {
            //    throw new System.ArgumentException("Node has to be one of the nodes in the edge");
            }
            Node tempNode = node == _node1 ? _node2 : _node1;
            Debug.WriteLine($"Coordinates: {tempNode.Coordinates}");
            return tempNode;
        }

        public bool ContainsNode(Node node)
        {
            return _node1 == node || _node2 == node;
        }

        private bool ContainsNodeType(NodeType type)
        {
            return _node1.ContainsNodeType(type) || _node2.ContainsNodeType(type);
        }

        private bool ContainsNodeTypes(NodeType type1, NodeType type2) 
        {
            return _node1.ContainsNodeType(type1) && _node2.ContainsNodeType(type2) || _node1.ContainsNodeType(type2) && _node2.ContainsNodeType(type1);
        }
    }
}
