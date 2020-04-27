using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public ChessType chessColor = ChessType.Black;

    protected virtual void Start() { }
    private void Update()
    {
        if (chessColor == Board.Instance.turn)
            StartPlay();
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    public virtual void StartPlay()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //limit the condition and area that allow player to place chess
        if (Input.GetMouseButtonDown(0) && pos.x > -7.5f && pos.y > -7.5f && pos.x < 7.5f && pos.y < 7.5f && !Board.isEnd)
        {
            Board.Instance.StarterPlay(new int[2] { (int)(pos.x + (float)AI.boardLength / 2), (int)(pos.y + (float)AI.boardLength / 2) }) ;
        }
    }
}
