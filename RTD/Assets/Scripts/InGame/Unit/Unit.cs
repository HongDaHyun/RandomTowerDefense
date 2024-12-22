using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum UnitState { SearchTarget = 0, AttackToTarget }

public class Unit : MonoBehaviour, IPoolObject
{
    SpawnManager sm;
    CSVReader cr;
    SoundManager soundm;

    #region Attack
    private UnitState unitState = UnitState.SearchTarget;
    public Transform attackTarget = null;
    private bool isAtkCool;
    public float totalDmg;
    #endregion
    #region Animation
    [HideInInspector] public Animator anim;
    public bool isAnim; //공격 방식 animation or default
    private SpriteRenderer render;
    public Sprite defSprite;
    #endregion
    public UnitStat unitStat;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", ""); //(Clone) 삭제

        SetUp();

        defSprite = render.sprite;
        if (anim != null)
            isAnim = true;

        sm.SpawnTier(unitStat.tier, transform);
    }

    void IPoolObject.OnGettingFromPool()
    {
        StopAllCoroutines();
        SetStat();
        totalDmg = 0;

        render.sprite = defSprite;
        StartCoroutine(DelayTarget());
    }

    public void SetStat()
    {
        unitStat = cr.csvList.FindUnit(name);
    }

    void SetUp() // 컴포넌트
    {
        // 외부 컴포넌트
        sm = SpawnManager.Instance;
        cr = CSVReader.Instance;
        soundm = SoundManager.Instance;

        // 내부 컴포넌트
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
    }

    #region Attack
    public void ChangeState(UnitState newState)
    {
        StopCoroutine(unitState.ToString());
        unitState = newState;
        StartCoroutine(unitState.ToString());
    }

    private void RotateToTarget()
    {
        if (transform.position.x > attackTarget.position.x)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator DelayTarget()
    {
        yield return new WaitForSeconds(0.5f);
        ChangeState(UnitState.SearchTarget);
    }

    private IEnumerator SearchTarget()
    {
        isAtkCool = false;
        attackTarget = null;
        while(true)
        {
            List<Enemy> enemies = new List<Enemy>(sm.enemyList);

            float closetDistSqr = Mathf.Infinity;
            foreach(Enemy enemy in enemies)
            {
                float distance = Vector2.Distance(enemy.transform.position, transform.position);
                if (distance <= unitStat.range && distance <= closetDistSqr)
                {
                    closetDistSqr = distance;
                    attackTarget = enemy.transform;
                }
            }

            if (attackTarget != null && attackTarget.gameObject.activeSelf)
            {
                ChangeState(UnitState.AttackToTarget);
                break;
            }
            yield return null;
        }
    }

    protected virtual IEnumerator AttackToTarget()
    {
        if(!isAtkCool)
        {
            isAtkCool = true;
            RotateToTarget(); // 방향 설정

            if (isAnim)
            {
                anim.SetTrigger("Atk");

                // 공격 애니메이션 종료 대기
                yield return new WaitUntil(() =>
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Atk"));
                yield return new WaitUntil(() =>
                    anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

                StartCoroutine(AtkCoolAnim());
            }
            else
            {
                Shoot();

                yield return new WaitForSeconds(1f / unitStat.atkSpeed);
                ChangeState(UnitState.SearchTarget);
            }
        }
    }

    // 애니메이션에서 작동
    IEnumerator AtkCoolAnim()
    {
        yield return new WaitForSeconds(1f / unitStat.atkSpeed);
        ChangeState(UnitState.SearchTarget);
    }

    Projectile Shoot()
    {
        if (attackTarget == null || !attackTarget.gameObject.activeSelf)
            return null;

        Projectile projectile = sm.SpawnProjectile(this, unitStat.unitType.ToString());

        projectile.SetStat(unitStat); // 투사체 공격력, 사거리 세팅

        soundm.PlaySFX($"Shoot{projectile.projectileStat.projectileName}"); // 효과음

        return projectile;
    }
    #endregion
}
