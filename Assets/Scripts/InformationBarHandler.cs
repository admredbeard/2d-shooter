using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationBarHandler : MonoBehaviour
{

    GameObject[] players = new GameObject[6];
    PlayerBehaviour[] playerBehaviours = new PlayerBehaviour[6];
    Text[] playerWeapons = new Text[6];
    Text[] playerAmmo = new Text[6];

    void Start()
    {
        players[0] = GameObject.Find("CoolDude");
        //players[1] = GameObject.Find("Player2").GetComponent<Text>().text;
        //players[2] = GameObject.Find("Player3").GetComponent<Text>().text;
        //players[3] = GameObject.Find("Player4").GetComponent<Text>().text;
        //players[4] = GameObject.Find("Player5").GetComponent<Text>().text;
        //players[5] = GameObject.Find("Player6").GetComponent<Text>().text;

        playerBehaviours[0] = players[0].GetComponent<PlayerBehaviour>();
        //playerBehaviours[1] = players[1].GetComponent<PlayerBehaviour> ();
        //playerBehaviours[2] = players[2].GetComponent<PlayerBehaviour> ();
        //playerBehaviours[3] = players[3].GetComponent<PlayerBehaviour> ();
        //playerBehaviours[4] = players[4].GetComponent<PlayerBehaviour> ();
        //playerBehaviours[5] = players[5].GetComponent<PlayerBehaviour> ();

        playerWeapons[0] = GameObject.Find("Player1Weapon").GetComponent<Text>();
        playerWeapons[1] = GameObject.Find("Player2Weapon").GetComponent<Text>();
        playerWeapons[2] = GameObject.Find("Player3Weapon").GetComponent<Text>();
        playerWeapons[3] = GameObject.Find("Player4Weapon").GetComponent<Text>();
        playerWeapons[4] = GameObject.Find("Player5Weapon").GetComponent<Text>();
        playerWeapons[5] = GameObject.Find("Player6Weapon").GetComponent<Text>();

        playerAmmo[0] = GameObject.Find("Player1Ammunition").GetComponent<Text>();
        playerAmmo[1] = GameObject.Find("Player2Ammunition").GetComponent<Text>();
        playerAmmo[2] = GameObject.Find("Player3Ammunition").GetComponent<Text>();
        playerAmmo[3] = GameObject.Find("Player4Ammunition").GetComponent<Text>();
        playerAmmo[4] = GameObject.Find("Player5Ammunition").GetComponent<Text>();
        playerAmmo[5] = GameObject.Find("Player6Ammunition").GetComponent<Text>();

        StartCoroutine(UpdateInfo());
    }

    IEnumerator UpdateInfo()
    {
        while (true)
        {

            for (int i = 0; i < players.Length; i++)
            {

                if (players[i] != null)
                {
                    playerWeapons[i].text = playerBehaviours[i].GetWeapon().ToString();
                    playerAmmo[i].text = playerBehaviours[i].GetCurrentAmmo();
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
