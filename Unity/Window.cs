using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using Utilities;

public class Window<T>
{
    private T[] array;
    private int nextIndex = 0;

    public int CurrentSize { get; private set; }

    public Window(int size)
    {
        array = new T[size];
    }

    public void Add(T value)
    {
        array[nextIndex] = value;
        nextIndex++;
        nextIndex = NumUtils.Mod(nextIndex, array.Length);

        if (CurrentSize < array.Length)
        {
            CurrentSize++;
        }
    }

    public IEnumerable<T> GetValues()
    {
        int startIndex;
        if (CurrentSize == array.Length)
        {
            startIndex = nextIndex;
            startIndex = NumUtils.Mod(startIndex, array.Length);
        }
        else
        {
            startIndex = 0;
        }

        for (int i = 0; i < CurrentSize; i++)
        {
            yield return array[startIndex];
            startIndex++;
            startIndex = NumUtils.Mod(startIndex, array.Length);
        }

        yield break;
    }
}
