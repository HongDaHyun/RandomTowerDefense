using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPace { Defend, Heal, Run }

public class Enemy_Boss : Enemy
{
    public BossPace pace;
    public SpriteRenderer iconSprite;

    protected override void Die()
    {
        base.Die();

        WaveManager.Instance.BossWaveEnd();
    }

    public override void OnGettingFromPool()
    {
        base.OnGettingFromPool();

        StartCoroutine(Activation(8f)); // 8초 주기로 페이스 변경
    }

    private IEnumerator Activation(float duration)
    {
        yield return new WaitForSeconds(duration);


        pace = RanPace();

        switch(pace)
        {
            case BossPace.Defend:
                enemyStat.defense += 50;
                enemyStat.strength += 50;
                break;
            case BossPace.Heal:
                curHp += (enemyStat.hp - curHp) / 5f;
                break;
            case BossPace.Run:
                enemyStat.moveSpeed += 0.2f;
                break;
        }

        UIManager.Instance.info.UpdateEnemyUI();

        iconSprite.sprite = SpriteData.Instance.bossIcon[(int)pace];
        anim.SetTrigger("Activation");

        StartCoroutine(Activation(duration));
    }

    private BossPace RanPace()
    {
        return (BossPace)Random.Range(0, System.Enum.GetValues(typeof(BossPace)).Length);
    }
}
