using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class State
{
    public int length;

    public int radius { get; private set; }
    public int[] digits { get; private set; }

    public State(int length, int[] ints)
    {
        this.length = length;
        digits = new int[length];
        radius = (int)Math.Sqrt(length);
        Array.Copy(ints, digits, length);
    }

    public State(State s)
    {
        this.length = s.length;
        this.digits = new int[this.length];
        radius = (int)Math.Sqrt(length);
        Array.Copy(s.digits, this.digits, this.length);
    }

    public override int GetHashCode()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(var d in this.digits)
        {
            stringBuilder.Append(d.ToString());
        }
        return stringBuilder.ToString().GetHashCode();
    }

    public bool Equals(State other)
    {
        return this.digits.Equals(other.digits);
    }

    public int IndexRow(int )

    public int IndexUp(int index)
    {
        return index - radius;
    }

    public int IndexDown(int index)
    {
        return index + radius;
    }

    public int IndexLeft(int index)
    {
        return index - 1;
    }

    public int IndexRight(int index)
    {
        return index + 1;
    }
}
