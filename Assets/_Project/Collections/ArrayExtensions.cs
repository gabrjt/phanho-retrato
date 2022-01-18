using System;

public static class ArrayExtensions
{
    public static bool IsValidIndex(this Array array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}