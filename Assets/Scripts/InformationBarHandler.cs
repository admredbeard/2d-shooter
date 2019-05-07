using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationBarHandler : MonoBehaviour
{
    List<GameObject> players;
    PlayerBehaviour[] playerBehaviours;
    Text[] playerWeapons;
    Text[] playerAmmo;
    GameController gc;
    Text team1Score;
    Text team2Score;

    public Slider[] playerHPBars;
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        GameObject ui = GameObject.Find("UI").gameObject;
        players = gc.GetPlayers();
        playerBehaviours = new PlayerBehaviour[players.Count];
        playerWeapons = new Text[players.Count];
        playerAmmo = new Text[players.Count];
        playerHPBars = new Slider[players.Count];

        team1Score = GameObject.Find("Team1ScoreText").GetComponent<Text>();
        team2Score = GameObject.Find("Team2ScoreText").GetComponent<Text>();

        for (int i = 0; i < players.Count; i++)
        {
            playerBehaviours[i] = players[i].GetComponent<PlayerBehaviour>();
            playerWeapons[i] = GameObject.Find("Player" + (i + 1).ToString() + "Weapon").GetComponent<Text>();
            playerAmmo[i] = GameObject.Find("Player" + (i + 1).ToString() + "Ammunition").GetComponent<Text>();
            playerHPBars[i] = GameObject.Find("Player" + (i + 1).ToString() + "HP").GetComponent<Slider> ();
        }

        StartCoroutine(UpdateInfo());
    }

    IEnumerator UpdateInfo()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (playerHPBars[i] != null && playerBehaviours[i] != null)
            {
                playerHPBars[i].maxValue = playerBehaviours[i].maxHealth;
            }
            else
            {
                //playerHPBars[i].gameObject.SetActive(false);
            }
        }

        while (true)
        {
            team1Score.text = gc.GetTeamOneScore().ToString();
            team2Score.text = gc.GetTeamTwoScore().ToString();
            for (int i = 0; i < players.Count; i++)
            {

                if (players[i] != null)
                {
                    playerWeapons[i].text = playerBehaviours[i].GetWeapon().ToString();
                    playerAmmo[i].text = playerBehaviours[i].GetCurrentAmmo();
                    playerHPBars[i].value = playerBehaviours[i].health;
                }
                else
                {
                    playerWeapons[i].text = "-";
                    playerAmmo[i].text = "-";
                }
            }
            yield return new WaitForSeconds(0.025f);
        }
    }
}
