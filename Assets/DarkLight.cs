#if UNITY_EDITOR
using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class DarkLight : MonoBehaviour
{
    public Vector4 hackColor;
    public float multiplier = 1;

    void Update()
    {
        
        GetComponent<Light>().color = new Color(hackColor.x, hackColor.y, hackColor.z, hackColor.w) * multiplier;
    }
}
#endif
