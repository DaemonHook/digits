using System;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 最小堆
/// </summary>
public class MinHeap<T>
{
    //默认容量为6
    private List<T> mItems;
    private readonly IComparer<T> mComparer;

    // public int Count { get; private set; }
    public int Count => mItems.Count;

    public MinHeap(IComparer<T> comparer)
    {
        mComparer = comparer;
    }

    /// <summary>
    /// 增加元素到堆，并从后往前依次对各结点为根的子树进行筛选，使之成为堆，直到根结点
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Enqueue(T value)
    {
        mItems.Add(value);
        int position = BubbleUp(Count - 1);
        // Count++;
        return (position == 0);
    }

    /// <summary>
    /// 取出堆的最小值
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException();
        }

        var result = mItems[0];
        if (Count == 0) return result;
        //取序列最后的元素放在堆顶
        mItems[0] = mItems[Count - 1];
        mItems.RemoveAt(mItems.Count - 1);
        // 维护堆的结构
        BubbleDown();
        return result;
    }

    public void Clear()
    {
        mItems = new List<T>();
    }

    /// <summary>
    /// 从前往后依次对各结点为根的子树进行筛选，使之成为堆，直到序列最后的节点
    /// </summary>
    private void BubbleDown()
    {
        int parent = 0;
        int leftChild = parent * 2 + 1;
        while (leftChild < Count)
        {
            // 找到子节点中较小的那个
            int rightChild = leftChild + 1;
            int bestChild = (rightChild < Count && mComparer.Compare(mItems[rightChild], mItems[leftChild]) < 0)
                ? rightChild
                : leftChild;
            if (mComparer.Compare(mItems[bestChild], mItems[parent]) < 0)
            {
                // 如果子节点小于父节点, 交换子节点和父节点
                (mItems[parent], mItems[bestChild]) = (mItems[bestChild], mItems[parent]);
                parent = bestChild;
                leftChild = parent * 2 + 1;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 从后往前依次对各结点为根的子树进行筛选，使之成为堆，直到根结点
    /// </summary>
    private int BubbleUp(int startIndex)
    {
        while (startIndex > 0)
        {
            int parent = (startIndex - 1) / 2;
            //如果子节点小于父节点，交换子节点和父节点
            if (mComparer.Compare(mItems[startIndex], mItems[parent]) < 0)
            {
                (mItems[startIndex], mItems[parent]) = (mItems[parent], mItems[startIndex]);
            }
            else
            {
                break;
            }

            startIndex = parent;
        }

        return startIndex;
    }
}