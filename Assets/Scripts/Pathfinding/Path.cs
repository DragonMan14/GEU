using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Pathfinding
{
    public class Path : IEnumerable<Edge>
    {
        private readonly Stack<Edge> Edges;
        public int Count => Edges.Count;

        public Path(Stack<Edge> Edges)
        {
            this.Edges = Edges ?? throw new System.ArgumentNullException("The stack of nodes is null");
            Debug.Log($"Beginning Length: {Edges.Count}");
        }
    
        public bool HasPath()
        {
            return Count > 0;
        }
        public Edge Peek()
        {
            return Edges.Peek();
        }

        public bool ContainsNode(Node node)
        {
            foreach(Edge edge in Edges)
            {
                if (edge.ContainsNode(node))
                {
                    return true;
                }
            }
            return false;
        }

        public Edge GetNextEdge()
        {
            if (!HasPath())
            {
                throw new System.Exception("The path does not exist, therefore there is no next node");
            }
            Debug.Log($"Check Length: {Edges.Count}");
            return Edges.Count > 1 ? Edges.Pop() : Edges.Peek();
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            return new PathEnumerator(Edges);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class PathEnumerator : IEnumerator<Edge>
        {
            public Edge Current => copy.Pop();
            object IEnumerator.Current => Current;
            private readonly Stack<Edge> copy;

            public PathEnumerator(Stack<Edge> original)
            {
                this.copy = new Stack<Edge>(new Stack<Edge>(original));
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