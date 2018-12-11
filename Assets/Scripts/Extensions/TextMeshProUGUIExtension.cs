using UnityEngine;
using TMPro;

public static class TextMeshProUGUIExtension {

	public static void SetTransparency(this TextMeshProUGUI text, float transparency) {
        if (text != null) {
            Color __alpha = text.color;
            __alpha.a = transparency;
            text.color = __alpha;
        }
    }
}
