using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static int GetIDPrefab(Chessman piece) {
        int id = 0;
        if(!piece.is_white)
            id += 6;

        if(piece is Queen)
            id += 1;
        else if(piece is Rook)
            id += 2;
        else if(piece is Bishop)
            id += 3;
        else if(piece is Knight)
            id += 4;
        else if(piece is Pawn)
            id += 5;
        return id;
    } 
    public static void SaveBoard() {
        string boardString = "";
        if(BoardManager.Instance.is_white_turn) 
            boardString += "white;";
        else
            boardString += "black;";
        Chessman[,] board = BoardManager.Instance.Chessmans;
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Chessman piece = board[x, y];
                if (piece != null)
                    boardString += GetIDPrefab(piece) + ",";
                else
                {
                    boardString += "null,";
                }
            }
        }
        if (boardString.Length > 0) boardString = boardString.TrimEnd(',');

        Chessman[,] white_out = BoardManager.Instance.white_out;
        boardString += ";";
        for(int i = 0; i < 2; i++) {
            for(int j = 0; j < 8; j++) {
                if(white_out[i,j] != null) 
                    boardString += GetIDPrefab(white_out[i,j]) + ",";
                else {
                    boardString += "null,";
                }
            }
        }
        if (boardString.Length > 0) boardString = boardString.TrimEnd(',');

        Chessman[,] black_out = BoardManager.Instance.black_out;
        boardString += ";";
        for(int i = 0; i < 2; i++) {
            for(int j = 0; j < 8; j++) {
                if(black_out[i,j] != null) 
                    boardString += GetIDPrefab(black_out[i,j]) + ",";
                else {
                    boardString += "null,";
                }
            }
        }
        if (boardString.Length > 0) boardString = boardString.TrimEnd(',');

        string key = "";
        if(AiManager.Instance.is_ai) 
            key = "bot";
        else 
            key = "local";
        PlayerPrefs.SetString(key, boardString);
        PlayerPrefs.Save();
        Debug.Log("Board saved to PlayerPrefs as string.");
    }
    public static void LoadBoard(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string boardString = PlayerPrefs.GetString(key);
            string[] board = boardString.Split(';');

            if(board[0] == "white")
                BoardManager.Instance.is_white_turn = true;
            else 
                BoardManager.Instance.is_white_turn = false;
            
            string[] pieceIDs = board[1].Split(',');

            int index = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    string pieceID = pieceIDs[index];
                    if (pieceID != "null" && !string.IsNullOrEmpty(pieceID))
                    {
                        int id = int.Parse(pieceID);
                        if(id > 5)
                            BoardManager.Instance.SpawnChessman(id, x, y, true);
                        else 
                            BoardManager.Instance.SpawnChessman(id, x, y, false);
                    }
                    index++;
                }
            }

            string[] white_out = board[2].Split(',');
            index = 0;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 7; y >= 0; y--)
                {
                    string pieceID = white_out[index];
                    if (pieceID != "null" && !string.IsNullOrEmpty(pieceID))
                    {
                        int id = int.Parse(pieceID);
                        if(id > 5)
                            BoardManager.Instance.SpawnChessmanOut(id, -2 - x, 7 - y, true);
                        else 
                            BoardManager.Instance.SpawnChessmanOut(id, -2 - x, 7 - y, false);
                    }
                    index++;
                }
            }

            string[] black_out = board[3].Split(',');
            index = 0;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 7; y >= 0; y--)
                {
                    string pieceID = black_out[index];
                    if (pieceID != "null" && !string.IsNullOrEmpty(pieceID))
                    {
                        int id = int.Parse(pieceID);
                        BoardManager.Instance.SpawnChessmanOut(id, 9 + x, y, true);
                    }
                    index++;
                }
            }
            Debug.Log("Board loaded from PlayerPrefs string.");
        }
        else
        {
            Debug.LogWarning("No saved board found in PlayerPrefs with key: " + key);
            BoardManager.Instance.SpawnAllChessman();
        }
    }
}
