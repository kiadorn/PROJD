using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DashType : ScriptableObject {

    public float dashDuration;
    public float dashForce;
    public float dashCooldown;

    public abstract void DashForce();

}
