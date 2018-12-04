using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuSwap : MonoBehaviour {

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
    float startTime;
    float lerpTime;
    RectTransform rt;

    public IEnumerator TransitionOut() {

        print(gameObject.name + " transitioning out");

        transform.Find("Behehe").GetComponent<Image>().enabled = true;
        startTime = Time.time;

        while(rt.anchoredPosition.y < outCurve.keys[1].value) {
            lerpTime = Time.time - startTime;

            rt.anchoredPosition = new Vector2(0,outCurve.Evaluate(lerpTime));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return 0;
    }

    public IEnumerator TransitionIn() {

        print(gameObject.name + " transitioning in");

        startTime = Time.time;
        if(rt.anchoredPosition.y != inCurve.keys[0].value) rt.anchoredPosition = new Vector2(0, inCurve.keys[0].value);


        while(rt.anchoredPosition.y < inCurve.keys[1].value) {
            lerpTime = Time.time - startTime;

            rt.anchoredPosition = new Vector2(0, inCurve.Evaluate(lerpTime));
            yield return new WaitForSeconds(Time.deltaTime);
        }

        transform.Find("Behehe").GetComponent<Image>().enabled = false;
        yield return 0;
    }

    public void TransitionOutActivate() {
        StartCoroutine(TransitionOut());
    }

    public void TransitionInActivate() {
        StartCoroutine(TransitionIn());
    }


    void Start () {
        rt = GetComponent<RectTransform>();
	}

	void Update () {
		
	}
}
