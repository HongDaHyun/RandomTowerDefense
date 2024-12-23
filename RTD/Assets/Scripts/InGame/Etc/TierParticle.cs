using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierParticle : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(LifeDuration());
    }

    IEnumerator LifeDuration()
    {
        ps.Play();
        yield return new WaitForSeconds(ps.main.duration);
        ps.Pause();
    }
}
