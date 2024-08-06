using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DamageController
{
    #region Fields
    EnemyBase m_enemy;
    ReactiveProperty<int> m_hp = new ReactiveProperty<int>();
    Subject<Unit> m_onDestroyed;
    #endregion
    #region Properties
    public IReadOnlyReactiveProperty<int> Hp => m_hp;
    public IObservable<Unit> OnDestroyed => m_onDestroyed;
    #endregion 
    #region Init
    public DamageController(EnemyBase enemy)
    {
        m_enemy = enemy;
    }

    public void Init()
    {
        m_hp.Value = CalculateHp();
        m_onDestroyed = new Subject<Unit>();
    }

    public void Clear()
    {
        m_hp.Value = 0;
        m_onDestroyed?.Dispose();
    }
    #endregion
    #region Action
    public void TakeDamage(FishBase attacker)
    {
        bool isCri = IsCriticalHit(attacker);
        int damage = CalcDamage(attacker, isCri);

        m_hp.Value = Math.Max(m_hp.Value - damage, 0);

        if (damage > 0)
        {
            Define.UICombatText combatTextType = isCri ? Define.UICombatText.CriDamage : Define.UICombatText.AttDamage;
            Managers.UI.ShowCombatText(combatTextType, m_enemy.transform.position, damage);
            Managers.Sound.Play(Define.Sound.Sfx, "Hit");
            if (Managers.Data.Setting.IsEffect.Value)
            {
                GameObject particle = Managers.Resource.Instantiate("Particle/Hit");
                particle.transform.position = m_enemy.transform.position;
            }
        }
        if (m_hp.Value <= 0)
            Die();
    }

    void Die()
    {
        Managers.Game.IncreasePearl(m_enemy.Template.Pearl);
        Managers.UI.ShowCombatText(Define.UICombatText.Pearl, m_enemy.transform.position, m_enemy.Template.Pearl);
        m_onDestroyed.OnNext(Unit.Default);
        Managers.Enemy.Despawn(m_enemy);
    }
    #endregion
    #region Utility
    int CalcDamage(FishBase attacker, bool isCri)
    {
        InGameCardData attackerData = Managers.Card.GetInGameCardData(attacker.Template.Value);

        int damage = UnityEngine.Random.Range(attackerData.DamageMin, attackerData.DamageMax);

        if (isCri)
            damage = Mathf.RoundToInt(damage * (attacker.Template.Value.CriDamage / 100.0f));

        return damage;
    }

    int CalculateHp()
    {
        int hp = m_enemy.Template.Hp;
        if (Managers.Stage.Type == Define.StageType.Single)
        {
            float stageRate = (Managers.Stage.Template.Stage - 1) * 0.1f;
            hp += Mathf.RoundToInt(m_enemy.Template.Hp * stageRate);
        }

        float waveRate = (Managers.Stage.Wave.Value - 1) * 0.2f;
        hp += Mathf.RoundToInt(m_enemy.Template.Hp * waveRate);

        return hp;
    }
    #endregion
    #region Is
    bool IsCriticalHit(FishBase attacker)
    {
        return UnityEngine.Random.Range(0, 100) < attacker.Template.Value.CriRate;
    }
    #endregion
}
