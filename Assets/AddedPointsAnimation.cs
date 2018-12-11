using System.Collections;
using TMPro;
using UnityEngine;

public class AddedPointsAnimation : MonoBehaviour {

    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private float _targetYOffset = 200f;
    [SerializeField]
    private float _duration = 1f;
    [SerializeField]
    private AnimationCurve _curve;

    private Vector2 _originalPosition;
    private Coroutine _currentAnimation;

    private void Start()
    {
        _text.enabled = false;
        _originalPosition = _rectTransform.anchoredPosition;
    }

    public void StartAnimation(int amountOfPoints)
    {
        if (_currentAnimation != null)
            StopCoroutine(_currentAnimation);
        _text.text = "+" + amountOfPoints.ToString();
        _currentAnimation = StartCoroutine(MoveAndFade());
    }
    
    private IEnumerator MoveAndFade()
    {
        _text.enabled = true;
        _rectTransform.anchoredPosition = _originalPosition;
        _text.SetTransparency(1);
        Vector2 targetPosition = new Vector2(_originalPosition.x, _originalPosition.y + _targetYOffset);
        float fullDistanceToTarget = Mathf.Abs(targetPosition.y - _originalPosition.y);
        float distanceToTarget = 0;
        float time = 0;
        float startTime = Time.time;

        while (_rectTransform.anchoredPosition != targetPosition)
        {
            distanceToTarget = Mathf.Abs(targetPosition.y - _rectTransform.anchoredPosition.y);

            _rectTransform.anchoredPosition = new Vector2(_originalPosition.x, _originalPosition.y + fullDistanceToTarget * _curve.Evaluate(time));
            time += Time.deltaTime / _duration;

            _text.SetTransparency(1 - _curve.Evaluate(time));

            yield return null;
        }

        _text.enabled = false;
        yield return 0;
    }
}
