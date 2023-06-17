using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructures
{
    public class MaxPQ<T> : IEnumerable<T>
    {
        public readonly List<T> _pq;
        private int Size => _pq.Count - 1;
        private readonly IComparer<T> _comparer;

        public MaxPQ()
        {
            _pq = new List<T>(new T[1]);
            _comparer = null;
        }

        public MaxPQ(IComparer<T> comparer)
        {
            _pq = new List<T>(new T[1]);
            this._comparer = comparer;
        }

        public bool IsEmpty()
        {
            return Size <= 0;
        }

        public void Insert(T item)
        {
            _pq.Add(item);
            Swim(Size);
        }

        public T Max()
        {
            if (IsEmpty())
            {
                throw new Exception("Priority queue is empty");
            }
            return _pq[1];
        }

        public T DelMax()
        {
            if (IsEmpty())
            {
                throw new Exception("Priority queue is empty");
            }
            T max = _pq[1];
            Exchange(1, Size);
            _pq.RemoveAt(Size);
            Sink(1);
            return max;
        }

        private void Swim(int i)
        {
            while (i > 1 && Less(i / 2, i))
            {
                Exchange(i, i / 2);
                i /= 2;
            }
        }

        private void Sink(int i)
        {
            while (2 * i <= Size)
            {
                int j = 2 * i;
                if (j < Size && Less(j, j + 1))
                {
                    j++;
                }
                if (!Less(i, j))
                {
                    break;
                }
                Exchange(i, j);
                i = j;
            }
        }

        private bool Less(int i, int j)
        {
            if (_comparer == null)
            {
                return ((IComparable) _pq[i]).CompareTo(_pq[j]) < 0;
            }
            return _comparer.Compare(_pq[i], _pq[j]) < 0;
        }

        private void Exchange(int i, int j)
        {
            (_pq[j], _pq[i]) = (_pq[i], _pq[j]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new HeapIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class HeapIterator : IEnumerator<T>
        {
            public T Current => copy.DelMax();
            object IEnumerator.Current => Current;
            private readonly MaxPQ<T> copy;

            public HeapIterator(MaxPQ<T> original)
            {
                copy = (original._comparer == null) ? new MaxPQ<T>() : new MaxPQ<T>(original._comparer);
                for (int i = 1; i <= original.Size; i++)
                {
                    copy.Insert(original._pq[i]);
                }
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                return !copy.IsEmpty();
            }

            public void Reset()
            {
                
            }
        }
    }
}
