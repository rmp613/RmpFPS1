using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RmpFPS1.Pathfinding
{
    public class Queue<T>
    {
        protected IComparer<T> comparer;
        protected List<T> queue = new List<T>();

        public Queue()
        {
            comparer = Comparer<T>.Default;
        }
        public Queue(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }
        protected int Compare(int i, int j)
        {
            return comparer.Compare(queue[i], queue[j]);
        }
        protected void SwapItems(int i, int j)
        {
            T item = queue[i];
            queue[i] = queue[j];
            queue[j] = item;
        }
        public int Push(T item)
        {
            int i = queue.Count, i2;
            queue.Add(item);
            do
            {
                if (i == 0)
                {
                    break;
                }
                i2 = (i - 1) / 2;

                if (Compare(i, i2) < 0)
                {
                    SwapItems(i, i2);
                    i = i2;
                }
                else
                {
                    break;
                }
            } while (true);
            return i;
        }
        public T Pop()
        {
            T answer = queue[0];
            int i = 0, i1, i2, i3;
            queue[0] = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);
            do
            {
                i3 = i;
                i1 = 2 * i + 1;
                i2 = 2 * i + 2;
                if (queue.Count > i1 && Compare(i, i1) > 0) //i1 < i
                    i = i1;
                if (queue.Count > i2 && Compare(i, i2) > 0) //i2 < i
                    i = i2;
                if (i == i3)
                    break;
                SwapItems(i, i3);
            } while (true);
            return answer;
        }
        public void Update(int item)
        {
            int i = item, i3;
            int i1, i2;
            do
            {
                if (i == 0)
                    break;
                i2 = (i - 1) / 2;
                if (Compare(i, i2) < 0)
                {
                    SwapItems(i, i2);
                    i = i2;
                }
                else
                    break;
            } while (true);
            if (i < item)
                return;
            do
            {
                i3 = i;
                i1 = 2 * i + 1;
                i2 = 2 * i + 2;
                if (queue.Count > i1 && Compare(i, i1) > 0) //i1 < i
                    i = i1;
                if (queue.Count > i2 && Compare(i, i2) > 0) //i2 < i
                    i = i2;
                if (i == i3)
                    break;
                SwapItems(i, i3);
            } while (true);
        }
        public T Smallest()
        {
            if (queue.Count > 0)
                return queue[0];
            return default(T);
        }
        public void Clear()
        {
            queue.Clear();
        }
        public int Count
        {
            get { return queue.Count; }
        }
        public void RemoveItem(T item)
        {
            int index = -1;
            for (int i = 0; i < queue.Count; i++)
            {
                if (comparer.Compare(queue[i], item) == 0)
                {
                    index = i;
                }
                if (index != -1)
                {
                    queue.RemoveAt(index);
                }
            }
        }
        public T this[int index]
        {
            get { return queue[index]; }
            set
            {
                queue[index] = value;
                Update(index);
            }
        }
    }
}