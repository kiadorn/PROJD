using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkDiscovery : NetworkDiscovery {

    private float timeout = 5f;

    private Dictionary<LanConnectionInfo, float> lanAdresses = new Dictionary<LanConnectionInfo, float>();

    private void Awake() {
        base.Initialize();
        //base.StartAsClient();
        //StartCoroutine(CleanupExpiredEntries());
    }

    public void StartBroadcast() {
        base.Initialize();
        //StopBroadcast();
        base.StartAsServer();
        NetworkManager.singleton.StartHost();
    }

    public void StartListening()
    {
        //StopBroadcast();
        base.Initialize();
        base.StartAsClient();
    }

    //private IEnumerator CleanupExpiredEntries() {
    //    while (true) {
    //        bool changed = false;

    //        var keys = lanAdresses.Keys.ToList();
    //        foreach(var key in keys) {
    //            if(lanAdresses[key] <= Time.time) {
    //                lanAdresses.Remove(key);
    //                changed = true;
    //            }
    //        }
    //        if (changed) {
    //            UpdateMatchInfos();
    //        }

    //        yield return new WaitForSeconds(timeout);
    //    }
    //}

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        print("IP: " + fromAddress + " data: " + data);
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();

        //base.OnReceivedBroadcast(fromAddress, data);

        //LanConnectionInfo info = new LanConnectionInfo(fromAddress, data);

        //if(lanAdresses.ContainsKey(info) == false) {
        //    lanAdresses.Add(info, Time.time + timeout);
        //    UpdateMatchInfos(); //Update UI
            
        //}
        //else {
        //    lanAdresses[info] = Time.time + timeout;
        //}
    }

    private void UpdateMatchInfos() {
        //GameListController.AddLanMatches(lanAdresses.Keys.ToList());
    }

    
}
