using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Redcode.Pools;
using TMPro;

public class PopUpText : MonoBehaviour, IPoolObject
{
    const float DEFAULT_LIFE = 1.5F;
    Vector3 originalScale;

    SpawnManager sm;
    Enemy parentEnemy;
    [HideInInspector] public bool isDestroy;
    private TextMeshPro meshPro;
    [HideInInspector] public float lifeTime;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        originalScale = transform.localScale;
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
        if (!isDestroy)
        {
            lifeTime -= Time.deltaTime;

            transform.localScale = new Vector3(
                        originalScale.x / parentEnemy.transform.lossyScale.x,
                        originalScale.y / parentEnemy.transform.lossyScale.y,
                        originalScale.z / parentEnemy.transform.lossyScale.z);

            if (lifeTime <= 0)
                Destroy();
        }
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
    public void Destroy()
    {
        parentEnemy.popUpText = null;
        transform.SetParent(null);
        isDestroy = true;

        meshPro.DOFade(0f, 1f).SetUpdate(true)
            .OnComplete(() => sm.DeSpawnDmgText(this));
    }
}
