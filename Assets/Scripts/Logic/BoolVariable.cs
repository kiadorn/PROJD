using System;
using UnityEngine;

[CreateAssetMenu]
public class BoolVariable : ScriptableObject {

    public bool Value;

    public static implicit operator bool(BoolVariable reference) {
        return reference.Value;
    }

    public void SetValue(bool value) {
        Value = value;
    }


}
