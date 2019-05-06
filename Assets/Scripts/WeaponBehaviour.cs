using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{

    public GameObject rifleBullet;
    public GameObject pistolBullet;
    public GameObject shotgunBullet;
    public float rifleDamage = 0f;
    public float pistolDamage = 0f;
    public float shotgunDamage = 0f;
    public float rifleBulletSpeed = 100f;
    public float pistolBulletSpeed = 70f;
    public float shotgunBulletSpeed = 80f;
    public float startRifleAmmunition = 0f;
    public float startPistolAmmunition = 0f;
    public float startShotgunAmmunition = 0f;

    public float rifleMagazineSize = 0f;
    public float pistolMagazineSize = 0f;
    public float shotgunMagazineSize = 0f;
    public float reloadTime = 1f;
    public float pistolCD = 1f;
    public float shotgunCD = 1f;
    public float rifleCD = 0.1f;

}
