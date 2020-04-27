using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Board : MonoBehaviour
{
    static Board _instance;
    public static bool isEnd=false;

    public ChessType turn = ChessType.Black;
    public int[,] grid;//lines on board
    public GameObject[] ChessPrefab;//black , white chess
    public Transform parent;

    public Stack<Transform> chessPos = new Stack<Transform>();

    public GameObject win;
    public GameObject lose;
    public static Board Instance
    {
        //singleton 
        get { return _instance; }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        //initialize grid which represent the position on chess board
        grid = new int[15, 15];
    }

    public bool StarterPlay(int[] pos)
    {
        pos[0] = Mathf.Clamp(pos[0], 0, 14);
        pos[1] = Mathf.Clamp(pos[1], 0, 14);

        //make sure the grid is empty
        if (grid[pos[0], pos[1]] != 0)
            return false;

        if (turn == ChessType.Black)
        {
            //center point is (0,0), transfer to point on board
            GameObject move = Instantiate(ChessPrefab[0], new Vector3(pos[0] - AI.halfBoard, pos[1] - AI.halfBoard), Quaternion.identity);
            chessPos.Push(move.transform);
            move.transform.SetParent(parent);
            grid[pos[0], pos[1]] = 1;

            //check winning state
            if(CheckWinner(pos))
            {
                End();
            }
            turn = ChessType.White;

        }
        else if (turn == ChessType.White)
        {
            GameObject move = Instantiate(ChessPrefab[1], new Vector3(pos[0] - AI.halfBoard, pos[1] - AI.halfBoard), Quaternion.identity);
            chessPos.Push(move.transform);
            move.transform.SetParent(parent);
            grid[pos[0], pos[1]] = 2;

            //check winning state
            if (CheckWinner(pos))
            {
                End();
            }
            turn = ChessType.Black;
        }

        return true;
    }

    void End()
    {
        Debug.Log("GameOver");
        //trigger winning UI
        if (turn == ChessType.White)
        {
            win.SetActive(true);
            isEnd = true;
        }
        else
        {
            lose.SetActive(true);
            isEnd = true;
        }
    }

    public bool CheckWinner(int[] pos)
    {
        //checking horizontally, vertically, diagnoally
        if (CheckLine(pos, new int[2] { 1, 0 })) return true;
        if (CheckLine(pos, new int[2] { 0, 1 })) return true;
        if (CheckLine(pos, new int[2] { 1, 1 })) return true;
        if (CheckLine(pos, new int[2] { 1, -1 })) return true;
        return false;
    }

    public bool CheckLine(int[] pos, int[] offset)
    {
        int count = 1;
        //pos = chess position, offset = the checking direction
        //check right
        for(int i = offset[0], j = offset[1]; (pos[0] + i >= 0 && pos[0] + i < AI.boardLength) 
            && (pos[1] + j >= 0 && pos[1] + j < AI.boardLength); i += offset[0], j += offset[1]) 
        {
            //if the checking direction's chess color is as same as the current chess color
            if (grid[pos[0] + i, pos[1] + j] == (int)turn)
            {
                count++;
            }
            else
                break;
        }
        //check left
        for (int i = -offset[0], j = -offset[1]; (pos[0] + i >= 0 && pos[0] + i < AI.boardLength)
            && (pos[1] + j >= 0 && pos[1] + j < AI.boardLength); i -= offset[0], j -= offset[1])
        {
            if (grid[pos[0] + i, pos[1] + j] == (int)turn)
            {
                count++;
            }
            else
                break;
        }

        if (count >= 5)//if 5 chess is linked 
            return true;
        else
            return false;
    }

    public void Retract()
    {
        if (chessPos.Count > 1)
        {
            //retract chess move
            Transform temp = chessPos.Pop();
            grid[(int)(temp.position.x + AI.halfBoard), (int)(temp.position.y + AI.halfBoard)] = 0;
            Destroy(temp.gameObject);
            temp = chessPos.Pop();
            grid[(int)(temp.position.x + AI.halfBoard), (int)(temp.position.y + AI.halfBoard)] = 0;
            Destroy(temp.gameObject);
        }
    }
}
public enum ChessType
{
    None,//0
    Black,//1
    White//2
}