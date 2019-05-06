using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    void Start()
    {
       
        wc = GetComponent<WeaponBehaviour>();
        ResetStats();
    }

    public float visionRange = 10f;
    public float health = 0f;
    public float maxHealth = 100f;
    public int id = -1; // Should be private, use getters, this is only for debug
    public int team = 0; // Should be private, use getters, this is only for debug
    WeaponBehaviour wc;
    GameController gc;
    private float rifleAmmunition = 0f;
    private float pistolAmmunition = 0f;
    private float shotgunAmmunition = 0f;
    private float rifleMagazineAmmunition = 0f;
    private float pistolMagazineAmmunition = 0f;
    private float shotgunMagazineAmmunition = 0f;
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

    public void ResetStats() {
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
        if (!reloading && !weaponSwap)
        {
            currentWeapon = newWeapon;
            weaponSwap = true;
            yield return new WaitForSeconds(wc.reloadTime);
            weaponSwap = false;
        }
        else
        {
            Debug.Log("Can't change weapon while reloading or swapping weapon");
        }
    }

    public IEnumerator ReloadWeapon()
    {
        if (currentWeapon != Weapon.Knife)
            yield return new WaitForSeconds(wc.reloadTime);

        if (!weaponSwap && !reloading)
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
        if (currentWeapon == Weapon.Rifle && !fired && !weaponSwap)
        {
            if (rifleMagazineAmmunition > 0)
            {
                fired = true;
                GameObject myBullet = Instantiate(wc.rifleBullet, transform.position + transform.up, Quaternion.identity);
                BulletInformation bulletInfo = myBullet.GetComponent<BulletInformation>();
                bulletInfo.InitiateBullet(wc.rifleDamage, wc.rifleBulletSpeed, transform.up);
                rifleMagazineAmmunition -= 1;
                yield return new WaitForSeconds(wc.rifleCD);
                fired = false;
            }
            else
            {
                print("No more rifle ammunition in magazine");
            }
        }
        else if (currentWeapon == Weapon.Pistol && !fired && !weaponSwap && pistolMagazineAmmunition > 0)
        {
            if (pistolMagazineAmmunition > 0)
            {
                fired = true;
                GameObject myBullet = Instantiate(wc.pistolBullet, transform.position + transform.up, Quaternion.identity);
                BulletInformation bulletInfo = myBullet.GetComponent<BulletInformation>();
                bulletInfo.InitiateBullet(wc.pistolDamage, wc.pistolBulletSpeed, transform.up);
                pistolMagazineAmmunition -= 1;
                yield return new WaitForSeconds(wc.pistolCD);
                fired = false;
            }
            else
            {
                print("No more pistol ammunition in magazine");
            }
        }
        else if (currentWeapon == Weapon.Shotgun && !fired && !weaponSwap)
        {
            if (shotgunMagazineAmmunition > 0)
            {
                fired = true;
                GameObject myBullet = Instantiate(wc.shotgunBullet, transform.position + transform.up, Quaternion.identity);
                BulletInformation bulletInfo = myBullet.GetComponent<BulletInformation>();
                bulletInfo.InitiateBullet(wc.shotgunDamage, wc.shotgunBulletSpeed, transform.up);
                shotgunMagazineAmmunition -= 1;
                yield return new WaitForSeconds(wc.shotgunCD);
                fired = false;
            }
            else
            {
                print("No more shotgun ammuniton in magazine");
            }
        }
        else
        {
            print(currentWeapon.ToString() + " bullet cooldown");
        }
    }
}

public enum Weapon
{
    Knife,
    Pistol,
    Rifle,
    Shotgun
}
