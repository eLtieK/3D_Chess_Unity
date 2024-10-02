using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
    public override bool[,] PossibleMove() {
        bool[,] r = new bool[8,8];
        
        //Upleft
        KnightMove(current_X - 1, current_Y + 2, ref r);

        //UpRight
        KnightMove(current_X + 1, current_Y + 2, ref r);

        //Downleft
        KnightMove(current_X - 1, current_Y - 2, ref r);

        //DownRight
        KnightMove(current_X + 1, current_Y - 2, ref r);

        //LeftUp
        KnightMove(current_X - 2, current_Y + 1, ref r);

        //LeftDown
        KnightMove(current_X - 2, current_Y - 1, ref r);

        //RightUp
        KnightMove(current_X + 2, current_Y + 1, ref r);

        //RightDown
        KnightMove(current_X + 2, current_Y - 1, ref r);
        return r; 
    }

    public void KnightMove(int x, int y, ref bool[,] r) {
        Chessman c;
        if(x >= 0 && x < 8 && y >= 0 && y < 8) {
            c = BoardManager.Instance.Chessmans[x,y];
            if(c == null) {
                r[x,y] = true;
            }
            else if(c.is_white != is_white) {
                r[x,y] = true;
            }
        }
    }
}
