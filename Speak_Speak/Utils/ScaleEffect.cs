// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Project
using DG.Tweening;

public static class ScaleEffect
{
    public static void Show(Transform transform)
    {
        Sequence sequence = DOTween.Sequence();

        transform.gameObject.SetActive(true);

        sequence.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
        sequence.Append(transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f));

        sequence.Play();
    }

    public static void Hide(Transform transform)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(0.0f, 0.1f));

        sequence.Play().OnComplete(() =>
        {
            transform.localScale = Vector3.one;
            transform.gameObject.SetActive(false);
        });
    }

    public static void Show(Transform transform00, Transform transform01)
    {
        Sequence sequence = DOTween.Sequence();

        transform01.gameObject.SetActive(true);

        sequence.Append(transform00.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
        sequence.Append(transform00.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f));

        sequence.Play();
    }

    public static void Hide(Transform transform00, Transform transform01)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform00.DOScale(0.0f, 0.1f));

        sequence.Play().OnComplete(() =>
        {
            transform00.localScale = Vector3.one;
            transform01.gameObject.SetActive(false);
        });
    }
}
