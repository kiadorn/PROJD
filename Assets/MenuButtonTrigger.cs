using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public MenuButtonManager mbm;


    public void OnPointerEnter(PointerEventData eventData) {
        mbm.shouldFade = false;
        mbm.moveStartTime = Time.time;
        StartCoroutine(mbm.MoveOut(mbm.moveStartTime, GetComponent<RectTransform>().anchoredPosition));
    }

    public void OnPointerExit(PointerEventData eventData) {

        mbm.shouldFade = true;
        mbm.fadeStartTime = Time.time;
        StartCoroutine(mbm.FadeOut(mbm.fadeStartTime));

    }

}
