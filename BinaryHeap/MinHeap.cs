using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BinaryHeap
{
    public class MinHeap
        <T> : IHeap<T> where T : IComparable
    {      
        public int Count { get; private set; }
        public int Capacity { get; private set; }

        private readonly IComparer<T> _comparer;
        T[] _minHeap;

        public MinHeap()
        {
            InitHeap();
            _comparer = null;
        }

        public MinHeap(IComparer<T> comparer)
        {
            InitHeap();           
            _comparer = comparer;
        }

        private void InitHeap()
        {
            Capacity = 4;
            _minHeap = new T[Capacity + 1];
            Count = 0;
        }

        public void Add(T key)
        {
            if (Count == Capacity) Resize(2 * Capacity);
            _minHeap[++Count] = key;
            Swim(Count);
           // Debug.Assert(IsMinHeap());
        }

        public T SeekMin()
        {
            return _minHeap[1];
        }

        public T PopMin()
        {
            T max = SeekMin();
            Swap(1, Count);
            _minHeap[Count--] = default;
            Sink(1);
            if (Count > 0 && Count == Capacity / 4)  Resize(Capacity / 2);
           // Debug.Assert(IsMinHeap());
            return max;
        }

        private void Resize(int length)
        {
            Capacity = length;
            T[] copy = new T[length+1];
            for(int i=1; i<=Count; i++)
            {
                copy[i] = _minHeap[i];
            }
            _minHeap = copy;
        }

        private void Swim(int node)
        {
            while(node > 1 && IsLess(node, Father(node)))
            {
                int father = Father(node);
                Swap(node, father);
                node = father;
            }
        }


        private void Sink(int node)
        {
            while(LeftSon(node) <= Count)
            {
                int leftSon = LeftSon(node);
                int rightSon = RightSon(node);
                int exchangedSon = leftSon;
                if (rightSon <= Count && IsLess(rightSon, leftSon)) exchangedSon = rightSon;
                if (IsLess(node, exchangedSon)) break;
                Swap(node, exchangedSon);               
                node = exchangedSon;
            }
        }

        private int RightSon(int nodeIndex)
        {
            return 2 * nodeIndex + 1;
        }

        private int LeftSon(int nodeIndex )
        {
            return 2 * nodeIndex;
        }

        private int Father(int nodeIndex)
        {
            return nodeIndex / 2;
        }

        private void Swap(int thisIndex, int thatIndex)
        {
            T aux = _minHeap[thisIndex];
            _minHeap[thisIndex] = _minHeap[thatIndex];
            _minHeap[thatIndex] = aux;
        }

        private bool IsLess(int thisIndex, int thatIndex)
        {
            if(_comparer is null) return _minHeap[thisIndex].CompareTo(_minHeap[thatIndex]) < 0;
            return _comparer.Compare(_minHeap[thisIndex], _minHeap[thatIndex]) < 0;
        }
        private bool IsGreater(int thisIndex, int thatIndex)
        {
            if (_comparer is null) return _minHeap[thisIndex].CompareTo(_minHeap[thatIndex]) > 0;
            return _comparer.Compare(_minHeap[thisIndex], _minHeap[thatIndex]) > 0;
        }
        private bool IsMinHeap()
        {
            return IsMinHeap(1);
        }

        private bool IsMinHeap(int k)
        {
            if(k > Count) return true;
            var leftSon = LeftSon(k);
            var rightSon = RightSon(k);

            if (leftSon <= Count && IsGreater(k, leftSon)) return false;
            if (rightSon <= Count && IsGreater(k, rightSon)) return false;

            return IsMinHeap(leftSon) && IsMinHeap(rightSon);
        }
    }
}
