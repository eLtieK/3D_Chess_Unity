    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class AiManager : MonoBehaviour
    {
        public bool is_ai;
        public const int ai_minimax_depth = 5;
        public const int ai_quiescene_depth = 3;
        public static AiManager Instance;
        private Chessman[,] copyBoard;
        private List<int[,]> arrayPosition;
        private void Start() {
            Instance = this;
            is_ai = false;
            CreatePositionArray();
            copyBoard = BoardManager.Instance.CopyChessBoard(BoardManager.Instance.Chessmans);
        }
        public void SetLocalOrBot(bool flag) {
            if(flag)
                is_ai = true;
            else
                is_ai = false;
        }
        private int Evaluate(Chessman[,] board) {
            int score = 0;
            for(int i = 0; i < 8; i++) {
                for(int j = 0; j < 8; j++) {
                    if(board[i,j] == null) {    
                        continue;
                    }

                    if (board[i, j].is_white) 
                        score -= PieceValue(board[i, j]) + PositionValue(board[i, j], i, j) + CalculatePawnAdvancement(board[i, j], i, j) + CalculatePieceMobilityAndThreads(board, board[i, j], i, j);
                    else 
                        score += PieceValue(board[i, j]) + PositionValue(board[i, j], i, j) + CalculatePawnAdvancement(board[i, j], i, j) + CalculatePieceMobilityAndThreads(board, board[i, j], i, j);
                }
            }
        
            return score;
        }
        #region Value
        private void CreatePositionArray() {
            arrayPosition = new List<int[,]>();
            for (int i = 0; i < 6; i++) {
                arrayPosition.Add(GetPoistionArray(i));
            }
        }
        private int[,] GetPoistionArray(Chessman c) {
            if(c is King)
                return arrayPosition[0];
            else if(c is Queen) 
                return arrayPosition[1];
            else if(c is Rook)
                return arrayPosition[2];
            else if(c is Bishop)
                return arrayPosition[3];
            else if(c is Knight)
                return arrayPosition[4];
            else
                return arrayPosition[5];
        }
        private int[,] GetPoistionArray(int i) {
            if(i == 0) { //King
                return new int[8, 8] {
                    { 20, 30, 10, 0, 0, 10, 30, 20 },
                    { 20, 20, 0, 0, 0, 0, 20, 20 },
                    { -10, -20, -20, -20, -20, -20, -20, -10 },
                    { -20, -30, -30, -40, -40, -30, -30, -20 },
                    { -30, -40, -40, -50, -50, -40, -40, -30 },
                    { -30, -40, -40, -50, -50, -40, -40, -30 },
                    { -30, -40, -40, -50, -50, -40, -40, -30 },
                    { -30, -40, -40, -50, -50, -40, -40, -30 },
                };
            } else if(i == 1) { //Queen
                return new int[8, 8] {
                    { -20, -10, -10, -5, -5, -10, -10, -20 },
                    { -10, 0, 0, 0, 0, 0, 0, -10 },
                    { -10, 0, 5, 5, 5, 5, 0, -10 },
                    { 0, 0, 5, 5, 5, 5, 0, 0 },
                    { -5, 0, 5, 5, 5, 5, 0, -5 },
                    { -10, 0, 5, 5, 5, 5, 0, -10 },
                    { -10, 0, 0, 0, 0, 0, 0, -10 },
                    { -20, -10, -10, -5, -5, -10, -10, -20 },
                };
            } else if(i == 2) {//Rook
                return new int[8, 8] {
                    { 0, 0, 0, 5, 5, 0, 0, 0 },
                    { -5, 0, 0, 0, 0, 0, 0, -5 },   
                    { -5, 0, 0, 0, 0, 0, 0, -5 },
                    { -5, 0, 0, 0, 0, 0, 0, -5 },
                    { -5, 0, 0, 0, 0, 0, 0, -5 },
                    { -5, 0, 0, 0, 0, 0, 0, -5 },
                    { 5, 10, 10, 10, 10, 10, 10, 5 },
                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                };
            } else if(i == 3) {//Bishop
                return new int[8, 8] {
                    { -20, -10, -10, -10, -10, -10, -10, -20 },
                    { -10, 5, 0, 0, 0, 0, 5, -10 },   
                    { -10, 10, 10, 10, 10, 10, 10, -10 },
                    { -10, 0, 10, 10, 10, 10, 0, -10 },
                    { -10, 5, 5, 10, 10, 5, 5, -10 },
                    { -10, 0, 5, 10, 10, 5, 0, -10 },
                    { -10, 0, 0, 0, 0, 0, 0, -10 },
                    { -20, -10, -10, -10, -10, -10, -10, -20 },
                };
            } else if(i == 4) {//Knight
                return new int[8, 8] {
                    { -50, -40, -30, -30, -30, -30, -40, -50 },
                    { -40, -20, 0, 5, 5, 0, -20, -40 },   
                    { -30, 5, 10, 15, 15, 10, 5, -30 },
                    { -30, 0, 15, 20, 20, 15, 0, -30 },
                    { -30, 5, 15, 20, 20, 15, 5, -30 },
                    { -30, 0, 10, 15, 15, 10, 0, -30 },
                    { -40, -20, 0, 0, 0, 0, -20, -40 },
                    { -50, -40, -30, -30, -30, -30, -40, -50 },
                };
            } else {//Pawn
                return new int[8, 8] {
                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 5, 10, 10, -20, -20, 10, 10, 5 },   
                    { 5, -5, -10, 0, 0, -10, -5, 5 },
                    { 0, 0, 0, 20, 20, 0, 0, 0 },
                    { 5, 5, 10, 25, 25, 10, 5, 5 },
                    { 10, 10, 20, 30, 30, 20, 10, 10 },
                    { 50, 50, 50, 50, 50, 50, 50, 50 },
                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                };
            }
        }
        private int PositionValue(Chessman piece, int x, int y) {
            int[,] positionValues = GetPoistionArray(piece);
            return !piece.is_white ? positionValues[7 - y, x] : positionValues[y, x] ;
        }
        private int PieceValue(Chessman c) {
            if(c.GetType() == typeof(Pawn)) 
                return 100;
            else if(c.GetType() == typeof(Bishop))
                return 300;
            else if(c.GetType() == typeof(Knight))
                return 300;
            else if(c.GetType() == typeof(Rook))
                return 500;
            else if(c.GetType() == typeof(Queen))
                return 900;
            else if(c.GetType() == typeof(King))
                return 9000;
            else 
                return 0;
        }
        #endregion
        private int MiniMax(ref Chessman[,] board, int depth, int alpha, int beta, bool isMaximizingPlayer) {
            if (depth == 0) 
                return QuiescenceSearch(ai_quiescene_depth, alpha, beta, board, isMaximizingPlayer);
            
            if (isMaximizingPlayer) {
                int maxEval = int.MinValue;

                // Thực hiện move order ở đây
                List<Move> moves = GenerateMoves(board, isMaximizingPlayer, depth);
                moves.Sort((move1, move2) => move2.heuristic.CompareTo(move1.heuristic));  // Sắp xếp nước đi theo heuristic

                foreach(Move move in moves) {
                    VirtualMove virrtual_move = VirtualMoveChessman(ref move.chessman, move.toX, move.toY, ref board);
                    int eval = MiniMax(ref board, depth - 1, alpha, beta, !isMaximizingPlayer);
                    UndoMove(ref virrtual_move, ref board);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)  
                        return maxEval;
                }
                return maxEval;
            } else {
                int minEval = int.MaxValue;

                // Thực hiện move order ở đây
                List<Move> moves = GenerateMoves(board, isMaximizingPlayer, depth);
                moves.Sort((move1, move2) => move2.heuristic.CompareTo(move1.heuristic));  // Sắp xếp nước đi theo heuristic

                foreach(Move move in moves) {
                    VirtualMove virtual_move = VirtualMoveChessman(ref move.chessman, move.toX, move.toY, ref board);
                    int eval = MiniMax(ref board, depth - 1, alpha, beta, !isMaximizingPlayer);
                    UndoMove(ref virtual_move, ref board);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)  
                        return minEval;
                }
                return minEval;
            }
        }
        private Move BestMove(Chessman[,] board, int depth) {
            int highestSeenValue = int.MinValue;
            int currentValue; 
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            Move best_move = null;
            List<Move> moves = GenerateMoves(board, true, depth);
            moves.Sort((move1, move2) => move2.heuristic.CompareTo(move1.heuristic));  // Sắp xếp nước đi theo heuristic

            foreach(Move move in moves) {
                VirtualMove virtual_move = VirtualMoveChessman(ref move.chessman, move.toX, move.toY, ref board);
                currentValue = MiniMax(ref board, depth - 1, alpha, beta, false);
                UndoMove(ref virtual_move, ref board);
                if(currentValue >= highestSeenValue) {
                    highestSeenValue = currentValue;
                    best_move = move;
                } 
                alpha = Math.Max(alpha, currentValue);
                if(beta <= alpha)
                    return best_move;
            }
            return best_move;
        }

        public VirtualMove VirtualMoveChessman(ref Chessman chessman, int x, int y, ref Chessman[,] board) {
            if(chessman.PossibleMove()[x,y]) {
                Chessman capturedChessman = board[x, y];
                board[chessman.current_X, chessman.current_Y] = null;
                int start_x = chessman.current_X;
                int start_y = chessman.current_Y;
                chessman.SetPosition(x, y);
                board[x, y] = chessman;

                return new VirtualMove(chessman, start_x, start_y, x, y, capturedChessman);
            }
            return null;
        }

        public void UndoMove(ref VirtualMove move, ref Chessman[,] board) {
            if (move != null) {
                // Đưa quân cờ về vị trí ban đầu
                board[move.StartX, move.StartY] = move.MovedChessman;
                board[move.EndX, move.EndY] = move.CapturedChessman;  // Đưa lại quân cờ đã bị bắt về nếu có

                // Cập nhật lại tọa độ hiện tại của quân cờ
                move.MovedChessman.SetPosition(move.StartX, move.StartY);
                if(move.CapturedChessman != null) 
                    move.CapturedChessman.SetPosition(move.EndX, move.EndY);
            }
        }

        public IEnumerator AiTurn() {
            copyBoard = BoardManager.Instance.CopyChessBoard(BoardManager.Instance.Chessmans);
            yield return new WaitForSeconds(1f);
            Move move = BestMove(copyBoard, ai_minimax_depth);
            BoardManager.Instance.SelectChessman((int)move.chessman.transform.position.x, (int)move.chessman.transform.position.z);
            yield return new WaitForSeconds(1f);
            BoardHighlights.Instance.UpdateHighlight(move.toX, move.toY);
            yield return new WaitForSeconds(0.25f);
            BoardManager.Instance.MoveChessman(move.toX, move.toY);
            yield return new WaitForSeconds(0.5f);
        }
    #region Print
        public void PrintBoard(Chessman[,] board) {
        string boardString = "";

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                Chessman piece = board[i, j];
                if (piece == null) {
                    boardString += "[ . ] ";
                } else {
                    char pieceChar = GetPieceChar(piece);
                    boardString += $"[{pieceChar}] ";
                }
            }
            boardString += "\n";
        }

        Debug.Log(boardString);
    }

    private char GetPieceChar(Chessman piece) {
        if (piece is Pawn) return 'P';
        if (piece is Rook) return 'R';
        if (piece is Knight) return 'N';
        if (piece is Bishop) return 'B';
        if (piece is Queen) return 'Q';
        if (piece is King) return 'K';
        return ' ';
    }
    #endregion
    #region Move
    public class VirtualMove {
        public Chessman MovedChessman { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public Chessman CapturedChessman { get; set; }

        public VirtualMove(Chessman movedChessman, int startX, int startY, int endX, int endY, Chessman capturedChessman) {
            MovedChessman = movedChessman;
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            CapturedChessman = capturedChessman;
        }
    }
    private class Move {
        public Chessman chessman;
        public int toX;
        public int toY;
        public int heuristic;
        public Move() {

        }
        public Move(Chessman chessman, int toX, int toY, int heuristic) {
            this.chessman = chessman;
            this.toX = toX;
            this.toY = toY;
            this.heuristic = heuristic;
        }
    }

    private int CalculateHeuristic(Chessman chessman, int x, int y, Chessman[,] board) {
        int heuristic = 0;
        if (board[x, y] != null) {
            heuristic += PieceValue(board[x,y]);
        }
        heuristic += PositionValue(chessman, x, y); // Ưu tiên vị trí trung tâm

        return heuristic;
    }
    private List<Move> GenerateMoves(Chessman[,] board, bool isMaximizingPlayer, int depth) {
        List<Move> moves = new List<Move>();

        foreach (Chessman c in board) {
            if (c == null || c.is_white == isMaximizingPlayer)
                continue;

            bool[,] possibleMoves = c.PossibleMove();
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (possibleMoves[i, j]) {
                        int heuristicValue = CalculateHeuristic(c, i, j, board);
                        moves.Add(new Move(c, i, j, heuristicValue));
                    }
                }
            }
        }
        return moves;
    }
    private List<Move> GenerateCaptureMoves(Chessman[,] board, bool isMaximizingPlayer) {
        List<Move> captureMoves = new List<Move>();

        foreach (Chessman c in board) {
            if (c == null || c.is_white == isMaximizingPlayer)
                continue;

            bool[,] possibleMoves = c.PossibleMove();
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (possibleMoves[i, j] && board[i, j] != null) {
                        int heuristicValue = CalculateHeuristic(c, i, j, board);
                        captureMoves.Add(new Move(c, i, j, heuristicValue));
                    }
                }
            }
        }
        return captureMoves;
    }
    #endregion
    #region BetterAi
    private int QuiescenceSearch(int num, int alpha, int beta, Chessman[,] board, bool isMaximizingPlayer) {
        if(num == 0)
            return Evaluate(board);

        int maxEval = int.MinValue;
        int minEval = int.MaxValue;
        List<Move> captures = GenerateCaptureMoves(board, isMaximizingPlayer);
        if(captures.Count == 0)
            return Evaluate(board);
        captures.Sort((move1, move2) => move2.heuristic.CompareTo(move1.heuristic));
        foreach (Move move in captures) {
            VirtualMove virtual_move = VirtualMoveChessman(ref move.chessman, move.toX, move.toY, ref board);
            int eval = QuiescenceSearch(num - 1, alpha, beta, board, !isMaximizingPlayer);
            UndoMove(ref virtual_move, ref board);

            if(isMaximizingPlayer) {
                maxEval = Math.Max(eval, maxEval);
                alpha = Math.Max(eval, alpha);
                if (beta <= alpha)
                    return maxEval;
            } else {
                minEval = Math.Min(eval, minEval);
                beta = Math.Min(eval, beta);
                if (beta <= alpha)
                    return minEval;
            }
        }
        return isMaximizingPlayer ? maxEval : minEval;
    }
    #endregion
    #region Bonus
    private int CalculatePawnAdvancement(Chessman chessman, int x, int y) {
        int score = 0;
        if(chessman is Pawn) {
            int advancementScore = chessman.is_white ? y : 7 - y; // Tính điểm dựa trên vị trí của quân tốt
            score += advancementScore * 10;
        }
        return score;
    }

    private int CalculatePieceMobilityAndThreads(Chessman[,] board, Chessman chessman, int x, int y) {
        bool[,] possibleMoves = chessman.PossibleMove();
        int score = 0;
        int thread = 0;
        int mobility = 0;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (possibleMoves[i, j]) 
                    mobility++;
                if (possibleMoves[i, j] && board[i,j] != null)
                    thread++;
            }
        }
        score += (mobility + thread * 2) * ((chessman is Pawn || chessman is Bishop )? 8 : (chessman is Knight ? 5 : (chessman is Rook ? 4 : 3)));
        return score;
    }
    #endregion
}
