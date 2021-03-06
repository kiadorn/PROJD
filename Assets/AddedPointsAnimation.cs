﻿using System.Collections;
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
    [SerializeField]
    private Vector2 targetPosition;

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
        Vector2 distanceWithVector;
        float time = 0;

        while (_rectTransform.anchoredPosition != targetPosition)
        {
            distanceWithVector = targetPosition - _originalPosition;
            _rectTransform.anchoredPosition = _originalPosition + distanceWithVector * _curve.Evaluate(time);
            time += Time.deltaTime / _duration;
            _text.SetTransparency(1 - _curve.Evaluate(time));
            yield return null;
        }

        _text.enabled = false;
        yield return 0;
    }
}
