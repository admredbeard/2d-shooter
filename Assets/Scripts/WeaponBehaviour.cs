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
    public float rifleBulletRange = 1.2f;
    public float pistolBulletRange = 0.5f;
    public float shotgunBulletRange = 0.35f;
    public int startRifleAmmunition = 0;
    public int startPistolAmmunition = 0;
    public int startShotgunAmmunition = 0;

    public int rifleMagazineSize = 0;
    public int pistolMagazineSize = 0;
    public int shotgunMagazineSize = 0;
    public float reloadTime = 1f;
    public float pistolCD = 1f;
    public float shotgunCD = 1f;
    public float rifleCD = 0.1f;

}
