using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public int current_X{set;get;}
    public int current_Y{set;get;}
    public bool is_white;

    public void SetPosition(int x, int y) {
        current_X = x;
        current_Y = y;
    }
    public virtual bool[,] PossibleMove() {
        return new bool[8,8];
    }
}
