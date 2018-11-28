using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialColliderTrigger : MonoBehaviour {


    public enum TutorialRoom { Movement, Dash, Reset, Decoy };
    public TutorialRoom Room;
    TutorialProgress progress;
    public Material ClearedMaterial, ResetMaterial;
    public bool triggered = false;
    public bool playerPop;

    private void Start() {
        progress = GameObject.Find("Tutorial Manager").GetComponent<TutorialProgress>();
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Player") {
            if(!triggered) {
                triggered = true;

                if(Room == TutorialRoom.Movement) {
                    if(progress.MovementRoomProgress < 4) progress.MovementRoomProgress++;
                    GetComponent<MeshRenderer>().material = ClearedMaterial;
                }
                else if(Room == TutorialRoom.Dash) {
                    if(progress.DashRoomProgress < 3) progress.DashRoomProgress++;
                    GetComponent<MeshRenderer>().material = ClearedMaterial;
                }
                else if(Room == TutorialRoom.Decoy) {
                    if(progress.DecoyRoomProgress < 1) progress.DecoyRoomProgress++;
                    playerPop = true;
                    GetComponent<MeshRenderer>().material = ClearedMaterial;

                }
                else if(Room == TutorialRoom.Reset) {
                    triggered = false;
                    if(progress.DashRoomProgress < 3) {
                        progress.DashRoomProgress = 0;
                        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tutorial Platform")) {
                            go.GetComponent<MeshRenderer>().material = ResetMaterial;
                            go.GetComponent<TutorialColliderTrigger>().triggered = false;
                        }
                    }
                }
            }
        }

        if(collision.gameObject.tag == "Decoy") {
            if(playerPop) {
                if(progress.DecoyRoomProgress < 1) {
                    GetComponent<MeshRenderer>().material = ClearedMaterial;
                    progress.DashRoomProgress++;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Player") {
            if(Room == TutorialRoom.Decoy) {
                if(progress.progress < 5) {
                    playerPop = false;
                    GetComponent<MeshRenderer>().material = ResetMaterial;
                }
            }
        }
    }
}
