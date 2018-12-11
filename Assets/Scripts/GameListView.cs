using UnityEngine;
using UnityEngine.UI;

public class GameListView : MonoBehaviour {

    public static GameListView instance;

    public GameObject LobbyBar;
    public Button Joinbutton;

    public GameSelection selectedGame;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }
}
