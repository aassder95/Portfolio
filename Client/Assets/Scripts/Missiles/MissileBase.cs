using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MissileBase : MonoBehaviour, IPoolable
{
    #region Interface
    public int PoolCnt => 20;
    public GameObject GameObject => gameObject;
    #endregion
    #region Fields
    FishBase m_attacker;
    EnemyBase m_target;
    IDisposable m_onDestroyedSub;
    #endregion
    #region Unity
    void Update()
    {
        if (Managers.Stage.IsGameOver)
            return;

        if (m_target == null)
        {
            Managers.Resource.Destroy(gameObject);
            return;
        }

        MoveTowardsTarget();
    }
    #endregion
    #region Init
    public void Activate(FishBase attacker, EnemyBase target)
    {
        m_attacker = attacker;
        m_target = target;
        m_onDestroyedSub = m_target.DamageCtrl.OnDestroyed.Subscribe(_ => OnTargetDestroyed());
    }

    public void Deactivate()
    {
        m_attacker = null;
        m_target = null;
        m_onDestroyedSub?.Dispose();
    }
    #endregion
    #region Action
    void MoveTowardsTarget()
    {
        float moveDist = m_attacker.Template.Value.MissileMoveSpeed * Time.deltaTime;

        if (moveDist >= Vector3.Distance(transform.position, m_target.transform.position))
        {
            m_target.DamageCtrl.TakeDamage(m_attacker);
            Managers.Resource.Destroy(gameObject);
        }
        else
        {
            Vector3 dir = (m_target.transform.position - transform.position).normalized;
            transform.position += dir * moveDist;
        }
    }
    #endregion
    #region Callback
    void OnTargetDestroyed()
    {
        Managers.Resource.Destroy(gameObject);
    }
    #endregion
}
