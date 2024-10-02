using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman
{
    public override bool[,] PossibleMove() {
        bool[,] r = new bool[8,8];
        Chessman c;
        int i;

        //Right
        i = current_X;
        while(true) {
            i++;
            if(i >= 8) {
                break;
            }
            c = BoardManager.Instance.Chessmans[i, current_Y];
            if(c == null) {
                r[i,current_Y] = true;
            } else {
                if(c.is_white != is_white) {
                    r[i,current_Y] = true;
                }
                break;
            }
        }

        //Left
        i = current_X;
        while(true) {
            i--;
            if(i < 0) {
                break;
            }
            c = BoardManager.Instance.Chessmans[i, current_Y];
            if(c == null) {
                r[i,current_Y] = true;
            } else {
                if(c.is_white != is_white) {
                    r[i,current_Y] = true;
                }
                break;
            }
        }

        //Up
        i = current_Y;
        while(true) {
            i++;
            if(i >= 8) {
                break;
            }
            c = BoardManager.Instance.Chessmans[current_X, i];
            if(c == null) {
                r[current_X, i] = true;
            } else {
                if(c.is_white != is_white) {
                    r[current_X, i] = true;
                }
                break;
            }
        }

        //Down
        i = current_Y;
        while(true) {
            i--;
            if(i < 0) {
                break;
            }
            c = BoardManager.Instance.Chessmans[current_X, i];
            if(c == null) {
                r[current_X, i] = true;
            } else {
                if(c.is_white != is_white) {
                    r[current_X, i] = true;
                }
                break;
            }
        }
        return r;
    }
}
