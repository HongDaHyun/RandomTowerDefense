using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopUpText_UI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void PopUp_UI(string s, float endValue)
    {
        text.alpha = 1f;
        text.color = Color.white;

        gameObject.SetActive(true);
        text.text = s;

        text.rectTransform.DOAnchorPosY(endValue, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).SetDelay(0.1f).OnComplete(() => //0.5f -> moveDuration
        {
            text.DOFade(0f, 0.5f).SetUpdate(true) //0.5f -> moveDuration
            .OnComplete(() =>
            {
                text.gameObject.SetActive(false);
                text.rectTransform.anchoredPosition -= new Vector2(0, 200f);
            });
        });
    }
}
