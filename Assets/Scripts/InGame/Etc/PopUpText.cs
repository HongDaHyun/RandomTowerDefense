using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Redcode.Pools;
using TMPro;

public class PopUpText : MonoBehaviour, IPoolObject
{
    const float DEFAULT_LIFE = 1.5F;

    SpawnManager sm;
    Enemy parentEnemy;
    [HideInInspector] public bool isDestroy;
    private TextMeshPro meshPro;
    [HideInInspector] public float lifeTime;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        meshPro = GetComponent<TextMeshPro>();
        sm = SpawnManager.Instance;
    }

    public void OnGettingFromPool()
    {
        isDestroy = false;
        meshPro.alpha = 1f;
        meshPro.color = Color.white;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
            Destroy();
    }

    public void PopUp_DMG_New(Enemy enemy, float s, bool isCrit)
    {
        parentEnemy = enemy;
        transform.SetParent(enemy.transform);
        transform.localPosition = Vector3.zero;

        meshPro.color = isCrit ? Color.yellow : Color.white;
        meshPro.text = s.ToString("F1");

        lifeTime = DEFAULT_LIFE;

        meshPro.transform.DOLocalMoveY(1f, 0.5f).SetEase(Ease.OutQuad);
    }
    public void PopUp_DMG_Remain(float s, bool isCrit)
    {
        meshPro.color = isCrit ? Color.yellow : Color.white;
        lifeTime = DEFAULT_LIFE;
        s += float.Parse(meshPro.text);
        meshPro.text = s.ToString("F1");
    }
    private void Destroy()
    {
        if (isDestroy)
            return;

        parentEnemy.popUpText = null;
        transform.SetParent(null);
        isDestroy = true;

        meshPro.DOFade(0f, 0.5f).SetUpdate(true)
            .OnComplete(() => sm.DeSpawnDmgText(this));
    }
}
