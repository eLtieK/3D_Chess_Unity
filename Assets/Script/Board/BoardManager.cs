using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
public class BoardManager : MonoBehaviour
{
    public bool pause;
    public static BoardManager Instance {set;get;} 
    private bool[,] AllowedMoves {set; get;}
    public Chessman[,] Chessmans {set;get;}
    public Chessman selected_Chessman;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selection_X = -1;
    private int selection_Y = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;
    private Material previous_mat;
    public Material selected_mat;

    public int[] EnPassantMove; /*{set;get;}*/ //bat tot qua duong
    public int[,] Castling {set;get;}
    private Quaternion orientation = Quaternion.Euler(0,180,0);
    public bool is_white_turn = true;
    // Start is called before the first frame update
    #region camera
    public CinemachineVirtualCamera[] cameras;
    #endregion
    void Start()
    {
        Instance = this;
        pause = true;
        white_out = new Chessman[2,8];
        black_out = new Chessman[2,8];
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8,8];
        EnPassantMove = new int[2]{-1,-1};
        Castling = new int[2,4]{{-1,-1,-1,-1},{-1,-1,-1,-1}};
        // 0-x, 1-y, 2-left or right, 3-haved move
        for(int i = 0; i < 2; i++) {
            for(int j = 0; j < 3; j++)
                Castling[i,j] = -1;
        }
    }
    public void Continue(string key) {
        foreach(GameObject go in activeChessman) {
            Destroy(go);
        }
        foreach(Chessman go in white_out) {
            if(go != null)
                Destroy(go.gameObject);
        }
        foreach(Chessman go in black_out) {
            if(go != null)
                Destroy(go.gameObject);
        }
        SaveManager.LoadBoard(key);
        SwitchCamera();
        pause = false;
        UiManager.Instance.HideAllUi();
        cameras[0].gameObject.SetActive(true);
    }
    // Update is called once per frame
    private void Update()
    {
        DrawChessBoard();
        UpdateSelection();
        DrawSelection();
        if(!is_white_turn && AiManager.Instance.is_ai) 
            return ;
        if(Input.GetMouseButtonDown(0) && !pause) {
            if(selection_X >= 0 && selection_Y >= 0 && selection_X <= 8 && selection_Y <= 8) {
                if (selected_Chessman == null) {
                    SelectChessman(selection_X, selection_Y);
                } else {
                    MoveChessman(selection_X, selection_Y);
                }
            }
        }
    }
    public void SelectChessman(int x, int y) {
        if(Chessmans[x,y] == null) {
            return;
        }
        if(Chessmans[x,y].is_white != is_white_turn) {
            return;
        }
        if(!activeChessman.Contains(Chessmans[x,y].gameObject)) {
            return;
        }

        bool hasAtleast_OneMove = false;
        AllowedMoves = Chessmans[x,y].PossibleMove();
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                if(AllowedMoves[i,j]) {
                    hasAtleast_OneMove = true;
                }
            }
        }
        if(!hasAtleast_OneMove) {
            return ;
        }
        
        selected_Chessman = Chessmans[x,y];
        selected_Chessman.transform.position = new Vector3(selected_Chessman.transform.position.x, 0.5f, selected_Chessman.transform.position.z);
        previous_mat = selected_Chessman.GetComponent<MeshRenderer>().material;
        if(selected_Chessman.is_white) {
            selected_mat.SetColor("_Color",new Color(223f / 255f, 210f / 255f, 194f / 255f));
        } else {
            selected_mat.SetColor("_Color", new Color(84f / 255f, 84f / 255f, 84f / 255f));
        }
        selected_mat.mainTexture = previous_mat.mainTexture;
        selected_Chessman.GetComponent<MeshRenderer>().material = selected_mat;
        BoardHighlights.Instance.HighlightAllowedMoves(AllowedMoves); //highlight cac o
    }
    public void MoveChessman(int x, int y) {
        if(AllowedMoves[x,y]) {
            Chessman c = Chessmans[x,y];
            if(c != null && c.is_white != is_white_turn) {
                //Capture chessman
                //If it is a king 
                if(c.GetType() == typeof(King)) {
                    AudioManager.Instance.PlaySFX("checkmate");
                    activeChessman.Remove(c.gameObject);
                    KickChessman(c.gameObject.GetComponent<Chessman>());
                    EndGame(x, y);
                    return ;
                }
                if(selected_Chessman.GetType()  == typeof(Pawn)) {
                    //Promotion
                    if(y == 7) {
                        activeChessman.Remove(selected_Chessman.gameObject);
                        Destroy(selected_Chessman.gameObject);
                        PromotionContainer.Instance.StartPromotion(x, y);
                    } else if(y == 0) {
                        activeChessman.Remove(selected_Chessman.gameObject);
                        Destroy(selected_Chessman.gameObject);
                        PromotionContainer.Instance.StartPromotion(x, y);
                    }
                }
                AudioManager.Instance.PlaySFX("capture");
                activeChessman.Remove(c.gameObject);
                KickChessman(c.gameObject.GetComponent<Chessman>());
                //Destroy(c.gameObject);
                EnPassantMove[0] = -1;
                EnPassantMove[1] = -1;
            } else {
                #region EnPassantMove
                if(x == EnPassantMove[0] && y == EnPassantMove[1] && selected_Chessman.GetType() == typeof(Pawn)) {
                    if(is_white_turn) { //White team
                        c = Chessmans[x, y - 1];
                    } else { //Black team
                        c = Chessmans[x, y + 1];
                    }
                    Chessmans[c.current_X, c.current_Y] = null;
                    activeChessman.Remove(c.gameObject);
                    //Destroy(c.gameObject);
                    KickChessman(c.gameObject.GetComponent<Chessman>());
                    AudioManager.Instance.PlaySFX("capture");
                    EnPassantMove[0] = -1;
                    EnPassantMove[1] = -1;
                    goto reset;
                }
                EnPassantMove[0] = -1;
                EnPassantMove[1] = -1;
                if(selected_Chessman.GetType() == typeof(Pawn)) {
                    if(selected_Chessman.current_Y == 1 && y == 3) {
                        EnPassantMove[0] = x;
                        EnPassantMove[1] = y - 1;
                    } else if(selected_Chessman.current_Y == 6 && y == 4) {
                        EnPassantMove[0] = x;
                        EnPassantMove[1] = y + 1;
                    } 
                    #endregion
                    AudioManager.Instance.PlaySFX("move");
                    #region Promotion
                    if(y == 7) {
                        activeChessman.Remove(selected_Chessman.gameObject);
                        Destroy(selected_Chessman.gameObject);
                        PromotionContainer.Instance.StartPromotion(x, y);
                        is_white_turn = !is_white_turn;
                        BoardHighlights.Instance.Hidehighlights();
                        return;
                    } else if(y == 0) {
                        activeChessman.Remove(selected_Chessman.gameObject);
                        Destroy(selected_Chessman.gameObject);
                        PromotionContainer.Instance.StartPromotion(x, y);
                        is_white_turn = !is_white_turn;
                        BoardHighlights.Instance.Hidehighlights();
                        return;
                    }
                    #endregion
                #region Castling
                } else if(selected_Chessman.GetType() == typeof(King)) {
                    if(Castling[0,3] == 1 && is_white_turn) {
                        AudioManager.Instance.PlaySFX("move");
                        goto reset;
                    } else if(Castling[1,3] == 1 && !is_white_turn) {
                        AudioManager.Instance.PlaySFX("move");
                        goto reset;
                    }
                    if(Castling[0,3] != 1 && is_white_turn) {
                        Castling[0,3] = 1;
                    } else if(Castling[1,3] != 1 && !is_white_turn) {
                        Castling[1,3] = 1;
                    }
                    if(Castling[0,2] == 1 && x == 6) {
                        AudioManager.Instance.PlaySFX("castling");
                        Chessman rook = Chessmans[Castling[0,0], Castling[0,1]];
                        Chessmans[rook.current_X, rook.current_Y] = null;
                        StartCoroutine(Smoothmove(rook, rook.transform.position, GetTileCenter(Castling[0,0] - 2, Castling[0,1]), 0.2f));
                        rook.SetPosition(Castling[0,0] - 2, Castling[0,1]);
                        Chessmans[Castling[0,0] - 2, Castling[0,1]] = rook;
                        Castling[0,2] = -1;
                    } else if(Castling[1,2] == 1 && x == 2) {
                        AudioManager.Instance.PlaySFX("castling");
                        Chessman rook = Chessmans[Castling[1,0], Castling[1,1]];
                        Chessmans[rook.current_X, rook.current_Y] = null;
                        StartCoroutine(Smoothmove(rook, rook.transform.position, GetTileCenter(Castling[1,0] + 3, Castling[1,1]), 0.2f));
                        rook.SetPosition(Castling[1,0] + 3, Castling[1,1]);
                        Chessmans[Castling[1,0] + 3, Castling[1,1]] = rook;
                        Castling[1,2] = -1;
                    } else {
                        AudioManager.Instance.PlaySFX("move");
                    }
                } else{
                    AudioManager.Instance.PlaySFX("move");
                }
            }
            #endregion
            reset:
            Chessmans[selected_Chessman.current_X, selected_Chessman.current_Y] = null;
            StartCoroutine(Smoothmove(selected_Chessman, selected_Chessman.transform.position, GetTileCenter(x, y), 0.2f));
            selected_Chessman.SetPosition(x, y);
            Chessmans[x, y] = selected_Chessman;
            is_white_turn = !is_white_turn;
        }
        BoardHighlights.Instance.Hidehighlights();
        if(selected_Chessman != null) {
            selected_Chessman.GetComponent<MeshRenderer>().material = previous_mat;
            selected_Chessman.transform.position = new Vector3(selected_Chessman.transform.position.x, 0, selected_Chessman.transform.position.z);
        }
        selected_Chessman = null;
        if(IsCheckMate()) {
            is_white_turn = !is_white_turn;
            AudioManager.Instance.PlaySFX("checkmate");
            UiManager.Instance.EndGame();
            pause = true;
            return ;
        }
        SwitchCamera();
        if(AiManager.Instance.is_ai && !is_white_turn && !pause) 
            StartCoroutine(AiManager.Instance.AiTurn());
    }
    private void UpdateSelection() {
        if(!Camera.main) {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 25.0f, LayerMask.GetMask("ChessPlane"))) //truyen vao hit
        {
            selection_X = (int) hit.point.x;
            selection_Y = (int) hit.point.z;
        }
        else {
            selection_X = -1;
            selection_Y = -1;
        }
    }
    private void DrawChessBoard() {    
        Vector3 widthLine = Vector3.right * 8; //trux x
        Vector3 heightLine = Vector3.forward * 8; //truc z

        for(int i = 0; i < 9; i++) {
            Vector3 start_x = Vector3.forward * i; 
            Vector3 start_z = Vector3.right * i; 
            Debug.DrawLine(start_x, start_x + widthLine);
            Debug.DrawLine(start_z, start_z + heightLine);
        } 
    }
    private void DrawSelection() {
        if(selection_X >= 0 && selection_Y >= 0) {
            Debug.DrawLine(Vector3.forward * selection_Y + Vector3.right * selection_X, Vector3.forward * (selection_Y + 1) + Vector3.right * (selection_X + 1));
            Debug.DrawLine(Vector3.forward * (selection_Y + 1) + Vector3.right * selection_X, Vector3.forward * selection_Y + Vector3.right * (selection_X + 1));
        }
    }
    private IEnumerator Smoothmove(Chessman chessman, Vector3 old_pos, Vector3 new_pos, float time) {
        float elapsedTime = 0.0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;
            if(!chessman.IsDestroyed())
                chessman.transform.position = Vector3.Lerp(old_pos, new_pos + Vector3.up * 0.5f, t);
            yield return null;
        }
        while (elapsedTime < time*1.2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;
            if(!chessman.IsDestroyed())
                chessman.transform.position = Vector3.Lerp(new_pos + Vector3.up * 0.5f, new_pos, t);
            yield return null;
        }
    }
    public Vector3 GetTileCenter(int x, int y) {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
    public void SpawnChessman(int index, int x, int y, bool rotate) {
        GameObject go;
        if(rotate) {
            go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), orientation) as GameObject;
        } else {
            go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), Quaternion.identity) as GameObject;
        }
        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }
    public void SpawnChessmanOut(int index, int x, int y, bool rotate) {
        GameObject go;
        if(rotate) {
            go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), orientation) as GameObject;
        } else {
            go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), Quaternion.identity) as GameObject;
        }
        go.transform.SetParent(transform);
        Chessman chessman = go.GetComponent<Chessman>();
        Chessman[,] list = null;
        if(chessman.is_white) 
            list = white_out;
        else 
            list = black_out;
        for(int i = 0; i < 2; i++) {
            for(int j = 7; j >= 0; j--) {
                if(list[i,j] == null) {
                    list[i,j] = chessman;
                    return;
                } 
            }
        }
    }
    public void SpawnAllChessman() {
    //White team
        //King
        SpawnChessman(0, 4, 0, false); 

        //Queen
        SpawnChessman(1, 3, 0, false); 

        //Rooks
        SpawnChessman(2, 0, 0, false);
        SpawnChessman(2, 7, 0, false);

        //Bishops
        SpawnChessman(3, 2, 0, false);
        SpawnChessman(3, 5, 0, false);

        //Knights
        SpawnChessman(4, 1, 0, false);
        SpawnChessman(4, 6, 0, false);

        //Pawns
        for(int i = 0; i < 8; i++) {
            SpawnChessman(5, i, 1, false);
        }

    //Black team
        //King
        SpawnChessman(6, 4, 7, true); 

        //Queen
        SpawnChessman(7, 3, 7, true); 

        //Rooks
        SpawnChessman(8, 0, 7, true);
        SpawnChessman(8, 7, 7, true);

        //Bishops
        SpawnChessman(9, 2, 7, true);
        SpawnChessman(9, 5, 7, true);

        //Knights
        SpawnChessman(10, 1, 7, true);
        SpawnChessman(10, 6, 7, true);

        //Pawns
        for(int i = 0; i < 8; i++) {
            SpawnChessman(11, i, 6, true);
        }
    }
    private void EndGame(int x, int y) {
        Chessmans[selected_Chessman.current_X, selected_Chessman.current_Y] = null;
        StartCoroutine(Smoothmove(selected_Chessman, selected_Chessman.transform.position, GetTileCenter(x, y), 0.2f));
        selected_Chessman.SetPosition(x, y);
        Chessmans[x, y] = selected_Chessman;
        selected_Chessman.GetComponent<MeshRenderer>().material = previous_mat;
        BoardHighlights.Instance.Hidehighlights();
        selected_Chessman = null;
        UiManager.Instance.EndGame();
        pause = true;
    }
    public void Restart() {
        foreach(GameObject go in activeChessman) {
            Destroy(go);
        }
        foreach(Chessman go in white_out) {
            if(go != null)
                Destroy(go.gameObject);
        }
        foreach(Chessman go in black_out) {
            if(go != null)
                Destroy(go.gameObject);
        }
        is_white_turn = true;
        SwitchCamera();
        pause = false;
        UiManager.Instance.HideAllUi();
        SpawnAllChessman();
    }
    public void SwitchCamera() {
        if(IsLockCam()) {return;}
        if(is_white_turn) {
            cameras[0].gameObject.SetActive(true);
            cameras[1].gameObject.SetActive(false);
        } else {
            cameras[1].gameObject.SetActive(true);
            cameras[0].gameObject.SetActive(false);
        }
    }
    private bool IsLockCam() {
        return UiManager.Instance.IsLockCam();
    }
    public void ChangeAngleCamera(int direction){
        if(IsLockCam()) {return;}
        for(int i = 0; i < 4; i++) {
            if(cameras[i].gameObject.activeSelf) {
                int index = i + direction;
                if(index < 0) {
                    index = 3;
                } else if(index > 3) {
                    index = 0;
                }
                cameras[index].gameObject.SetActive(true);
                foreach(CinemachineVirtualCamera cam in cameras) {
                    if(cam != cameras[index]) {
                        cam.gameObject.SetActive(false);
                    }
                }
                break;
            }
        }
    }
    private bool IsCheckMate() {
        bool[,][,] atk_chessman = new bool[8,8][,];
        bool[,][,] def_chessman = new bool[8,8][,];
        Chessman target_king = null;
        bool[,] king_allowedmove = new bool[8,8];
        int[,] level_allowedmove = new int[8,8];
        bool is_attacked = false;

        //tim vua va dich
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                if(Chessmans[i,j] == null) {
                    continue;
                }
                if(is_white_turn) {
                    if(!Chessmans[i,j].is_white) {
                        atk_chessman[i,j] = Chessmans[i,j].PossibleMove();
                    } else if(Chessmans[i,j].GetType() == typeof(King)) {
                        target_king = Chessmans[i,j];
                        king_allowedmove = target_king.PossibleMove();
                    } else {
                        def_chessman[i,j] = Chessmans[i,j].PossibleMove();
                    }
                } else {
                    if(Chessmans[i,j].is_white) {
                        atk_chessman[i,j] = Chessmans[i,j].PossibleMove();
                    } else if(Chessmans[i,j].GetType() == typeof(King)) {
                        target_king = Chessmans[i,j];
                        king_allowedmove = target_king.PossibleMove();
                    } else {
                        def_chessman[i,j] = Chessmans[i,j].PossibleMove();
                    }
                }
            }
        }

        //kiem tra muc phong thu cac nuoc di cua vua
        foreach(bool[,] temp_allowedmove_chessman in atk_chessman) {
            if(temp_allowedmove_chessman == null) {continue;}
            for(int i = 0; i < 8; i++) {
                for(int j = 0; j < 8; j++) {
                    if(temp_allowedmove_chessman[i,j] && def_chessman[i,j] != null) {
                        def_chessman[i,j] = null;
                    } 
                }
            }
        }
        foreach(bool[,] temp_allowedmove_chessman in def_chessman) {
            if(temp_allowedmove_chessman == null) {continue;}
            for(int i = 0; i < 8; i++) {
                for(int j = 0; j < 8; j++) {
                    if(temp_allowedmove_chessman[i,j] && king_allowedmove[i,j]) {
                        level_allowedmove[i,j] += 1;
                    } 
                }
            }
        }
        
        //kiem tra nuoc di cua dich
        foreach(bool[,] temp_allowedmove_chessman in def_chessman) {
            if(temp_allowedmove_chessman == null) {continue;}
            for(int i = 0; i < 8; i++) {
                for(int j = 0; j < 8; j++) {
                    if(temp_allowedmove_chessman[i,j] && atk_chessman[i,j] != null) {
                        atk_chessman[i,j] = null;
                    } 
                }
            }
        }
        foreach(bool[,] temp_allowedmove_chessman in atk_chessman) {
            if(temp_allowedmove_chessman == null) {continue;}
            for(int i = 0; i < 8; i++) {
                for(int j = 0; j < 8; j++) {
                    if(temp_allowedmove_chessman[i,j] && king_allowedmove[i,j]) {
                        if(level_allowedmove[i,j] > 0) 
                            level_allowedmove[i,j] -= 1;
                        else if(level_allowedmove[i,j] == 0) 
                            king_allowedmove[i,j] = false;
                    } else if(temp_allowedmove_chessman[i,j] && target_king.current_X == i && target_king.current_Y == j) {
                        is_attacked = true;
                    }
                }
            }
        }

        //kiem tra duong di cua vua co bi chan ko
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                if(king_allowedmove[i,j]) {
                    return false;
                }
            }
        }
        return is_attacked;
    }
    public void Play() {
        Restart();
        cameras[0].gameObject.SetActive(true);
    }
    public void Home() {
        pause = true;
        UiManager.Instance.MainMenu();
        MainmenuManager.Instance.GetBackMenu();
        for(int i = 0; i < 4; i++) {
            if(cameras[i].gameObject.activeSelf) {
                cameras[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    #region OutBoard
    public Chessman[,] white_out {set;get;}
    public Chessman[,] black_out {set;get;}
    public void KickChessman(Chessman chessman) {
        Chessman[,] list = null;
        if(chessman == null) {return ;}
        if(chessman.is_white) 
            list = white_out;
        else 
            list = black_out;
        for(int i = 0; i < 2; i++) {
            for(int j = 7; j >= 0; j--) {
                if(list[i,j] == null) {
                    list[i,j] = chessman;
                    if(!chessman.is_white) 
                        StartCoroutine(Smoothmove(list[i,j], list[i,j].transform.position, GetTileCenter(9 + i, j), 0.1f));
                    else 
                        StartCoroutine(Smoothmove(list[i,j], list[i,j].transform.position, GetTileCenter(-2 - i, 7 - j), 0.1f));
                    return;
                } 
            }
        }
    }

    public Chessman[,] CopyChessBoard(Chessman[,] board) {
        Chessman[,] newArray = new Chessman[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                newArray[i, j] = board[i, j];
            }
        }
        return newArray;
    }
    #endregion
}
