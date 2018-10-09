using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {


    public int ID;
    private StatsManager GameStats;

    private void Start() {
        GameStats = GameObject.Find("StatsManager").GetComponent<StatsManager>();
        ID = GameStats.GetPlayerID();
    }

    public void AddPoint() {
        GameStats.Addpoint(ID);
    }


}
