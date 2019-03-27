﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBehaviour : MonoBehaviour
{
    float playerSpeed = 10; //speed player moves
    private  Animator anim;
    private bool knifeActive = true;
    private bool handgunActive = false;
    private bool shotgunActive = false;
    private bool rifleActive = false;
    private WeaponBehaviour weaponBehaviour;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        weaponBehaviour = GetComponent<WeaponBehaviour>();
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
                anim.SetTrigger("shoot");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            //We should check our atackspeed before we fire, we dont atm
            anim.SetTrigger("melee");
        }
    }
    
    void CheckOtherInput()
    {
        if (Input.GetKeyDown("1"))
        {
            knifeActive = true;
            handgunActive = false;
            shotgunActive = false;
            rifleActive = false;
            anim.SetTrigger("knife");
        }
        if (Input.GetKeyDown("2"))
        {
            knifeActive = false;
            handgunActive = true;
            shotgunActive = false;
            rifleActive = false;
            anim.SetTrigger("handgun");
        }

        if (Input.GetKeyDown("3"))
        {
            knifeActive = false;
            handgunActive = false;
            shotgunActive = true;
            rifleActive = false;
            anim.SetTrigger("rifle");
        }

        if (Input.GetKeyDown("4"))
        {
            knifeActive = false;
            handgunActive = false;
            shotgunActive = false;
            rifleActive = true;
            anim.SetTrigger("shotgun");
        }

        if (Input.GetKeyDown("r"))
        {
            
            if(!knifeActive) //This if is needed to reload as soon as we switch weapons
            {
                //we want to check if we can reaload (ie, ammo not full)
                anim.SetTrigger("reload");
            }
            
        }
    }
}
