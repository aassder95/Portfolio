using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TargetController
{
    #region Fields
    FishBase m_fish;
    EnemyBase m_target;
    IDisposable m_onDestroyedSub;
    #endregion
    #region Properties
    public EnemyBase Target => m_target;
    #endregion
    #region Init
    public TargetController(FishBase fish)
    {
        m_fish = fish;
    }

    public void Update()
    {
        RefreshTarget();
    }

    public void Clear()
    {
        m_target = null;
        m_onDestroyedSub?.Dispose();
    }
    #endregion
    #region Action
    void RefreshTarget()
    {
        EnemyBase newTarget = FindTarget();
        if (newTarget == m_target)
            return;

        m_target = newTarget;

        m_onDestroyedSub?.Dispose();
        m_onDestroyedSub = m_target?.DamageCtrl.OnDestroyed.Subscribe(_ => OnTargetDestroyed());
    }
    #endregion
    #region Utility
    EnemyBase FindTarget()
    {
        if (IsTargetInRange(Managers.Enemy.Target))
            return Managers.Enemy.Target;

        if (IsTargetInRange(m_target))
            return m_target;

        return FindBehindTarget();
    }

    EnemyBase FindBehindTarget()
    {
        EnemyBase target = null;
        foreach (EnemyBase enemy in Managers.Enemy.Characters)
        {
            if (!IsTargetInRange(enemy) || enemy.MovementCtrl.CurWaypoint == -1)
                continue;

            if (IsBehindTarget(target, enemy))
                target = enemy;
        }

        return target;
    }
    #endregion
    #region Is
    bool IsTargetInRange(EnemyBase enemy)
    {
        return enemy != null && Vector3.Distance(m_fish.transform.position, enemy.transform.position) <= m_fish.Template.Value.AttRange;
    }

    bool IsBehindTarget(EnemyBase target, EnemyBase enemy)
    {
        if (target == null)
            return true;

        switch (Mathf.Abs(target.MovementCtrl.CurWaypoint - enemy.MovementCtrl.CurWaypoint))
        {
            case 0:
                return target.MovementCtrl.GetProgressToNextWaypoint() > enemy.MovementCtrl.GetProgressToNextWaypoint();
            case 1:
            case 2:
                return target.MovementCtrl.CurWaypoint > enemy.MovementCtrl.CurWaypoint;
            case 3:
                return target.MovementCtrl.CurWaypoint < enemy.MovementCtrl.CurWaypoint;
        }

        return false;
    }
    #endregion
    #region Callback
    void OnTargetDestroyed()
    {
        m_target = null;
    }
    #endregion
}
