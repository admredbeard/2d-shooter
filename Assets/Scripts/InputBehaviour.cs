using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBehaviour : MonoBehaviour
{
    float playerSpeed = 10; //speed player moves
    private Animator anim;
    private bool knifeActive = true;
    private bool handgunActive = false;
    private bool shotgunActive = false;
    private bool rifleActive = false;
    PlayerBehaviour player;
    private WeaponBehaviour weaponBehaviour;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        weaponBehaviour = GetComponent<WeaponBehaviour>();
        player = gameObject.GetComponent<PlayerBehaviour>();
    }
    void Update()
    {
        RotateTowardsMouse();
        CheckMovementInput();
        CheckMouseInput();
        CheckOtherInput();
    }

    private void RotateTowardsMouse()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    private void CheckMovementInput()
    {
        bool moving = false;
        if (Input.GetKey("up") || Input.GetKey("w"))
        {
            anim.SetBool("moving", true);
            moving = true;
            transform.Translate(0, playerSpeed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey("down") || Input.GetKey("s"))
        {
            moving = true;
            anim.SetBool("moving", true);
            transform.Translate(0, -playerSpeed * Time.deltaTime, 0, Space.World);
        }
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            moving = true;
            anim.SetBool("moving", true);
            transform.Translate(-playerSpeed * Time.deltaTime, 0, 0, Space.World);
        }
        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            moving = true;
            anim.SetBool("moving", true);
            transform.Translate(playerSpeed * Time.deltaTime, 0, 0, Space.World);
        }

        if (moving == false)
        {
            anim.SetBool("moving", false);
        }
    }

    void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //We should check our atackspeed before we fire, we dont atm    
            if (!knifeActive)
            {
                Fire();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            //We should check our atackspeed before we fire, we dont atm
            Fire();
        }
    }

    void CheckOtherInput()
    {
        if (!player.fired && !player.reloading && !player.weaponSwap)
        {
            if (Input.GetKeyDown("1"))
            {
                knifeActive = true;
                handgunActive = false;
                shotgunActive = false;
                rifleActive = false;
                //anim.SetTrigger("knife");
                ChangeWeapon(1);
            }
            if (Input.GetKeyDown("2"))
            {
                knifeActive = false;
                handgunActive = true;
                shotgunActive = false;
                rifleActive = false;
                //anim.SetTrigger("handgun");
                ChangeWeapon(2);
            }

            if (Input.GetKeyDown("3"))
            {
                knifeActive = false;
                handgunActive = false;
                shotgunActive = true;
                rifleActive = false;
                //anim.SetTrigger("rifle");
                ChangeWeapon(3);
            }

            if (Input.GetKeyDown("4"))
            {
                knifeActive = false;
                handgunActive = false;
                shotgunActive = false;
                rifleActive = true;
                //anim.SetTrigger("shotgun");
                ChangeWeapon(4);
            }
        }

        if (Input.GetKeyDown("r"))
        {
            if (!knifeActive) //This if is needed to reload as soon as we switch weapons
            {
                //anim.SetTrigger("reload");
                StartCoroutine(player.ReloadWeapon());
            }

        }
    }

    void ChangeWeapon(int weapon)
    {
        if (weapon == 1)
        {
            StartCoroutine(player.ChangeWeapon(Weapon.Knife));
        }
        else if (weapon == 2)
        {
            StartCoroutine(player.ChangeWeapon(Weapon.Pistol));
        }
        else if (weapon == 3)
        {
            StartCoroutine(player.ChangeWeapon(Weapon.Rifle));
        }
        else
        {
            StartCoroutine(player.ChangeWeapon(Weapon.Shotgun));
        }
    }

    void Fire()
    {
        StartCoroutine(player.FireWeapon());
        //GameObject myBullet = Instantiate(bullet, transform.position + transform.up, Quaternion.identity);
        //BulletInformation bulletInfo = myBullet.GetComponent<BulletInformation> ();
        //bulletInfo.InitiateBullet(1, 100f, transform.up);

    }
}
