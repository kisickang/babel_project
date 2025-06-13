using UnityEngine;
using System.Collections;

public class RecoveryController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("PlayerStatus가 붙은 플레이어 게임오브젝트")]
    [SerializeField] private PlayerStatus playerStatus;
    
    [Header("HP Heal Settings")]
    [Tooltip("몇 초마다 HP 회복 (기본 3초)")]
    [SerializeField] private float hpInterval = 3f;
    [Tooltip("한 번에 회복할 HP 양 (기본 10)")]
    [SerializeField] private float hpAmount = 10f;
    [Tooltip("HP 회복 이펙트 오브젝트 (항상 활성화 상태)")]
    [SerializeField] private GameObject hpEffect;

    [Header("MP Heal Settings")]
    [Tooltip("몇 초마다 MP 회복 (기본 1초)")]
    [SerializeField] private float mpInterval = 1f;
    [Tooltip("한 번에 회복할 MP 양 (기본 5)")]
    [SerializeField] private float mpAmount = 5f;

    // HP 이펙트용
    private ParticleSystem[] hpPS;
    private float hpEffectDuration;

    private void Awake()
    {
        if (playerStatus == null)
            Debug.LogError("RecoveryController: PlayerStatus 할당 안됨!");

        // HP 이펙트 셋업
        if (hpEffect != null)
        {
            hpPS = hpEffect.GetComponentsInChildren<ParticleSystem>();
            hpEffectDuration = CalculateEffectDuration(hpPS);
            hpEffect.SetActive(true);
        }

        StartCoroutine(HPRecoveryLoop());
        StartCoroutine(MPRecoveryLoop());
    }

    private IEnumerator HPRecoveryLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(hpInterval);

            if (playerStatus.CurrentHP < playerStatus.MaxHP)
            {
                playerStatus.currentHP = Mathf.Min(playerStatus.CurrentHP + hpAmount, playerStatus.MaxHP);

                // HP 이펙트 재생
                if (hpPS != null)
                {
                    foreach (var ps in hpPS) ps.Play();
                    yield return new WaitForSeconds(hpEffectDuration);
                    foreach (var ps in hpPS) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
    }

    private IEnumerator MPRecoveryLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(mpInterval);

            if (playerStatus.CurrentMP < playerStatus.MaxMP)
            {
                // MP는 이펙트 없이 바로 회복
                playerStatus.currentMP = Mathf.Min(playerStatus.CurrentMP + mpAmount, playerStatus.MaxMP);
            }
        }
    }

    /// <summary>
    /// 여러 파티클 시스템 중 가장 긴 (startDelay + duration) 값을 구합니다.
    /// loop=true인 파티클은 무시합니다.
    /// </summary>
    private float CalculateEffectDuration(ParticleSystem[] systems)
    {
        float max = 0f;
        foreach (var ps in systems)
        {
            var m = ps.main;
            if (m.loop) continue;
            float delay = m.startDelay.constant;
            float dur   = m.duration;
            max = Mathf.Max(max, delay + dur);
        }
        return max > 0f ? max : 1f;
    }
}
