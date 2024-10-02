using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    public override bool[,] PossibleMove() {
        bool[,] r = new bool[8,8];
        Chessman c;
        int i, j;

        //Top side
        i = current_X - 1;
        j = current_Y + 1;
        if(current_Y != 7) {
            for(int k = 0; k < 3; k++) {
                if(i >= 0 && i < 8) {
                    c = BoardManager.Instance.Chessmans[i,j];
                    if(c == null) {
                        r[i,j] = true;
                    } else if(c.is_white != is_white) {
                        r[i,j] = true;
                    }
                }
                i++;
            }
        }

        //Down side
        i = current_X - 1;
        j = current_Y - 1;
        if(current_Y != 0) {
            for(int k = 0; k < 3; k++) {
                if(i >= 0 && i < 8) {
                    c = BoardManager.Instance.Chessmans[i,j];
                    if(c == null) {
                        r[i,j] = true;
                    } else if(c.is_white != is_white) {
                        r[i,j] = true;
                    }
                }
                i++;
            }
        }

        //Middle left
        if(current_X != 0) {
            c = BoardManager.Instance.Chessmans[current_X - 1, current_Y];
            if(c == null) {
                r[current_X - 1, current_Y] = true;
            } else if(c.is_white != is_white) {
                r[current_X - 1, current_Y] = true;
            }
        }

        //Middle right
        if(current_X != 7) {
            c = BoardManager.Instance.Chessmans[current_X + 1, current_Y];
            if(c == null) {
                r[current_X + 1, current_Y] = true;
            } else if(c.is_white != is_white) {
                r[current_X + 1, current_Y] = true;
            }
        }
        Castling(ref r, BoardManager.Instance.Chessmans);
        return r;
    }

    private void Castling(ref bool[,] r, Chessman[,] c) {
        if(is_white && current_X == 4 && current_Y == 0 && BoardManager.Instance.Castling[0,3] != 1){goto pass;}
        if(!is_white && current_X == 4 && current_Y == 7 && BoardManager.Instance.Castling[1,3] != 1){goto pass;}
        return;
            
        pass:
        //Right
        for(int i = 1; i <= 3; i++) {
            if(i != 3) {
                if(c[current_X + i, current_Y] != null) {
                    break;
                }
            } else {
                if(c[current_X + i, current_Y] == null) {continue;}
                if(c[current_X + i, current_Y].GetType() == typeof(Rook)) {
                    r[current_X + 2, current_Y] = true;
                    BoardManager.Instance.Castling[0,0] = current_X + 3;
                    BoardManager.Instance.Castling[0,1] = current_Y;
                    BoardManager.Instance.Castling[0,2] = 1;
                }
            }
        }
        //Left
        for(int i = 1; i <= 4; i++) {
            if(i != 4) {
                if(c[current_X - i, current_Y] != null) {
                    break;
                }
            } else {
                if(c[current_X - i, current_Y] == null) {continue;}
                if(c[current_X - i, current_Y].GetType() == typeof(Rook)) {
                    r[current_X - 2, current_Y] = true;
                    BoardManager.Instance.Castling[1,0] = current_X - 4;
                    BoardManager.Instance.Castling[1,1] = current_Y;
                    BoardManager.Instance.Castling[1,2] = 1;
                }
            }
        }
    } 
}
 