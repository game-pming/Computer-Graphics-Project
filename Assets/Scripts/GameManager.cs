using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player player;
    public Monster monster;

    public GameObject playerGO;
    public GameObject monsterGO;

    public GameObject MainCam;
    public GameObject MenuCam;

    public GameObject MainPanel;
    public GameObject MenuPanel;
    public Text HpTxt;
    public Text ScoreTxt;
    public Text CoinTxt;
    
    bool gameStart;
    public void GameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameStart = true;
        
        MenuCam.SetActive(false);
        MainCam.SetActive(true);

        playerGO.SetActive(true);
        monsterGO.SetActive(true);
        MenuPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    void Update()
    {
        HpTxt.text = string.Format("{0:n0}",player.hp);
        ScoreTxt.text = string.Format("{0:n0}", player.score);
        CoinTxt.text = string.Format("{0:n0}", player.coin);
    }
}
