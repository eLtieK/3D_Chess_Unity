using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainmenuManager : MonoBehaviour
{
    public static MainmenuManager Instance;
    private Quaternion orientation = Quaternion.Euler(0,180,0);
    private GameObject[] v_camera;
    private Chessman[,] Chessmans {set;get;}
    public List<GameObject> chessmanPrefabs;
    private int index = 0;
    public bool menu;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private void Start() {
        Instance = this;
        v_camera = new GameObject[4];
        Chessmans = new Chessman[8,8];
        menu = true;
        index = 0;
        for(int i = 1; i <= 4; i++) {
            GameObject cam = GameObject.Find("Camera MainMenu " + i);
            v_camera[i-1] = cam;
        }
        MainMenu();
        //SpawnAllChessmanRandom();
        //SpawnAllChessman();
    }
    private void Update() {
        CinemachineBrain brain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
        if(menu && !brain.IsBlending) {
            CinemachineBlendDefinition newBlend = new CinemachineBlendDefinition
            {
                m_Style = CinemachineBlendDefinition.Style.EaseInOut,
                m_Time = 10.0f
            };
            brain.m_DefaultBlend = newBlend;
            SwitchCamera();
        }
    }
    public void MainMenu() {
        for(int i = 0; i < 4; i++) {
            if(i == 0) {
                v_camera[i].SetActive(true);
            }
            else
                v_camera[i].SetActive(false);
        } 
    }
    public void SwitchCamera() {
        if(index < 3) {
            index += 1;
            v_camera[index].SetActive(true);
            v_camera[index - 1].SetActive(false);
        } else {
            index = 0;
            v_camera[index].SetActive(true);
            v_camera[3].SetActive(false);
        }
    }
    public void Play() {
        menu = false;
        for(int i = 0; i < 4; i++) {
            v_camera[i].SetActive(false);
        }
        index = 0;
        CinemachineBrain brain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
        CinemachineBlendDefinition newBlend = new CinemachineBlendDefinition
        {
            m_Style = CinemachineBlendDefinition.Style.EaseInOut,
            m_Time = 2.0f
        };
        brain.m_DefaultBlend = newBlend;
    }
    private void SpawnChessman(int index, int x, int y, bool rotate) {
        GameObject go;
        if(rotate) {
            go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), orientation) as GameObject;
        } else {
            go = Instantiate(chessmanPrefabs[index], GetTileCenter(x,y), Quaternion.identity) as GameObject;
        }
        go.transform.SetParent(transform);
        Chessmans[x - 20, y - 20] = go.GetComponent<Chessman>();
    }
    public Vector3 GetTileCenter(int x, int y) {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
    private void SpawnAllChessmanRandom() {
        int x,y;
        bool rotate;
        int time;
        for(int i = 0; i < 12; i++) {
            if((i > 1 && i < 5) || (i > 7 && i < 11)) 
                time = 2;
            else if(i == 5 || i == 11) 
                time = 8;
            else    
                time = 1;
            while(true) {
                x = Random.Range(0,7);
                y = Random.Range(0,7);
                if(Chessmans[x,y] == null) {
                    if(i < 6)
                        rotate = false;
                    else 
                        rotate = true;
                    SpawnChessman(i, x + 20, y + 20, rotate);
                    time--;
                    if(time == 0)
                        break;
                } 
            }
        }
    }
    public void GetBackMenu() {
        v_camera[0].SetActive(true);
        index = 0;
        StartCoroutine(WaitThenMenu(2.0f));
    }
    IEnumerator WaitThenMenu(float seconds) {
        yield return new WaitForSeconds(seconds); 
        menu = true;
    }
    private void SpawnAllChessman() {
    //White team
        //King
        SpawnChessman(0, 4 + 20, 0 + 20, false); 

        //Queen
        SpawnChessman(1, 3 + 20, 0 + 20, false); 

        //Rooks
        SpawnChessman(2, 0 + 20, 0 + 20, false);
        SpawnChessman(2, 7 + 20, 0 + 20, false);

        //Bishops
        SpawnChessman(3, 2 + 20, 0 + 20, false);
        SpawnChessman(3, 5 + 20, 0 + 20, false);

        //Knights
        SpawnChessman(4, 1 + 20, 0 + 20, false);
        SpawnChessman(4, 6 + 20, 0 + 20, false);

        //Pawns
        for(int i = 0; i < 8; i++) {
            SpawnChessman(5, i + 20, 1 + 20, false);
        }

    //Black team
        //King
        SpawnChessman(6, 4 + 20, 7 + 20, true); 

        //Queen
        SpawnChessman(7, 3 + 20, 7 + 20, true); 

        //Rooks
        SpawnChessman(8, 0 + 20, 7 + 20, true);
        SpawnChessman(8, 7 + 20, 7 + 20, true);

        //Bishops
        SpawnChessman(9, 2 + 20, 7 + 20, true);
        SpawnChessman(9, 5 + 20, 7 + 20, true);

        //Knights
        SpawnChessman(10, 1 + 20, 7 + 20, true);
        SpawnChessman(10, 6 + 20, 7 + 20, true);

        //Pawns
        for(int i = 0; i < 8; i++) {
            SpawnChessman(11, i + 20, 6 + 20, true);
        }
    }
}
