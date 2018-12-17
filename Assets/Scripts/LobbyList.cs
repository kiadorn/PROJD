using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyList : MonoBehaviour
{

    public static LobbyList _instance = null;

    public RectTransform playerListContentTransform;
    //public GameObject warningDirectPlayServer;
    //public Transform addButtonRow;

    protected VerticalLayoutGroup _layout;
    public List<LobbyPlayer> _players = new List<LobbyPlayer>();
    public GameObject player2Model;

    public void OnEnable()
    {
        _instance = this;
        _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
    }

    //public void DisplayDirectServerWarning(bool enabled)
    //{
    //    if (warningDirectPlayServer != null)
    //        warningDirectPlayServer.SetActive(enabled);
    //}

    void Update()
    {
        //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
        //sometime to child being assigned before layout was enabled/init, leading to broken layouting)

        //if (_layout)
        //_layout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        if (_players.Contains(player))
            return;

        _players.Add(player);
        //if (_players.Count == 2)
        //    player2Model.SetActive(true);
        //else if (_players.Count != 2 || _players.Count > 2)
        //    player2Model.SetActive(false);

        player.transform.SetParent(CustomNetworkLobbyManager.singleton.transform, false);
        player.transform.GetChild(0).SetParent(playerListContentTransform, false);
        //addButtonRow.transform.SetAsLastSibling();

        // PlayerListModified();
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        _players.Remove(player);

        //if (_players.Count != 2 || _players.Count > 2)
        //    player2Model.SetActive(false);

        Destroy(player.gameObject);
        //  PlayerListModified();
    }

    //public void PlayerListModified()
    //{
    //    int i = 0;
    //    foreach (LobbyPlayer p in _players)
    //    {
    //        p.OnPlayerListChanged(i);
    //        ++i;
    //    }
    //}

}
