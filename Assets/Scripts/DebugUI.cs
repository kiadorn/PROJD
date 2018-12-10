using UnityEngine;
using TMPro;
public class DebugUI : MonoBehaviour {

    public TextMeshProUGUI ghhghgh;
   
	
	// Update is called once per frame
	void Update () {
        if (RoundManager.instance.tutorialActive)
        {
            ghhghgh.text = "is Active!";
        } else
        {
            ghhghgh.text = "no";
        }

    }
}
