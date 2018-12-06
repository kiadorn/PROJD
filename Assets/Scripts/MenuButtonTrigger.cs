using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public MenuButtonManager mbm;


    public void OnPointerEnter(PointerEventData eventData) {
        mbm.ShouldFade = false;
        mbm.moveStartTime = Time.time;
        StartCoroutine(mbm.ButtonSelectionMoveOut(mbm.moveStartTime, GetComponent<RectTransform>().anchoredPosition));
    }

    public void OnPointerExit(PointerEventData eventData) {

        mbm.ShouldFade = true;
        mbm.FadeStartTime = Time.time;
        StartCoroutine(mbm.ButtonSelectionFadeOut(mbm.FadeStartTime));

    }

}
