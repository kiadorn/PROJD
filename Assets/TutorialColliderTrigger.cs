using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialColliderTrigger : MonoBehaviour {


    public enum TutorialRoom { Movement, Dash, Reset, Decoy };
    public TutorialRoom Room;
    TutorialProgress progress;
    public Material ClearedMaterial, ResetMaterial;
    public bool triggered = false;

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
                if(Room == TutorialRoom.Decoy) {
                    if(transform.name != "DecoyPlatform")
                    GetComponent<MeshRenderer>().material = ClearedMaterial;
                }

                else if(Room == TutorialRoom.Reset) {
                    triggered = false;
                    if(progress.DashRoomProgress < 3) {
                        progress.DashRoomProgress = 0;
                        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tutorial Platform")) {
                            if(go.GetComponent<TutorialColliderTrigger>().Room == TutorialRoom.Dash) {
                                go.GetComponent<MeshRenderer>().material = ResetMaterial;
                                go.GetComponent<TutorialColliderTrigger>().triggered = false;
                            }
                        }
                    }
                    if(progress.DecoyRoomProgress < 1) {
                        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tutorial Platform")) {
                            if(go.GetComponent<TutorialColliderTrigger>().Room == TutorialRoom.Decoy) {
                                go.GetComponent<MeshRenderer>().material = ResetMaterial;
                                go.GetComponent<TutorialColliderTrigger>().triggered = false;
                            }
                        }
                    }
                }
            }
        }
        else if(collision.gameObject.tag == "Decoy") {
            if(transform.name == "DecoyPlatform") {
                if(GameObject.Find("PlayerPlatform").GetComponent<TutorialColliderTrigger>().triggered == true) {
                    GetComponent<MeshRenderer>().material = ClearedMaterial;
                    triggered = true;
                    if(progress.DecoyRoomProgress < 1) progress.DecoyRoomProgress++;
                }
            }   
        }
    }
}
