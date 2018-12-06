public class MainMenuController : MenuController {

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
        UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene("Tutorial 1");
    }

}
