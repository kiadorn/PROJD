﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class PersonalUI : MonoBehaviour {

    public Image hitmarker;
    public Image crosshair;
    public TextMeshProUGUI deathText;

    [Header("Shoot")]
    public Image ShootBar;
    public Image ChargeBar;
    public Image ChargeBar2;
    public GameObject ShootCDTextTimer;
    private float shootCooldown;
    private float shootMAX = 1f;
    private float shootYellowTime = 0f;
    private float shootGreenTime = 0f;
    private float chargeAmount;

    [Header("Dash")]
    public Image dashBar;
    public GameObject DashCDTextTimer;
    private float dashCountdown;
    private float dashMAX;
    private float dashYellowTime = 0f;
    private float dashGreenTime = 0f;

    [Header("Decoy")]
    public Image decoyBar;
    public GameObject DecoyCDTextTimer;
    private float decoyCountdown;
    private float decoyMAX;
    private float decoyYellowTime = 0f;
    private float decoyGreenTime = 0f;

    public static PersonalUI instance;

    public TeamUISwap uiSwap;


    private void Awake() {
        if (!instance) {
            instance = this;
        }
        else {
            Destroy(instance);
            instance = this;
        }
    }

    void Start () {
		
	}
	
	void Update () {
        UpdateUI();
    }

    private void UpdateUI() {
        UpdateDashBar();
        UpdateDecoyBar();//BORE GÖRAS SÅSMÅNINGOM
        UpdateShootCD();
    }

    public void StartDashTimer(float dashTimer) {
        dashMAX = dashTimer;
        dashCountdown = 0;
        dashBar.fillAmount = 0;
        DashCDTextTimer.SetActive(true);
    }

    public void StartDecoyTimer(float decoyTimer)//TO DO LATER
    {

        decoyMAX = decoyTimer;
        decoyCountdown = 0;
        decoyBar.fillAmount = 0;
        DecoyCDTextTimer.SetActive(true);

    }

    public void StartShootTimer(float shootTimer) {
        shootMAX = shootTimer;
        shootCooldown = shootMAX;
    }

    private void UpdateDashBar() {
        if (dashBar.fillAmount < 1) {
            dashBar.fillAmount = dashCountdown / dashMAX;
            dashCountdown += Time.deltaTime;

            //dashBar.color = Color.red;

            DashCDTextTimer.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(dashMAX - dashCountdown + 1)).ToString();
        }
        if (dashBar.fillAmount == 1) {
            //dashBar.color = Color.green;
            dashYellowTime = 0;
            dashGreenTime = 0;
            DashCDTextTimer.SetActive(false);
        }

        /*else if (dashBar.fillAmount <= 0.5) {
            dashBar.color = Color.Lerp(Color.red, Color.yellow, dashYellowTime);
            dashYellowTime += Time.deltaTime / (dashMAX / 2);
        }
        else if (dashBar.fillAmount > 0.5) {
            dashBar.color = Color.Lerp(Color.yellow, Color.green, dashGreenTime);
            dashGreenTime += Time.deltaTime / (dashMAX / 2);
        }*/


    }

    private void UpdateDecoyBar()
    {
        if (decoyBar.fillAmount < 1) {
            decoyBar.fillAmount = decoyCountdown / decoyMAX;
            decoyCountdown += Time.deltaTime;

            //decoyBar.color = Color.red;

            DecoyCDTextTimer.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(decoyMAX - decoyCountdown + 1)).ToString();
        }
        if (decoyBar.fillAmount == 1) {
            //decoyBar.color = Color.green;
            decoyYellowTime = 0;
            decoyGreenTime = 0;
            DecoyCDTextTimer.SetActive(false);
        }

        /*else if (decoyBar.fillAmount <= 0.5) {
            decoyBar.color = Color.Lerp(Color.red, Color.yellow, decoyYellowTime);
            decoyYellowTime += Time.deltaTime / (decoyMAX / 2);
        }
        else if (decoyBar.fillAmount > 0.5) {
            decoyBar.color = Color.Lerp(Color.yellow, Color.green, decoyGreenTime);
            decoyGreenTime += Time.deltaTime / (decoyMAX / 2);
        }*/


    }

    public void UpdateShootCharge(float beamDistance, float beamMax) {
        ChargeBar.fillAmount = ((beamDistance / (beamMax)));
        ChargeBar2.fillAmount = ChargeBar.fillAmount;
        chargeAmount = ChargeBar.fillAmount;
    }


    public void UpdateShootCD() {
        if (shootCooldown > 0) {
            if (!ShootCDTextTimer.activeInHierarchy) {
                ShootCDTextTimer.SetActive(true);
            }
            ShootCDTextTimer.GetComponentInChildren<TextMeshProUGUI>().text = ((int)shootCooldown + 1).ToString();
            ShootBar.fillAmount = 1 - ((shootCooldown / (shootMAX)));

            //shootBar.color = Color.red;
            //shootBar.color = new Color32(255, 255, 0, 50);
            ChargeBar.fillAmount = (chargeAmount * (shootCooldown / shootMAX));
            ChargeBar2.fillAmount = ChargeBar.fillAmount;
            //UpdateChargeCD();
            shootCooldown -= Time.deltaTime;

            if (shootCooldown <= 0)
            {
                ChargeBar.fillAmount = 0;
                ChargeBar2.fillAmount = 0;
            }

            /*if (shootBar.fillAmount <= 0.5) {
                shootBar.color = Color.Lerp(Color.red, Color.yellow, shootYellowTime);
                shootYellowTime += Time.deltaTime / (shootMAX / 2);
            }
            else if (shootBar.fillAmount > 0.5) {
                shootBar.color = Color.Lerp(Color.yellow, Color.green, shootGreenTime);
                shootGreenTime += Time.deltaTime / (shootMAX / 2);
            }*/
        }
        else {

            shootYellowTime = 0;
            shootGreenTime = 0;
            //shootBar.color = Color.green;
            ShootCDTextTimer.SetActive(false);
        }
    }

    public void UpdateChargeCD()
    {
        chargeAmount = ChargeBar.fillAmount;
        Debug.Log(ChargeBar.fillAmount);
        chargeAmount -= Time.deltaTime;
        
            ChargeBar.fillAmount = chargeAmount * ((shootCooldown / shootMAX));
            ChargeBar2.fillAmount = ChargeBar.fillAmount;
        
        
    }

    public IEnumerator ShowHitMarker() {
        Color c = hitmarker.color;
        float a = 1;

        while (a > 0) {
            a -= Time.deltaTime * 0.5f;
            hitmarker.color = new Color(c.r, c.g, c.b, a);
            yield return 0;
        }

    }
}
