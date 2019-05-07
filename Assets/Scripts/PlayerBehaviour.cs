using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{

    void Start()
    {
        wc = GameObject.Find("Bullets").GetComponent<WeaponBehaviour>();
        ResetStats();

    }

    public float visionRange = 10f;
    private float health = 0f;
    public float maxHealth = 100f;
    private int id = -1; // Should be private, use getters, this is only for debug
    private int team = 0; // Should be private, use getters, this is only for debug
    WeaponBehaviour wc;
    GameController gc;
    private int rifleAmmunition = 0;
    private int pistolAmmunition = 0;
    private int shotgunAmmunition = 0;
    private int rifleMagazineAmmunition = 0;
    private int pistolMagazineAmmunition = 0;
    private int shotgunMagazineAmmunition = 0;
    [System.NonSerialized]
    public bool fired = false;
    [System.NonSerialized]
    public bool weaponSwap = false;
    [System.NonSerialized]
    public bool reloading = false;
    private Weapon currentWeapon;

    // Gets the hp of a unit.
    public float GetHealth()
    {
        return health;
    }

    // Gets the id of a unit.
    public int GetID()
    {
        return id;
    }

    public int GetReserveAmmunition(Weapon weapon)
    {
        if (weapon == Weapon.Rifle)
            return rifleAmmunition;
        else if (weapon == Weapon.Pistol)
            return pistolAmmunition;
        else if (weapon == Weapon.Shotgun)
            return shotgunAmmunition;
        else
            return 0;
    }

    public int GetMagazineAmmunition(Weapon weapon)
    {
        if (weapon == Weapon.Rifle)
            return rifleMagazineAmmunition;
        else if (weapon == Weapon.Pistol)
            return pistolMagazineAmmunition;
        else if (weapon == Weapon.Shotgun)
            return shotgunMagazineAmmunition;
        else
            return 0;
    }

    // Sets the id of a unit.
    public void SetID(int id)
    {
        this.id = id;
    }

    // Gets the team of a unit.
    public int GetTeam()
    {
        return team;
    }

    // Sets the team of a unit.
    public void SetTeam(int team)
    {
        this.team = team;
    }

    // Applies damage.
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void ResetStats()
    {
        health = maxHealth;
        rifleAmmunition = wc.startRifleAmmunition - wc.rifleMagazineSize;
        shotgunAmmunition = wc.startShotgunAmmunition - wc.shotgunMagazineSize;
        pistolAmmunition = wc.startPistolAmmunition - wc.pistolMagazineSize;
        rifleMagazineAmmunition = wc.rifleMagazineSize;
        pistolMagazineAmmunition = wc.pistolMagazineSize;
        shotgunMagazineAmmunition = wc.shotgunMagazineSize;
        currentWeapon = Weapon.Knife;
    }

    public Weapon GetWeapon()
    {
        return currentWeapon;
    }

    public string GetCurrentAmmo()
    {
        if (currentWeapon == Weapon.Rifle)
        {
            return rifleMagazineAmmunition + " / " + rifleAmmunition;
        }
        else if (currentWeapon == Weapon.Pistol)
        {
            return pistolMagazineAmmunition + " / " + pistolAmmunition;
        }
        else if (currentWeapon == Weapon.Shotgun)
        {
            return shotgunMagazineAmmunition + " / " + shotgunAmmunition;
        }
        else
        {
            return "-";
        }
    }

    public IEnumerator ChangeWeapon(Weapon newWeapon)
    {
        if (!reloading && !weaponSwap && !fired)
        {
            currentWeapon = newWeapon;
            weaponSwap = true;
            yield return new WaitForSeconds(wc.reloadTime);
            weaponSwap = false;
        }
    }

    public IEnumerator ReloadWeapon()
    {
        if (currentWeapon != Weapon.Knife)
            yield return new WaitForSeconds(wc.reloadTime);

        if (!weaponSwap && !reloading && !fired)
        {
            if (currentWeapon == Weapon.Rifle)
            {
                if (rifleAmmunition > wc.rifleMagazineSize)
                {
                    rifleMagazineAmmunition = wc.rifleMagazineSize;
                    rifleAmmunition -= rifleMagazineAmmunition;
                }
                else if (rifleAmmunition > 0)
                {
                    rifleMagazineAmmunition = rifleAmmunition;
                    rifleAmmunition -= rifleMagazineAmmunition;
                }
                else
                {
                    Debug.Log("Out of rifle ammunition");
                }
            }
            else if (currentWeapon == Weapon.Pistol)
            {
                if (pistolAmmunition > wc.pistolMagazineSize)
                {
                    pistolMagazineAmmunition = wc.pistolMagazineSize;
                    pistolAmmunition -= pistolMagazineAmmunition;
                }
                else if (pistolAmmunition > 0)
                {
                    pistolMagazineAmmunition = pistolAmmunition;
                    pistolAmmunition -= pistolMagazineAmmunition;
                }
                else
                {
                    Debug.Log("Out of pistol ammunition");
                }
            }
            else if (currentWeapon == Weapon.Shotgun)
            {
                if (shotgunAmmunition > wc.shotgunMagazineSize)
                {
                    shotgunMagazineAmmunition = wc.shotgunMagazineSize;
                    shotgunAmmunition -= shotgunMagazineAmmunition;
                }
                else if (shotgunAmmunition > 0)
                {
                    shotgunMagazineAmmunition = shotgunAmmunition;
                    shotgunAmmunition -= shotgunMagazineAmmunition;
                }
                else
                {
                    Debug.Log("Out of shotgun ammunition");
                }
            }
            else
            {
                Debug.Log("Why you trying to reload knife???");
            }
        }
        else
        {
            Debug.Log("Can't reload while swapping weapons or reloading");
        }
    }

    public IEnumerator FireWeapon()
    {
        if (!fired && !weaponSwap && !reloading)
        {
            if (currentWeapon == Weapon.Rifle)
            {
                if (rifleMagazineAmmunition > 0)
                {
                    fired = true;
                    GameObject myBullet = Instantiate(wc.rifleBullet, transform.position + (transform.up * 2), Quaternion.identity);
                    BulletInformation bulletInfo = myBullet.GetComponent<BulletInformation>();
                    bulletInfo.InitiateBullet(wc.rifleDamage, wc.rifleBulletSpeed, transform.up, gameObject, wc.rifleBulletRange);

                    rifleMagazineAmmunition -= 1;
                    yield return new WaitForSeconds(wc.rifleCD);
                    fired = false;
                }
                else
                {
                    print("No more rifle ammunition in magazine");
                }
            }
            else if (currentWeapon == Weapon.Pistol)
            {
                if (pistolMagazineAmmunition > 0)
                {
                    fired = true;
                    GameObject myBullet = Instantiate(wc.pistolBullet, transform.position + (transform.up * 2), Quaternion.identity);
                    BulletInformation bulletInfo = myBullet.GetComponent<BulletInformation>();
                    bulletInfo.InitiateBullet(wc.pistolDamage, wc.pistolBulletSpeed, transform.up, gameObject, wc.pistolBulletRange);

                    pistolMagazineAmmunition -= 1;
                    yield return new WaitForSeconds(wc.pistolCD);
                    fired = false;
                }

                else
                {
                    print("No more pistol ammunition in magazine");
                }
            }
            else if (currentWeapon == Weapon.Shotgun)
            {
                int shotgunBulletAmount = 6; //This number needs to be dividable with 3, eg 3, 6, 9 etc.
                float spreadFactor = 0.09f;
                if (shotgunMagazineAmmunition > 0)
                {
                    fired = true;
                    for (int i = 0; i < shotgunBulletAmount; i++)
                    {
                        Vector3[] bullet = RandomDirections(transform.up, spreadFactor * i, 1);
                        GameObject myBullet = Instantiate(wc.shotgunBullet, transform.position + (transform.up * 2), Quaternion.identity);
                        BulletInformation straightBulletInfo = myBullet.GetComponent<BulletInformation>();
                        straightBulletInfo.InitiateBullet(wc.shotgunDamage, wc.shotgunBulletSpeed, bullet[0], gameObject, wc.shotgunBulletRange);
                    }

                    shotgunMagazineAmmunition -= 1;
                    yield return new WaitForSeconds(wc.shotgunCD);
                    fired = false;
                }
                else
                {
                    print("No more shotgun ammuniton in magazine");
                }
            }
        }
    }

    public void SwingKnife()
    {
        if (!fired && !weaponSwap && !reloading)
        {
            if (currentWeapon == Weapon.Knife)
            {
                Collider2D hitArea = new BoxCollider2D ();
                hitArea = Physics2D.OverlapBox(gameObject.transform.position + gameObject.transform.forward, new Vector2(2f, 2f), 0f);
                
            }
        }
    }

    Vector3[] RandomDirections(Vector3 direction, float offset, int amount)
    {
        Vector3[] bulletDirs = new Vector3[amount];
        for (int i = 0; i < amount; i++)
        {
            float x = Random.Range(-offset, offset);
            float y = Random.Range(-offset, offset);
            bulletDirs[i] = direction + new Vector3(x, y, 0f);
        }
        return bulletDirs;
    }
}

public enum Weapon
{
    Knife,
    Pistol,
    Rifle,
    Shotgun
}
