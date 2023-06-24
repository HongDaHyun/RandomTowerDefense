using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Redcode.Pools;

public class CharacSlot : MonoBehaviour, IPoolObject
{
    public Image icon;
    public Text level;

    public void OnCreatedInPool()
    {

    }

    public void OnGettingFromPool()
    {

    }

    public void SetUI(CharacteristicCard card)
    {
        icon.sprite = card.cardPannel.cardIconImg.sprite;

        level.text = string.Format("Lv {0}", card.characStat.level + 1);
    }
}
