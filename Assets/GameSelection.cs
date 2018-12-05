using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelection : MonoBehaviour {

    public static Transform game;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectGame() {
        DeselectGame();
        game = transform.Find("Game BG");
        game.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
    }

    public void DeselectGame() {
        if(game != null) game.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
    }
}
