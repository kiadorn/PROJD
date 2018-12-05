using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuSwap : MonoBehaviour {

    public RectTransform selection;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
    public RectTransform currentElement;
    float tempY;
    float startTime;
    float lerpTime;
    RectTransform rt;
    public static bool transitioned = false;
    int tempInt;
    public static RectTransform[] Menues = new RectTransform[4];

    public IEnumerator TransitionOut() {

        transitioned = true;

        print(gameObject.name + " transitioning out");


        startTime = Time.time;
        tempY = selection.anchoredPosition.y;
        while(rt.anchoredPosition.y < outCurve.keys[1].value) {
            lerpTime = Time.time - startTime;
            selection.anchoredPosition = new Vector2(0, outCurve.Evaluate(lerpTime)+tempY);
            rt.anchoredPosition = new Vector2(0,outCurve.Evaluate(lerpTime));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return 0;
    }

    public IEnumerator TransitionIn() {

        currentElement = rt;

        print(gameObject.name + " transitioning in");

        startTime = Time.time;
        if(rt.anchoredPosition.y != inCurve.keys[0].value) rt.anchoredPosition = new Vector2(0, inCurve.keys[0].value);


        while(rt.anchoredPosition.y < inCurve.keys[1].value) {
            lerpTime = Time.time - startTime;

            rt.anchoredPosition = new Vector2(0, inCurve.Evaluate(lerpTime));
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if((Menues[0].anchoredPosition.y > 0 && Menues[1].anchoredPosition.y > 0) || (Menues[1].anchoredPosition.y > 0 && Menues[2].anchoredPosition.y > 0) || (Menues[0].anchoredPosition.y > 0 && Menues[2].anchoredPosition.y > 0)) {

            currentElement.anchoredPosition = new Vector2(0, 0);

        }

        transform.Find("Behehe").GetComponent<Image>().enabled = false;
        yield return 0;
    }

    public void TransitionOutActivate() {
        transform.Find("Behehe").GetComponent<Image>().enabled = true;
        StartCoroutine(TransitionOut());
    }

    public void TransitionInActivate() {
        StartCoroutine(TransitionIn());
    }


    void Start () {
        rt = GetComponent<RectTransform>();
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Menu Screen")) {
            Menues[tempInt] = go.GetComponent<RectTransform>();
            tempInt++;
        }
    }

	void Update () {
        
		
	}
}
