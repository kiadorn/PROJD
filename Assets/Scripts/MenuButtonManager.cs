using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class MenuButtonManager : MonoBehaviour {

    public GameObject ButtonSelection;
    public Image SelectionImage;
    [SerializeField] GameObject particles;
    [SerializeField] Image blockImage;
    RectTransform SelectionPos;
    float mouseCursorSpeed;

    [Header("Button Selection Move")]
    public float moveStartTime;
    [SerializeField] private AnimationCurve moveCurve;
    private float _moveLerpTime;

    [Header("Button Selection Fade")]
    public float FadeStartTime;
    [SerializeField] private AnimationCurve _fadeCurve;
    public bool ShouldFade = false;
    IEnumerator _coroutine;
    float _fadeLerpTime;

    private void Start() {
        _coroutine = ButtonSelectionFadeOut(FadeStartTime);
        SelectionPos = ButtonSelection.GetComponent<RectTransform>();
        //particles = GameObject.Find("Menu Particles");
        
    }

    private void Update() {
        mouseCursorSpeed = Mathf.Abs(Input.GetAxis("Mouse Y") / Time.deltaTime);
        //if(mouseCursorSpeed > 0) print(1 / (mouseCursorSpeed / 2));
    }

    public IEnumerator ButtonSelectionFadeOut(float startTime) {
        if(UIMenuSwap.transitioned == true) {


        }
        if(ShouldFade == true) {
            while(SelectionImage.color.a > 0 && ShouldFade == true) {
                _fadeLerpTime = Time.time - startTime;
                SelectionImage.color = new Color(1, 1, 1, _fadeCurve.Evaluate(_fadeLerpTime));
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        else {
            StopCoroutine(_coroutine);
        }
        yield return 0;
    }

    public IEnumerator ButtonSelectionMoveOut(float startTime, Vector2 targetPos) {
        if(UIMenuSwap.transitioned == true) {
            particles.SetActive(false);
            SelectionPos.anchoredPosition = new Vector2(0, targetPos.y);
            UIMenuSwap.transitioned = false;
            particles.SetActive(true);
            yield return 0;
        }
        if(SelectionImage.color.a < 1) SelectionImage.color = new Color(1, 1, 1, 1);
        StopCoroutine(_coroutine);
        moveCurve = AnimationCurve.EaseInOut(0, SelectionPos.anchoredPosition.y, Mathf.Clamp(1/(mouseCursorSpeed/2), 0.05f, 0.5f), targetPos.y);
        //print(Mathf.Clamp(1 / (mouseCursorSpeed / 2), 0, 1));
        //moveCurve.keys[0].value = SelectionPos.anchoredPosition.y;
        while(SelectionPos.anchoredPosition.y != targetPos.y) {
            _moveLerpTime = Time.time - startTime;
            SelectionPos.anchoredPosition = new Vector2(0, moveCurve.Evaluate(_moveLerpTime));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return 0;
    }

    
}
