using UnityEngine;

public static class ImageAlphaExtention{

    public static void SetTransparency(this UnityEngine.UI.Image p_image, float p_transparency) {
        if (p_image != null) {
            Color __alpha = p_image.color;
            __alpha.a = p_transparency;
            p_image.color = __alpha;
        }
    }
}
