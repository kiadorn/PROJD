using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LanConnectionInfo{

    public string ipAdress;
    public int port;
    public string name;

	public LanConnectionInfo(string fromAdress, string data) {
        ipAdress = fromAdress.Substring(fromAdress.LastIndexOf(":") + 1, fromAdress.Length - (fromAdress.LastIndexOf(":") + 1));
        string portText = data.Substring(data.LastIndexOf(":") + 1, data.Length - (data.LastIndexOf(":") + 1));
        port = 7777;
        int.TryParse(portText, out port);
        name = "local";
    }
}
