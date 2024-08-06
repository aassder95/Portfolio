using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController
{
    #region Fields
    float m_attCooldown = 0.0f;
    FishBase m_fish;
    #endregion
    #region Init
    public MissileController(FishBase fish)
    {
        m_fish = fish;
    }

    public void Update()
    {
        AttackTarget();
    }

    public void Clear()
    {
        m_attCooldown = 0.0f;
    }
    #endregion
    #region Action
    void AttackTarget()
    {
        if (m_attCooldown > 0.0f)
        {
            m_attCooldown -= Time.deltaTime;
        }
        else if (IsTargetInRange())
        {
            LaunchMissile();
            m_attCooldown = 1.0f / m_fish.Template.Value.AttSpeed;
        }
    }

    void LaunchMissile()
    {
        m_fish.StartCoroutine(LaunchMissilesCoroutine());
    }
    #endregion
    #region Coroutines
    IEnumerator LaunchMissilesCoroutine()
    {
        EnemyBase target = m_fish.TargetCtrl.Target;

        string path = $"Missile/{m_fish.Template.Value.MissileName}";

        for (int i = 0; i < m_fish.Template.Value.MissileCount; i++)
        {
            if (target.gameObject.activeSelf == false)
                yield break;

            GameObject go = Managers.Resource.Instantiate(path);
            go.transform.position = m_fish.transform.position;

            MissileBase missile = go.GetOrAddComponent<MissileBase>();
            missile.Activate(m_fish, target);

            yield return new WaitForSeconds(m_fish.Template.Value.MissileInterval);
        }
    }
    #endregion
    #region Is
    bool IsTargetInRange()
    {
        EnemyBase target = m_fish.TargetCtrl.Target;

        return target != null && Vector3.Distance(m_fish.transform.position, target.transform.position) <= m_fish.Template.Value.AttRange;
    }
    #endregion
}
