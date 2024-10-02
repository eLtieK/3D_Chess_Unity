using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman
{
    public override bool[,] PossibleMove() {
        bool[,] r = new bool [8,8];
        Chessman c;
        int i,j;

        //Top left
        i = current_X;
        j = current_Y;
        while(true) {
            i--;
            j++;
            if(i < 0 || j >= 8) {
                break;
            }
            c = BoardManager.Instance.Chessmans[i,j];
            if(c == null) {
                r[i,j] = true;
            }
            else {
                if(c.is_white != is_white) {
                    r[i,j] = true;
                }
                break;
            }
        }

        //Top right
        i = current_X;
        j = current_Y;
        while(true) {
            i++;
            j++;
            if(i >= 8 || j >= 8) {
                break;
            }
            c = BoardManager.Instance.Chessmans[i,j];
            if(c == null) {
                r[i,j] = true;
            }
            else {
                if(c.is_white != is_white) {
                    r[i,j] = true;
                }
                break;
            }
        }

        //Bottom left
        i = current_X;
        j = current_Y;
        while(true) {
            i--;
            j--;
            if(i < 0 || j < 0) {
                break;
            }
            c = BoardManager.Instance.Chessmans[i,j];
            if(c == null) {
                r[i,j] = true;
            }
            else {
                if(c.is_white != is_white) {
                    r[i,j] = true;
                }
                break;
            }
        }

        //Bottom right
        i = current_X;
        j = current_Y;
        while(true) {
            i++;
            j--;
            if(i >= 8 || j < 0) {
                break;
            }
            c = BoardManager.Instance.Chessmans[i,j];
            if(c == null) {
                r[i,j] = true;
            }
            else {
                if(c.is_white != is_white) {
                    r[i,j] = true;
                }
                break;
            }
        }
        return r;
    }
}
