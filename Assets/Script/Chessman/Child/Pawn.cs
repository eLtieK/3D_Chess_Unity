using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : Chessman
{
    public override bool[,] PossibleMove() {
        bool[,] r = new bool[8,8];
        Chessman c, c2; 
        int[] e = BoardManager.Instance.EnPassantMove;

        //White move
        if (is_white) {
            //Diagonal left
            if(current_X != 0 && current_Y != 7) {
                if(e[0] == current_X - 1 && e[1] == current_Y + 1) {
                    r[current_X - 1,current_Y + 1] = true;
                }

                c = BoardManager.Instance.Chessmans[current_X - 1, current_Y + 1];
                if(c != null && !c.is_white) {
                    r[current_X - 1,current_Y + 1] = true;
                } 
            }
            //Diaognal right
            if(current_X != 7 && current_Y != 7) {
                if(e[0] == current_X + 1 && e[1] == current_Y + 1) {
                    r[current_X + 1,current_Y + 1] = true;
                }

                c = BoardManager.Instance.Chessmans[current_X + 1, current_Y + 1];
                if(c != null && !c.is_white) {
                    r[current_X + 1,current_Y + 1] = true;
                } 
            }
            //Middle
            if(current_Y != 7) {
                c = BoardManager.Instance.Chessmans[current_X, current_Y + 1];
                if(c == null) {
                    r[current_X, current_Y + 1] = true;
                }
            }
            //Middle on first move
            if(current_Y == 1) {
                c = BoardManager.Instance.Chessmans[current_X, current_Y + 1];
                c2 = BoardManager.Instance.Chessmans[current_X, current_Y + 2];
                if(c == null && c2 == null) {
                    r[current_X, current_Y + 2] = true;
                }
            }
        }
        else {
            //Diagonal left
            if(current_X != 0 && current_Y != 0) {
                if(e[0] == current_X - 1 && e[1] == current_Y - 1) {
                    r[current_X - 1,current_Y - 1] = true;
                }

                c = BoardManager.Instance.Chessmans[current_X - 1, current_Y - 1];
                if(c != null && c.is_white) {
                    r[current_X - 1,current_Y - 1] = true;
                } 
            }
            //Diaognal right
            if(current_X != 7 && current_Y != 0) {
                if(e[0] == current_X + 1 && e[1] == current_Y - 1) {
                    r[current_X + 1,current_Y - 1] = true;
                }

                c = BoardManager.Instance.Chessmans[current_X + 1, current_Y - 1];
                if(c != null && c.is_white) {
                    r[current_X + 1,current_Y - 1] = true;
                } 
            }
            //Middle
            if(current_Y != 0) {
                c = BoardManager.Instance.Chessmans[current_X, current_Y - 1];
                if(c == null) {
                    r[current_X, current_Y - 1] = true;
                }
            }
            //Middle on first move
            if(current_Y == 6) {
                c = BoardManager.Instance.Chessmans[current_X, current_Y - 1];
                c2 = BoardManager.Instance.Chessmans[current_X, current_Y - 2];
                if(c == null && c2 == null) {
                    r[current_X, current_Y - 2] = true;
                }
            }
        }
        return r;
    }
}
