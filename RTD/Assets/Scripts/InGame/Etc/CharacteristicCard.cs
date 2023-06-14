using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacteristicCard : MonoBehaviour
{
    public CharacteristicStat characStat;
    public CardPannel cardPannel;

    public void SetCard(int id)
    {
        characStat = CSVReader.Instance.CharacteristicExport(id);

        cardPannel.cardNameTxt.text = characStat.charateristicName;
        cardPannel.explanationTxt.text = string.Format(characStat.explanation, characStat.condition);

        cardPannel.cardIconImg.sprite = SpriteData.Instance.CharacIcon[id];

        SetStar(characStat.level);
    }

    private void SetStar(int count)
    {
        Sprite[] starSprites = SpriteData.Instance.Stars;

        if (count > 4) // 4레벨 이상 (보라색 스타)
        {
            DrawStar(4, starSprites[0]); // 노란색 스타
            DrawStar(count - 4, starSprites[1]); // 보라색 스타
        }

        else // 4레벨 이하 (노란색 스타)
        {
            DrawStar(4, starSprites[2]); // 빈 스타
            DrawStar(count, starSprites[0]); // 노란색 스타
        }
    }

    private void DrawStar(int count, Sprite _sprite)
    {
        for (int i = 0; i < count; i++)
            cardPannel.starImg[i].sprite = _sprite;
    }

    public void ClickCard()
    {
        CharacLevelUp(ref CSVReader.Instance.csvList.characteristicStat[characStat.ID]);

        UIManager.Instance.characteristicPannel.SetActive(false);
        BtnManager.Instance.Play();
    }

    public void CharacLevelUp(ref CharacteristicStat stat)
    {
        stat.level++;
        stat.condition += stat.conditionWidth;
    }
}
