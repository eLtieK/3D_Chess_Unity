using System.Collections;
using Cinemachine;
using UnityEngine;

public class PromotionContainer : MonoBehaviour
{
    private int selection_X = -1;
    private int selection_Y = -1;
    private bool is_promotion = false;
    private int move_X = -1;
    private int move_Y = -1;
    private CinemachineVirtualCamera promotion_camera;
    private GameObject black_chessman;
    private GameObject white_chessman;
    private Chessman[] list_chessman;
    private Chessman select_chessman;
    public int pick_chessman;
    private void Find() {
        promotion_camera = GameObject.Find("Virtual Camera Promotion").GetComponent<CinemachineVirtualCamera>();
        black_chessman = GameObject.Find("Black Chessman");
        white_chessman = GameObject.Find("White Chessman");
    }
    private void SetUp() {
        list_chessman = new Chessman[4];
        promotion_camera.gameObject.SetActive(false);
    }
    public static PromotionContainer Instance;
    private void Start() {
        Instance = this;
        Find();
        SetUp();
    }
    public void StartPromotion(int move_x, int move_y) {
        promotion_camera.gameObject.SetActive(true);
        UiManager.Instance.SetUpPromotionStart();
        BoardManager.Instance.pause = true;
        is_promotion = true;
        GameObject list_chessman_object = null;
        move_X = move_x;
        move_Y = move_y;
        if(BoardManager.Instance.is_white_turn) {
            white_chessman.SetActive(true);
            black_chessman.SetActive(false);
            list_chessman_object = white_chessman;
        } else {
            white_chessman.SetActive(false);
            black_chessman.SetActive(true);
            list_chessman_object = black_chessman;
        }
        int i = 0;
        foreach(Transform child in list_chessman_object.transform) {
            list_chessman[i] = child.gameObject.GetComponent<Chessman>();
            i++;
        }
        StartCoroutine(Promotion(1.9f));
    }

    private void EndProtion() {
        if(!BoardManager.Instance.is_white_turn) {
            if(selection_X == 6)
                pick_chessman = 3;
            else if(selection_X == 5)
                pick_chessman = 4;
            else if(selection_X == 4)
                pick_chessman = 1;
            else if(selection_X == 3)
                pick_chessman = 2;

            BoardManager.Instance.SpawnChessman(pick_chessman, move_X, move_Y, false);
        } else {
            if(selection_X == 6)
                pick_chessman = 9;
            else if(selection_X == 5)
                pick_chessman = 10;
            else if(selection_X == 4)
                pick_chessman = 7;
            else if(selection_X == 3)
                pick_chessman = 8;

            BoardManager.Instance.SpawnChessman(pick_chessman, move_X, move_Y, true);
        }
        promotion_camera.gameObject.SetActive(false);
        BoardManager.Instance.pause = false;
        is_promotion = false;
        UiManager.Instance.EndPromotion();
        select_chessman = null;
        AudioManager.Instance.PlaySFX("promotion");
        if(AiManager.Instance.is_ai && !BoardManager.Instance.is_white_turn && !BoardManager.Instance.pause) 
            StartCoroutine(AiManager.Instance.AiTurn());
    }
    IEnumerator Promotion(float seconds) {
        yield return new WaitForSeconds(seconds); 
        UiManager.Instance.Promotion();
    }

    private void Update()
    {
        UpdateSelection();
        SelectChessman();
        ResetNotSelected();

        if(Input.GetMouseButtonDown(0) && is_promotion) {
            if(selection_X >= 0 && selection_Y >= 0) {
                if (select_chessman != null) {
                    EndProtion();
                } 
            }
        }
    }

    private void UpdateSelection() {
        if(!Camera.main) {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 25.0f, LayerMask.GetMask("ChessPlane"))) //truyen vao hit
        {
            selection_X = (int) hit.point.x + 20;
            selection_Y = (int) hit.point.z + 20;
        }
        else {
            selection_X = -1;
            selection_Y = -1;
        }
    }
    private void SelectChessman() {
        if(is_promotion) {
            Vector3 old_position;
            Vector3 new_position;
            int index = -1;
            if(selection_X == 6)
                index = 0;
            else if(selection_X == 5)
                index = 1;
            else if(selection_X == 4)
                index = 2;
            else if(selection_X == 3)
                index = 3;
            else {
                select_chessman = null;
                return;
            }
            select_chessman = list_chessman[index];
            old_position = select_chessman.transform.position;
            new_position = select_chessman.transform.position + Vector3.up * 0.5f;
            select_chessman.transform.Rotate(0f, 20.0f * Time.deltaTime, 0f, Space.Self);
            if(select_chessman.transform.position.y > 0.5f) {
                return;
            }
            select_chessman.transform.position = Vector3.Lerp(old_position, new_position, 0.2f);
        }
    }
    private void ResetNotSelected() {
        foreach(Chessman child in list_chessman) {
            if(child != select_chessman) {
                Vector3 reset_position = child.gameObject.transform.position;
                reset_position.y = 0;
                child.transform.position = Vector3.Lerp(child.transform.position, reset_position, 0.2f);
            }
        }
    }
}
