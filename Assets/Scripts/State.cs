using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// 可行的操作
/// </summary>
public enum Operation
{
    UP,
    LEFT,
    DOWN,
    RIGHT
}

public static class Util
{
    /// <summary>
    /// 操作对应的位置变化
    /// </summary>
    public static Dictionary<Operation, Vector2Int> Delta = new Dictionary<Operation, Vector2Int>
    {
        { Operation.UP, new Vector2Int(0, 1) },
        { Operation.LEFT, new Vector2Int(-1, 0) },
        { Operation.DOWN, new Vector2Int(0, -1) },
        { Operation.RIGHT, new Vector2Int(1, 0) }
    };
}

/// <summary>
/// 数码问题的一个状态
/// </summary>
public class State
{
    public int length
    {
        get { return radius * radius; }
    }
    public int radius { get; private set; }
    public int[,] digits { get; private set; }

    public Vector2Int zeroPos { get; private set; }

    public State(int length, int[] ints)
    {
        radius = (int)Math.Sqrt(length);
        digits = new int[radius, radius];
        // Array.Copy(ints, digits, length);
        int cnt = 0;
        for (int i = 0; i < radius; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                digits[i, j] = ints[cnt++];
                if (digits[i, j] == 0)
                    zeroPos = new Vector2Int(i, j);
            }
        }
    }

    public State(State s)
    {
        radius = s.radius;
        digits = new int[radius, radius];
        Array.Copy(s.digits, digits, s.digits.Length);
        zeroPos = s.zeroPos;
    }

    public override int GetHashCode()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var d in this.digits)
        {
            stringBuilder.Append(d.ToString());
        }

        return stringBuilder.ToString().GetHashCode();
    }

    public bool Equals(State other)
    {
        return this.digits.Equals(other.digits);
    }

    /// <summary>
    /// 位置是否合法
    /// </summary>
    private bool CheckBorder(Vector2Int pos)
    {
        return 0 <= pos.x && pos.x < radius && 0 <= pos.y && pos.y < radius;
    }

    /// <summary>
    /// 能否进行操作
    /// </summary>
    public bool CanDoOperation(Operation op)
    {
        Vector2Int newZeroPos = zeroPos + Util.Delta[op];
        if (!CheckBorder(newZeroPos)) return false;
        return true;
    }

    /// <summary>
    /// 进行操作（保证操作合法）
    /// </summary>
    public void DoOperation(Operation op)
    {
        Vector2Int newZeroPos = zeroPos + Util.Delta[op];
        (digits[newZeroPos.x, newZeroPos.y], digits[zeroPos.x, zeroPos.y]) =
            (digits[zeroPos.x, zeroPos.y], digits[newZeroPos.x, newZeroPos.y]);
        zeroPos = newZeroPos;
    }
}