/*
 * Name: Wesley Ho
 * ID: 2382489
 * Email: weho@chapman.edu
 * CPSC 236-02
 * Assignment: Final Project - Hide and Seek (and Tag)
 * 
 * This file is NOT my own work. It is authored by Sebastian Lague
 * as a part of his A* pathfinding tutorial:
 * https://github.com/SebLague/Pathfinding/blob/master/Episode%2004%20-%20heap/Assets/Scripts/Heap.cs
 * The video: https://www.youtube.com/watch?v=3Dw5d7PlcTM&ab_channel=SebastianLague
 * There are slight modifications to meet the CPSC 236 style guide.
 * Used as a MinHeap ADT since C# does not have a standard implementation
 * with permission from Professor Prate.
 */

using UnityEngine;
using System.Collections;
using System;

public class MinHeap<T> where T : IMinHeapItem<T>
{
	private T[] items;
	private int currentItemCount;

	public MinHeap(int maxMinHeapSize)
	{
		items = new T[maxMinHeapSize];
	}

	public void Add(T item)
	{
		item.MinHeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}

	public T RemoveFirst()
	{
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].MinHeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item)
	{
		SortUp(item);
	}

	public int Count
	{
		get
		{
			return currentItemCount;
		}
	}

	public bool Contains(T item)
	{
		return Equals(items[item.MinHeapIndex], item);
	}

	void SortDown(T item)
	{
		while (true)
		{
			int childIndexLeft = item.MinHeapIndex * 2 + 1;
			int childIndexRight = item.MinHeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount)
			{
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount)
				{
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
					{
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0)
				{
					Swap(item, items[swapIndex]);
				}
				else
				{
					return;
				}

			}
			else
			{
				return;
			}

		}
	}

	void SortUp(T item)
	{
		int parentIndex = (item.MinHeapIndex - 1) / 2;

		while (true)
		{
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0)
			{
				Swap(item, parentItem);
			}
			else
			{
				break;
			}

			parentIndex = (item.MinHeapIndex - 1) / 2;
		}
	}

	void Swap(T itemA, T itemB)
	{
		items[itemA.MinHeapIndex] = itemB;
		items[itemB.MinHeapIndex] = itemA;
		int itemAIndex = itemA.MinHeapIndex;
		itemA.MinHeapIndex = itemB.MinHeapIndex;
		itemB.MinHeapIndex = itemAIndex;
	}
}

public interface IMinHeapItem<T> : IComparable<T>
{
	int MinHeapIndex
	{
		get;
		set;
	}
}