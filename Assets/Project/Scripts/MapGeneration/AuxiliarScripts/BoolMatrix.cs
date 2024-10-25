using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoolMatrix
{
    public bool[] Data;
    public int Cols;
    public int Rows;

    public BoolMatrix(int Cols, int Rows)
    {
        this.Cols = Cols;
        this.Rows = Rows;
        Data = new bool[Cols * Rows];
    }

    public bool GetValue(int col, int row)
    {
        return Data[row * Cols + col];
    }

    public void SetValue(int col, int row, bool value)
    {
        Data[row * Cols + col] = value;
    }

    public int GetLength(int dimension)
    {
        return (dimension == 0) ? Cols : Rows;
    }
}
