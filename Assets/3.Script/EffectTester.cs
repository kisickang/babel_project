using UnityEngine;
using System.Collections;

public class EffectTester : MonoBehaviour
{
    [Header("Player FX References")]
    [Tooltip("플레이어 프리팹 내 FX_LevelUp 오브젝트")]
    [SerializeField] private GameObject fxLevelUp;
    [Tooltip("플레이어 프리팹 내 FX_SkillSelect 오브젝트")]
    [SerializeField] private GameObject fxSkillSelect;
    [Tooltip("플레이어 프리팹 내 FX_Resurrection 오브젝트")]
    [SerializeField] private GameObject fxResurrection;

    /// <summary>
    /// 레벨업 이펙트 재생
    /// </summary>
    public void PlayLevelUp() => StartCoroutine(PlayAndDeactivate(fxLevelUp));

    /// <summary>
    /// 스킬 선택 이펙트 재생
    /// </summary>
    public void PlaySkillSelect() => StartCoroutine(PlayAndDeactivate(fxSkillSelect));

    /// <summary>
    /// 부활 이펙트 재생
    /// </summary>
    public void PlayResurrection() => StartCoroutine(PlayAndDeactivate(fxResurrection));

    private IEnumerator PlayAndDeactivate(GameObject fx)
    {
        if (fx == null)
        {
            Debug.LogWarning($"EffectTester: FX {fx?.name} 이 할당되지 않았습니다!");
            yield break;
        }

        // 1) 켜기
        fx.SetActive(true);

        // 2) ParticleSystem이 있다면, 그 중 가장 긴 duration을 뽑아서 대기
        float wait = GetEffectDuration(fx);
        yield return new WaitForSeconds(wait);

        // 3) 끄기
        fx.SetActive(false);
    }

    /// <summary>
    /// FX 오브젝트 내 모든 ParticleSystem의 duration+delay 중 최대값을 반환.
    /// loop=true인 이펙트는 0으로 간주(수동 비활성화 필요).
    /// </summary>
    private float GetEffectDuration(GameObject fx)
    {
        var systems = fx.GetComponentsInChildren<ParticleSystem>();
        float max = 0f;
        foreach (var ps in systems)
        {
            var m = ps.main;
            if (m.loop) continue;
            float d = m.startDelay.constant + m.duration;
            if (d > max) max = d;
        }
        // ParticleSystem이 하나도 없거나, 모두 loop였으면 기본 1초
        return max > 0f ? max : 1f;
    }
}
