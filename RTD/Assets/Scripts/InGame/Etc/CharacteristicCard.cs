using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacteristicCard : MonoBehaviour
{
    public CharacteristicStat characStat;
    public CardPannel cardPannel;
    public bool isMax;

    public void SetCard(int id)
    {
        characStat = CSVReader.Instance.csvList.FindCharac(id);

        cardPannel.cardNameTxt.text = characStat.charateristicName;

        if(characStat.level < 7) // 만렙 전
        {
            isMax = false;

            cardPannel.explanationTxt.text = string.Format(characStat.explanation, characStat.condition);
            cardPannel.cardIconImg.sprite = SpriteData.Instance.CharacIcon[id];
            cardPannel.cardImg.color = Color.white;
        }
        else
        {
            isMax = true;

            cardPannel.explanationTxt.text = characStat.maxExplanation;
            cardPannel.cardIconImg.sprite = SpriteData.Instance.CharacMaxIcon[id];

            Color color;
            ColorUtility.TryParseHtmlString("#FFECAF", out color);
            cardPannel.cardImg.color = color;
        }

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
        CharacManager cm = CharacManager.Instance;

        if (cm.isLevelUping)
            return;

        BtnManager bm = BtnManager.Instance;

        bm.ClickSound(); // Sound

        cm.isLevelUping = true;

        CharacLevelUp(ref CSVReader.Instance.csvList.characteristicStat[characStat.ID]); // 레벨 업

        if (!isMax) // 만렙 전
            cm.DoCharac(characStat);
        else // 만렙
            cm.DoCharac_Max(characStat);

        UIManager.Instance.characInfo.CharacInfoPannel(this); // 내 정보 업데이트

        //UI
        Sequence seq = DOTween.Sequence().SetUpdate(true);

        seq.Append(cardPannel.starImg[characStat.level % 4].DOFade(0f, 0.1f).OnComplete(() => {
            cardPannel.starImg[characStat.level % 4].sprite = characStat.level + 1 > 4 ? SpriteData.Instance.Stars[1] : SpriteData.Instance.Stars[0];
        }));
        seq.Append(cardPannel.starImg[characStat.level % 4].DOFade(1f, 0.1f).OnComplete(() => {
            bm.Tab(UIManager.Instance.characCardPannel.pannel);
            bm.Play();
            cm.isLevelUping = false;
        }));
    }

    public void CharacLevelUp(ref CharacteristicStat stat)
    {
        stat.level++;
        stat.condition += stat.conditionWidth;
    }
}
