using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Pathfinding
{
    public class Path : IEnumerable<Node>
    {
        private Stack<Node> Nodes;
        public int Count => Nodes.Count;

        public Path(Stack<Node> Nodes)
        {
            if (Nodes == null)
            {
                throw new System.ArgumentNullException("The stack of nodes is null");
            }
            this.Nodes = Nodes;
        }
    
        public bool HasPath()
        {
            return Count > 0;
        }
        public Node Peek()
        {
            return Nodes.Peek();
        }

        public bool Contains(Node node)
        {
            return Nodes.Contains(node);
        }

        public Node GetNextNode()
        {
            if (!HasPath())
            {
                throw new System.Exception("The path does not exist, therefore there is no next node");
            }
            return Nodes.Count > 1 ? Nodes.Pop() : Nodes.Peek();
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return new PathEnumerator(Nodes);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class PathEnumerator : IEnumerator<Node>
        {
            public Node Current => copy.Pop();
            object IEnumerator.Current => Current;
            private Stack<Node> copy;

            public PathEnumerator(Stack<Node> original)
            {
                this.copy = new Stack<Node>(new Stack<Node>(original));
            }  

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                return copy.Count > 0;
            }

            public void Reset()
            {

            }
        }
    }
}