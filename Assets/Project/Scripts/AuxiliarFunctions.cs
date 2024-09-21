using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AuxiliarFunctions
{
    public static BoolMatrix CopyBoolMatrix(BoolMatrix original)
    {
        int cols = original.GetLength(0);
        int rows = original.GetLength(1);
        BoolMatrix copy = new BoolMatrix(cols, rows);

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                copy.SetValue(x, y, original.GetValue(x, y));
            }
        }

        return copy;
    }
}
