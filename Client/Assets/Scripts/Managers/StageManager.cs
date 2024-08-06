using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class StageManager
{
    #region Fields
    StageTemplate m_template;
    Define.StageType m_type;
    bool m_isGameOver = false;
    bool m_isWin = false;
    EnemySpawner m_enemySpawner;
    ReactiveProperty<int> m_wave;
    ReactiveProperty<float> m_remainTime;
    #endregion
    #region Properties
    public StageTemplate Template => m_template;
    public bool IsGameOver => m_isGameOver;
    public Define.StageType Type { get => m_type; set => m_type = value; }
    public IReadOnlyReactiveProperty<int> Wave => m_wave;
    public IReadOnlyReactiveProperty<float> RemainTime => m_remainTime;
    #endregion
    #region Init
    public void Init()
    {
        if(IsSingle())
            m_template = Managers.Template.GetStageTemplate(m_type, Managers.Data.Game.ClearStage + 1);
        else if(IsInfinite())
            m_template = Managers.Template.GetStageTemplate(m_type);

        m_isGameOver = false;
        m_isWin = false;

        m_wave = new ReactiveProperty<int>(1);
        m_remainTime = new ReactiveProperty<float>(m_template.TotalTime);

        m_enemySpawner = new GameObject("@EnemySpawner").GetOrAddComponent<EnemySpawner>();
        m_enemySpawner.StartSpawning();
    }

    public void Update()
    {
        if (m_isGameOver)
            return;

        UpdateRemainTime();
    }

    public void Clear()
    {
        if (m_isGameOver == false)
        {
            m_isGameOver = true;
            m_enemySpawner.StopSpawning();
        }

        m_wave?.Dispose();
        m_remainTime?.Dispose();
        if (m_enemySpawner != null)
        {
            Managers.Resource.Destroy(m_enemySpawner.gameObject);
            m_enemySpawner = null;
        }
    }
    #endregion
    #region Control
    void UpdateRemainTime()
    {
        if (m_remainTime.Value <= 0.0f)
            return;

        m_remainTime.Value -= Time.deltaTime;

        if (CheckGameOver())
        {
            GameOver();
            return;
        }

        if (m_remainTime.Value <= 0.0f)
            NextWave();
    }

    void NextWave()
    {
        m_wave.Value++;
        Managers.Game.IncreaseBonusPearl();

        if (IsSingleBoss() || IsInfiniteBoss())
        {
            m_remainTime.Value = m_template.TotalBossTime;
            m_enemySpawner.StartSpawning(true);
        }
        else
        {
            m_remainTime.Value = m_template.TotalTime;
            m_enemySpawner.StartSpawning();
        }
    }

    public void GameOver()
    {
        m_isGameOver = true;
        m_enemySpawner.StopSpawning();

        Managers.Network.RequestStageEnd(m_type, IsSingle() ? m_template.Stage : m_wave.Value, m_isWin);
    }

    public void ShowUI()
    {
        if (IsSingle())
        {
            if (m_isWin)
            {
                if (Managers.Template.GetSingleStageTemplate(Managers.Data.Game.ClearStage + 1) == null)
                    Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.WinEnd);
                else
                    Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.WinNext);
            }
            else
            {
                Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.Lose);
            }
        }
        else if(IsInfinite())
        {
            Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.Result);
        }
    }
    #endregion
    #region Utility
    bool CheckGameOver()
    {
        bool isWin = Managers.Enemy.Characters.Count <= 0;
        if (IsSingleBoss() && isWin && m_enemySpawner.IsEndSpawn)
        {
            m_isWin = true;
            return true;
        }

        bool isLose = Managers.Enemy.Characters.Count >= Constant.Game.MAX_ENEMY_CNT;
        bool isTimeOver = IsLastWave() && m_remainTime.Value <= 0.0f && Managers.Enemy.Characters.Count > 0;
        if (isLose || isTimeOver)
            return true;

        return false;
    }
    #endregion
    #region Is
    bool IsSingle()
    {
        return m_type == Define.StageType.Single;
    }

    bool IsInfinite()
    {
        return m_type == Define.StageType.Infinite;
    }

    bool IsLastWave()
    {
        return m_template.Wave == m_wave.Value;
    }

    bool IsSingleBoss()
    {
        return IsSingle() && IsLastWave();
    }

    bool IsInfiniteBoss()
    {
        return IsInfinite() && m_wave.Value % 15 == 0;
    }
    #endregion
}
