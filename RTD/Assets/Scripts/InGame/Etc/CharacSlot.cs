using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacSlot : MonoBehaviour, IPointerClickHandler
{
    public bool isShow;
    public Image icon;
    public Text level;

    public string s;

    public void OnPointerClick(PointerEventData eventData)
    {
        SpawnManager.Instance.SpawnCharacBubble(UIManager.Instance.characInfo.pannel, this);
    }

    public void SetUI(CharacteristicCard card)
    {
        icon.sprite = card.cardPannel.cardIconImg.sprite;

        level.text = card.characStat.level < 7 ? string.Format("Lv {0}", card.characStat.level + 1) : "MAX";

        s = string.Format(card.characStat.explanation, card.characStat.condition);
    }
}
