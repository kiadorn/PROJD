using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public MenuButtonManager mbm;

    public void OnPointerEnter(PointerEventData eventData) {
        mbm.ShouldFade = false;
        mbm.moveStartTime = Time.time;
        SoundManager.instance.PlayButtonHover();
        StartCoroutine(mbm.ButtonSelectionMoveOut(mbm.moveStartTime, GetComponent<RectTransform>().anchoredPosition));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((gameObject.name == "Host Button!") || (gameObject.name == "Back Button!"))
        {
            SoundManager.instance.PlayButtonPress2();
        }
        else
            SoundManager.instance.PlayButtonPress();
    }

    public void OnPointerExit(PointerEventData eventData) {
        mbm.ShouldFade = true;
        mbm.FadeStartTime = Time.time;
        StartCoroutine(mbm.ButtonSelectionFadeOut(mbm.FadeStartTime));
    }
}
