using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeClasses : MonoBehaviour {

    public GameObject SoldierMask;
    public GameObject SorcererMask;
    public GameObject ScoutrMask;


    public float speedMultiplier = 1f;

    public float shootCooldownMultyplier = 1f;
    public float dashDurationMultyplier = 1f;
    public float dashCooldownMultyplier = 1f;

    public PlayerController controller;

    private float baseForwardspeed;
    private float baseBackwardspeed;
    private float baseStrafespeed;

    //private float startBeamDistanceMultiplier;
    private float baseShootCooldown;
    private float baseDashDuration;
    private float baseDashCooldown;

    // Use this for initialization
    void Start() {
        controller = gameObject.GetComponent<PlayerController>();
        baseForwardspeed = controller.movementSettings.forwardSpeed;
        baseBackwardspeed = controller.movementSettings.backwardSpeed;
        baseStrafespeed = controller.movementSettings.strafeSpeed;

        baseShootCooldown = controller.shootCooldown;
        baseDashDuration = controller.dashDuration;
        baseDashCooldown = controller.dashCooldown;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("t"))//soldier
        {
            SetToSodierStats();
        }

        else if (Input.GetKeyDown("y")) //scout
        {
            SetToScoutStats();
        }

        else if (Input.GetKeyDown("u"))//sourcerer
        {
            SetToSourcererStats();
        }


        ClassStats();

    }

    public void ClassStats()
    {
        controller.movementSettings.forwardSpeed = baseForwardspeed * speedMultiplier;
        controller.movementSettings.backwardSpeed = baseBackwardspeed * speedMultiplier;
        controller.movementSettings.strafeSpeed = baseStrafespeed * speedMultiplier;

        controller.shootCooldown = baseShootCooldown * shootCooldownMultyplier;
        controller.dashDuration = baseDashDuration * dashDurationMultyplier;
        controller.dashCooldown = baseDashCooldown * dashCooldownMultyplier;

    }

    public void SetToSodierStats()
    {
        speedMultiplier = 1f;
        shootCooldownMultyplier = 1f;
        dashDurationMultyplier = 1f;
        dashCooldownMultyplier = 1f;

        SoldierMask.SetActive(true);
        SorcererMask.SetActive(false);
        ScoutrMask.SetActive(false);
    }

    public void SetToScoutStats()
    {
        speedMultiplier = 1.1f;
        shootCooldownMultyplier = 0.5f;
        dashDurationMultyplier = 0.5f;
        dashCooldownMultyplier = 0.5f;

        SoldierMask.SetActive(false);
        SorcererMask.SetActive(false);
        ScoutrMask.SetActive(true);
    }
    public void SetToSourcererStats()
    {
        speedMultiplier = 0.9f;
        shootCooldownMultyplier = 1.5f;
        dashDurationMultyplier = 1.5f;
        dashCooldownMultyplier = 1.5f;

        SoldierMask.SetActive(false);
        SorcererMask.SetActive(true);
        ScoutrMask.SetActive(false);
    }

}
