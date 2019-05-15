using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationHandler : MonoBehaviour
{

    public GameObject unitInfoObj;
    public float updateFrequency = 0.5f;

    private GameController gc;
    private List<GameObject> players;

    private GameObject team1InfoObj;
    private GameObject team2InfoObj;

    private List<PlayerInformationBar> team1Information;
    private List<PlayerInformationBar> team2Information;

    private Text team1Score;
    private Text team2Score;


    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        players = gc.GetPlayers();

        team1Information = new List<PlayerInformationBar>();
        team2Information = new List<PlayerInformationBar>();

        team1InfoObj = transform.Find("Team1").gameObject;
        team2InfoObj = transform.Find("Team2").gameObject;

        team1Score = team1InfoObj.transform.Find("ScoreText").GetComponent<Text>();
        team2Score = team2InfoObj.transform.Find("ScoreText").GetComponent<Text>();

        foreach (GameObject player in players){
            PlayerBehaviour p = player.GetComponent<PlayerBehaviour>();
            if (p.GetTeam() == 1){
                GameObject obj = Instantiate(unitInfoObj, team1InfoObj.transform.Find("UnitPanel"));
                obj.transform.Find("PlayerText").GetComponent<Text>().text = "Player " + (team1Information.Count + 1).ToString();
                Image hpImg = obj.transform.Find("HPBar").transform.Find("HpLeft").GetComponent<Image>();
                Image reloadImg = obj.transform.Find("ReloadingImage").GetComponent<Image>();
                Text weaponText = obj.transform.Find("WeaponText").GetComponent<Text>();
                Text ammoText = obj.transform.Find("AmmunitionText").GetComponent<Text>();
                obj.GetComponent<Button>().onClick.AddListener(delegate{ChangeCameraLoc(p);});
                team1Information.Add(new PlayerInformationBar(p, hpImg, reloadImg, weaponText, ammoText));
                
            }else{
                GameObject obj = Instantiate(unitInfoObj, team2InfoObj.transform.Find("UnitPanel"));
                obj.transform.Find("PlayerText").GetComponent<Text>().text = "Player " + (team2Information.Count + 1).ToString();
                Image hpImg = obj.transform.Find("HPBar").transform.Find("HpLeft").GetComponent<Image>();
                Image reloadImg = obj.transform.Find("ReloadingImage").GetComponent<Image>();
                Text weaponText = obj.transform.Find("WeaponText").GetComponent<Text>();
                Text ammoText = obj.transform.Find("AmmunitionText").GetComponent<Text>();
                obj.GetComponent<Button>().onClick.AddListener(delegate{ChangeCameraLoc(p);});
                team2Information.Add(new PlayerInformationBar(p, hpImg, reloadImg, weaponText, ammoText));
            }
        }
        StartCoroutine("UpdateInformation");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UpdateInformation(){
        while(true){
            team1Score.text = gc.GetTeamOneScore().ToString();
            team2Score.text = gc.GetTeamTwoScore().ToString();
            foreach(PlayerInformationBar player in team1Information){
                player.UpdatePlayerInformation();
            }
            foreach(PlayerInformationBar player in team2Information){
                player.UpdatePlayerInformation();
            }
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    public void ChangeCameraLoc(PlayerBehaviour p){
        Camera.main.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, -10);
    }

    public class PlayerInformationBar{
        PlayerBehaviour player;
        Image hpImage;
        Image reloadImage;
        Text weaponText;
        Text weaponAmmo;
        public PlayerInformationBar(PlayerBehaviour p, Image hp, Image reload, Text weapon, Text ammo){
            this.player = p;
            this.hpImage = hp;
            this.reloadImage = reload;
            this.weaponAmmo = ammo;
            this.weaponText = weapon;
        }

        public void UpdatePlayerInformation(){
            hpImage.fillAmount = player.GetHealth()/player.maxHealth;
            weaponAmmo.text = player.GetCurrentAmmo();
            weaponText.text = player.GetWeapon().ToString();

            //do stuff with reloadImg as well

        }
    }
}
