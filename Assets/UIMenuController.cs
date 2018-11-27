using UnityEngine;

public class UIMenuController : MonoBehaviour {

    public GameObject[] views;
    

	// Use this for initialization
	public void SwitchViewTo(GameObject objectToEnable)
    {
        foreach(GameObject g in views)
        {
            g.SetActive(false);
        }
        objectToEnable.SetActive(true);
    }

    public void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void StartTutorial()
    {
        UnityEngine.Networking.NetworkManager.singleton.StartHost();
        UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene("Tutorial");
    }
}
