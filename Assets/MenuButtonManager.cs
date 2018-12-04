using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour {

    public GameObject ButtonSelection;
    public Image SelectionImage;
    RectTransform SelectionPos;
    float mouseCursorSpeed;

    [Header("Move")]
    public Vector2 targetPos;
    public float moveStartTime;
    public AnimationCurve moveCurve;
    float moveLerpTime;
    bool moving;

    [Header("Fade")]
    IEnumerator coroutine;
    public float fadeStartTime;
    public AnimationCurve fadeCurve;
    public float fadeOutTimer = 3;
    public bool shouldFade = false;
    float fadeLerpTime;

    private void Start() {
        coroutine = FadeOut(fadeStartTime);
        SelectionPos = ButtonSelection.GetComponent<RectTransform>();
    }

    private void Update() {
        mouseCursorSpeed = Mathf.Abs(Input.GetAxis("Mouse Y") / Time.deltaTime);
        //if(mouseCursorSpeed > 0) print(1 / (mouseCursorSpeed / 2));
    }

    public IEnumerator FadeOut(float startTime) {
        
        if(shouldFade == true) {
            while(SelectionImage.color.a > 0 && shouldFade == true) {
                fadeLerpTime = Time.time - startTime;
                SelectionImage.color = new Color(1, 1, 1, fadeCurve.Evaluate(fadeLerpTime));
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        else {
            StopCoroutine(coroutine);
        }
        yield return 0;
    }

    public IEnumerator MoveOut(float startTime, Vector2 targetPos) {
        if(SelectionImage.color.a > 1) SelectionImage.color = new Color(1, 1, 1, 1);
        StopCoroutine(coroutine);
        moveCurve = AnimationCurve.EaseInOut(0, SelectionPos.anchoredPosition.y, 1/(mouseCursorSpeed/2), targetPos.y);
        //moveCurve.keys[0].value = SelectionPos.anchoredPosition.y;
        //moveCurve.keys[1].value = targetPos.y;
        while(SelectionPos.anchoredPosition.y != targetPos.y) {
            moveLerpTime = Time.time - startTime;
            SelectionPos.anchoredPosition = new Vector2(0, moveCurve.Evaluate(moveLerpTime));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return 0;
    }

    public Vector3 delta = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;
    
}
