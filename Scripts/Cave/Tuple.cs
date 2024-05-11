using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<T,K>
{
    public T Left;
    public K Right;

    public Tuple(T Left, K Right) {
        this.Left = Left;
        this.Right = Right;
    }
}
