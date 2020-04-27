using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
    public GameObject win,lose;

    public void Reload()
    {
        Board.isEnd = false;
        win.SetActive(false);
        lose.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
