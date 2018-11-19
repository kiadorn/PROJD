﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkDiscovery : NetworkDiscovery
{

    private float timeout = 5f;

    public static CustomNetworkDiscovery singleton;
    public static bool stopConfirmed = false;

    private Dictionary<LanConnectionInfo, float> lanAdresses = new Dictionary<LanConnectionInfo, float>();

    private void Awake()
    {
        base.Initialize();
        if (singleton != null && singleton != this)
            this.enabled = false;
        else
            singleton = this;
        //StartCoroutine(CleanupExpiredEntries());
        base.StartAsClient();
    }

    public void StartBroadcast()
    {
        StopBroadcast();
        base.Initialize();
        base.StartAsServer();
        NetworkLobbyManager.singleton.StartHost();
        //NetworkManager.singleton.StartHost();
    }

    public void StartListening()
    {
        //StopBroadcast();
        base.Initialize();
        base.StartAsClient();
    }

    public new void StopBroadcast()
    {
        if (running)
            base.StopBroadcast();
        ConfirmStopped();
    }

    private void ConfirmStopped()
    {
        try
        {
            stopConfirmed = !NetworkTransport.IsBroadcastDiscoveryRunning();
      
        } catch (UnityException e)
        {
            stopConfirmed = true;
        }
        
    }

    void LateUpdate()
    {
        if (!running && !stopConfirmed)
            ConfirmStopped();
    }

    //private IEnumerator CleanupExpiredEntries()
    //{
    //    while (true)
    //    {
    //        bool changed = false;

    //        var keys = lanAdresses.Keys.ToList();
    //        foreach (var key in keys)
    //        {
    //            if (lanAdresses[key] <= Time.time)
    //            {
    //                lanAdresses.Remove(key);
    //                changed = true;
    //            }
    //        }
    //        if (changed)
    //        {
    //            UpdateMatchInfos();
    //        }

    //        yield return new WaitForSeconds(timeout);
    //    }
    //}

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        print("IP: " + fromAddress + " data: " + data);


        //LanConnectionInfo info = new LanConnectionInfo(fromAddress, data);

        //if (lanAdresses.ContainsKey(info) == false)
        //{
        //    lanAdresses.Add(info, Time.time + timeout);
        //    //UpdateMatchInfos(); //Update UI
        //}
        //else
        //{
        //    lanAdresses[info] = Time.time + timeout;
        //}

        //NetworkManager.singleton.networkPort = info.port;
        //NetworkManager.singleton.networkAddress = fromAddress;//info.ipAdress;
        //NetworkManager.singleton.StartClient();
        NetworkLobbyManager.singleton.networkAddress = fromAddress;
        LobbyView.instance.JoinButton.SetActive(true);
        //NetworkLobbyManager.singleton.StartClient();

    }

    public void JoinShit()
    {
        NetworkLobbyManager.singleton.StartClient();
    }

    private void UpdateMatchInfos()
    {
        //GameListController.AddLanMatches(lanAdresses.Keys.ToList());
    }


}
