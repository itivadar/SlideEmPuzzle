using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryHeap
{
    public interface IHeap<T> where T:IComparable
    {
        //capacity of the heap before resizing
        int Capacity { get;}

        //count of the elements in the heap
        int Count { get;}
        
        //add a new key to the heap
        void Add(T key);

        //returns the top element in the heap
        T SeekMin();

        //deletes the top element in the heap and returns it
        T PopMin();

    }
}
