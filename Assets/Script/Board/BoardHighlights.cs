using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance{set;get;}
    public GameObject highlight_prefab;
    private List<GameObject> highlights;
    private float mouse_x = -1;
    private float mouse_y = -1;
    private void Start() {
        Instance = this;
        highlights = new List<GameObject>();
    }
    private void Update() {
        UpdateMouse();
    }
    public void UpdateHighlight(int x, int z) {
        mouse_x = x + 0.5f;
        mouse_y = z + 0.5f;
        
        GameObject go = highlights.Find(g => g.transform.position.x == mouse_x && g.transform.position.z == mouse_y);
        if(go != null && x == BoardManager.Instance.EnPassantMove[0] && z == BoardManager.Instance.EnPassantMove[1]) {
            go.GetComponent<Renderer>().material.color = Color.red;
        } else if(go != null && BoardManager.Instance.Chessmans[x, z] == null) {
            go.transform.position = new Vector3(go.transform.position.x, 0.3f, go.transform.position.z);
            go.GetComponent<Renderer>().material.color = Color.yellow;
        } else if(go != null) {
            go.GetComponent<Renderer>().material.color = Color.red;
        }
        List<GameObject> all = highlights.FindAll(g => g != go);
        foreach(GameObject la in all) {
            la.transform.position = new Vector3(la.transform.position.x, 0.1f, la.transform.position.z);
            la.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
    private void UpdateMouse() {
        if(AiManager.Instance.is_ai && !BoardManager.Instance.is_white_turn) 
            return;
        if(!Camera.main) {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50.0f, LayerMask.GetMask("Highlight"))) //truyen vao hit
        {
            mouse_x = (int) hit.point.x + 0.5f;
            mouse_y = (int) hit.point.z + 0.5f;
            
            GameObject go = highlights.Find(g => g.transform.position.x == mouse_x && g.transform.position.z == mouse_y);
            if(go != null && (int) hit.point.x == BoardManager.Instance.EnPassantMove[0] && (int) hit.point.z == BoardManager.Instance.EnPassantMove[1]) {
                go.GetComponent<Renderer>().material.color = Color.red;
            } else if(go != null && BoardManager.Instance.Chessmans[(int) hit.point.x, (int) hit.point.z] == null) {
                go.transform.position = new Vector3(go.transform.position.x, 0.3f, go.transform.position.z);
                go.GetComponent<Renderer>().material.color = Color.yellow;
            } else if(go != null) {
                go.GetComponent<Renderer>().material.color = Color.red;
            }
            List<GameObject> all = highlights.FindAll(g => g != go);
            foreach(GameObject la in all) {
                la.transform.position = new Vector3(la.transform.position.x, 0.1f, la.transform.position.z);
                la.GetComponent<Renderer>().material.color = Color.yellow;
            }
        } else {
            List<GameObject> all = highlights.FindAll(g => g);
            foreach(GameObject la in all) {
                la.transform.position = new Vector3(la.transform.position.x, 0.1f, la.transform.position.z);
                la.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    private GameObject GetHighlightObject() {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if(go == null) {
            go = Instantiate(highlight_prefab);
            go.GetComponent<Renderer>().material.color = Color.yellow;
            go.layer = LayerMask.NameToLayer("Highlight");
            highlights.Add(go);
        }
        return go;
    }
    public void HighlightAllowedMoves(bool[,] moves) {
        for(int i = 0; i < 8; i++) {
           for(int j = 0; j < 8; j++) {
                if(moves[i,j]) {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0 + 0.1f,j + 0.5f);
                }
            } 
        }
    }
    public void Hidehighlights() {
        foreach(GameObject go in highlights) {
            go.GetComponent<Renderer>().material.color = Color.yellow;
            go.SetActive(false);
        }
    }
}
