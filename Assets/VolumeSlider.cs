using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    public FloatVariable volume;

    public void Start()
    {
        GetComponent<Slider>().value = volume.Value;
    }
}
