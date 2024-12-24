using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CharacSlot : MonoBehaviour, IPointerClickHandler
{
    public bool isShow;
    public Image pannelImg;
    public Image icon;
    public TextMeshProUGUI level;

    public string s;

    public void OnPointerClick(PointerEventData eventData)
    {
        SpawnManager.Instance.SpawnCharacBubble(UIManager.Instance.characInfo.pannel, this);
    }

    public void SetUI(CharacteristicCard card)
    {
        icon.sprite = card.cardPannel.cardIconImg.sprite;

        if(!card.isMax) // ¸¸·¾ Àü
        {
            s = string.Format(card.characStat.explanation, card.characStat.condition);
            level.text = string.Format("Lv {0}", card.characStat.level + 1);
        }

        else // ¸¸·¾
        {
            level.text = "MAX";
            level.color = Color.cyan;
            pannelImg.color = Color.cyan;
            s += "\n\n" + card.characStat.maxExplanation;
        }
    }
}
