using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxNode
{
    public int chessColor;
    public int[] pos;
    public List<MinMaxNode> childNode;
    public float value;
}
public class AI : Player
{
    static public int boardLength = 15;
    static public int halfBoard = 7;
    private Dictionary<string, float> score = new Dictionary<string, float>();

    private void Awake()
    {
        //add weight to rules. As "x" represents chess position "_" represents empty
        score.Add("xx___", 100);                      
        score.Add("x_x__", 100);
        score.Add("___xx", 100);
        score.Add("__x_x", 100);
        score.Add("x__x_", 100);
        score.Add("_x__x", 100);
        score.Add("x___x", 100);
      
        score.Add("_x_x_", 1000);
        score.Add("_x__x_", 1000);
        score.Add("_xx__", 1000);
        score.Add("__xx_", 1000);

        score.Add("x_x_x", 10000);
        score.Add("xx__x", 10000);
        score.Add("x__xx", 10000);
        score.Add("_xx_x", 10000);
        score.Add("x_xx_", 10000);
        score.Add("_x_xx", 10000);
        score.Add("xx_x_", 10000);
        score.Add("xxx__", 10000);
        score.Add("__xxx", 10000);

        score.Add("_xx_x_", 100000);                    
        score.Add("_x_xx_", 100000);
        score.Add("_xxx_", 100000);                        

        score.Add("x_xxx", 1000000);                   
        score.Add("xxx_x", 1000000);                    
        score.Add("_xxxx", 1000000);                    
        score.Add("xxxx_", 1000000);                   
        score.Add("xx_xx", 1000000);                        

        score.Add("_xxxx_", 10000000);

        score.Add("xxxxx", float.MaxValue);           
    }

    public float CheckLine(int[,] grid, int[] pos, int[] offset, int chess)
    {
        float countScore = 0;
        bool leftFirst = true, leftStop=false,rightStop=false;
        int count = 1;
        string str = "x";
        int rightx = offset[0], righty = offset[1];
        int leftx = -offset[0], lefty = -offset[1];

        while (count < halfBoard && (!leftStop || !rightStop))
        {
            //check left
            if (leftFirst)
            {
                if ((pos[0] + leftx >= 0 && pos[0] + leftx < boardLength) && 
                    pos[1] + lefty >= 0 && pos[1] + lefty < boardLength && !leftStop)
                {
                    //if the neighbor is same color
                    if (grid[pos[0] + leftx, pos[1] + lefty] == chess)
                    {
                        count++;
                        str = "x" + str;
                    }
                    //if the neighbor is empty, stop checking
                    else if(grid[pos[0] + leftx, pos[1] + lefty] == 0)
                    {
                        count++;
                        str = "_" + str;
                        if (!rightStop)
                            leftFirst = false;
                    }
                    //if the neighbor is enemy's chess, stop checking
                    else
                    {
                        leftStop = true;
                        if (!rightStop)
                            leftFirst = false;
                    }
                    leftx -= offset[0];
                    lefty -= offset[1];
                }
                else
                {
                    leftStop = true;
                    if (!rightStop)
                        leftFirst = false;
                }
            }
            //check right
            else
            {
                if ((pos[0] + rightx >= 0 && pos[0] + rightx < boardLength) &&
                    pos[1] + righty >= 0 && pos[1] + righty < boardLength && !leftFirst && !rightStop)
                {
                    if (grid[pos[0] + rightx, pos[1] + righty] == chess)
                    {
                        count++;
                        str = str + "x" ;
                    }
                    else if (grid[pos[0] + rightx, pos[1] + righty] == 0)
                    {
                        count++;
                        str = str + "_" ;
                        if (!leftStop)
                            leftFirst = true;
                    }
                    else
                    {
                        rightStop = true;
                        if (!leftStop)
                            leftFirst = true;
                    }
                    rightx += offset[0];
                    righty += offset[1];
                }
                else
                {
                    rightStop = true;
                    if (!leftStop)
                        leftFirst = true;
                }
            }
        }
        string tempStr = "";
        foreach(KeyValuePair<string,float> key in score)
        {
            if (str.Contains(key.Key))
            {
                //check if the rules is in dictionary and compare values
                if (tempStr != "")
                {
                    if (score[key.Key] > score[tempStr])
                        tempStr = key.Key;
                }
                else
                    tempStr = key.Key;
            }
        }

        if (tempStr != "")
        {
            countScore += score[tempStr];
        }
        return countScore;
    }

    public float GetScore(int[,] grid, int[] pos)
    {
        float countScore = 0;

        //check black vertically, horizontally, diagonally
        countScore += CheckLine(grid, pos, new int[2] { 1, 0 }, 1);
        countScore += CheckLine(grid, pos, new int[2] { 1, 1 }, 1);
        countScore += CheckLine(grid, pos, new int[2] { 1, -1 }, 1);
        countScore += CheckLine(grid, pos, new int[2] { 0, 1 }, 1);

        //check white vertically, horizontally, diagonally
        countScore += CheckLine(grid, pos, new int[2] { 1, 0 }, 2);
        countScore += CheckLine(grid, pos, new int[2] { 1, 1 }, 2);
        countScore += CheckLine(grid, pos, new int[2] { 1, -1 }, 2);
        countScore += CheckLine(grid, pos, new int[2] { 0, 1 }, 2);

        return countScore;
    }

    List<MinMaxNode> GetList(int[,] grid, int chess, bool isSelf)
    {
        List<MinMaxNode> nodes = new List<MinMaxNode>();
        MinMaxNode node;

        for(int i = 0; i < boardLength; i++)
        {
            for (int j = 0; j < boardLength; j++)
            {
                int[] pos = new int[2] { i, j };
                //if the current position is not empty
                if (grid[pos[0], pos[1]] != 0)  continue;

                //initialize a new node and set up information
                node = new MinMaxNode();
                node.pos = pos;
                node.chessColor = chess;

                //if the chess belongs to player
                if (isSelf)
                    node.value = GetScore(grid, pos);
                //if the chess belongs to enmey[
                else
                    node.value = -GetScore(grid, pos);
                //one chess have four options to go(right,up,right up, right bottom)
                if (nodes.Count < 4)
                    nodes.Add(node);

                else//if count >=4 , compare node value
                {
                    foreach(MinMaxNode item in nodes)
                    {
                        if(isSelf)//belongs to player, find max value
                        {
                            if (node.value > item.value)
                            {
                                nodes.Remove(item);
                                nodes.Add(node);
                                break;
                            }
                        }
                        else//belongs to enemy, find min value
                        {
                            if (node.value < item.value)
                            {
                                nodes.Remove(item);
                                nodes.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
        }
        return nodes;
    }

    public void GetTree(MinMaxNode node, int[,] grid, int depth, bool isSelf)
    {
        //if game is over, no need to go forward
        if (depth == 0 || node.value == float.MaxValue)
        {
            return;
        }

        grid[node.pos[0], node.pos[1]] = node.chessColor;

        node.childNode = GetList(grid, node.chessColor, !isSelf);

        foreach(MinMaxNode item in node.childNode)
        {
            GetTree(item, (int[,])grid.Clone(), depth - 1, !isSelf);
        }
    }

    public float AlphaBeta(MinMaxNode node,int depth, bool isSelf, float alpha,float beta)
    {
        if(depth ==0|| node.value == float.MaxValue || node.value == float.MinValue)
        {
            return node.value;
        }
        //if is player, find the max value
        if (isSelf)
        {
            foreach(MinMaxNode child in node.childNode)
            {
                //compare to self and childnodes
                alpha = Mathf.Max(alpha, AlphaBeta(child, depth - 1, !isSelf, alpha, beta));

                //cut alpha branch
                if (alpha >= beta)
                    return alpha;
            }
            return alpha;
        }
        //if is enemy, find the min value
        else
        {
            foreach(MinMaxNode child in node.childNode)
            {
                //compare to self and childnodes
                beta = Mathf.Min(beta, AlphaBeta(child, depth - 1, !isSelf, alpha, beta));

                //cut beta branch
                if (alpha >= beta)
                    return beta;
            }
            return beta;
        }
    }

    public override void StartPlay()
    {
        //if AI is black, let it occupy the center
        if (Board.Instance.chessPos.Count == 0)
        {
            Board.Instance.StarterPlay(new int[2] { halfBoard, halfBoard });
            return;
        }
        MinMaxNode node = null;
        
        foreach(var item in GetList(Board.Instance.grid, (int)chessColor, true))
        {
            //create tree for each node
            GetTree(item, (int[,])Board.Instance.grid.Clone(), 3, false);
            //find the alpha, beta value
            float a = float.MinValue;
            float b = float.MaxValue;

            item.value += AlphaBeta(item, 3, false, a, b);

            if (node != null)
            {
                if (node.value < item.value)
                    node = item;
            }
            else
                node = item;
        }
        Board.Instance.StarterPlay(node.pos);
    }
}
