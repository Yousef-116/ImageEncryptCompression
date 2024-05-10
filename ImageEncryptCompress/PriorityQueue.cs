using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    public class PriorityQueue<T>
    {
        private List<T> elements;
        private readonly IComparer<T> comparer;

        public PriorityQueue(IComparer<T> comparer)
        {
            this.elements = new List<T>();
            this.comparer = comparer;
        }

        // Function called count that returns elements.count
        public int Count => elements.Count;


        public void Enqueue(T item)
        {
            elements.Add(item);
            int index = Count - 1;

            while(index > 0)
            {
                int nodeIndex = (index - 1) / 2;
                // new element has higher priority
                if (comparer.Compare(elements[nodeIndex], elements[index]) <= 0)
                    break;
                // Swap two queue elements
                Swap(index, nodeIndex);
                index = nodeIndex;
            }
        }

        public T Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Queue is empty.");
            T front = elements[0];
            elements[0] = elements[Count - 1];
            elements.RemoveAt(Count - 1);
            int index = 0;
            while (true)
            {
                int leftChild = 2 * index + 1;
                if (leftChild >= Count)
                    break;
                int rightChild = leftChild + 1;
                int minChild = (rightChild < Count && comparer.Compare(elements[rightChild], elements[leftChild]) < 0)
                    ? rightChild
                    : leftChild;
                if (comparer.Compare(elements[index], elements[minChild]) <= 0)
                    break;
                Swap(index, minChild);
                index = minChild;
            }
            return front;
        }

        private void Swap(int i, int j)
        {
            T temp = elements[i];
            elements[i] = elements[j];
            elements[j] = temp;
        }
    }
}
