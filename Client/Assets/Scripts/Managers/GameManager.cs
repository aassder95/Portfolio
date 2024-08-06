using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager
{
    #region Fields
    ReactiveProperty<int> m_totalPearl;
    #endregion
    #region Properties
    public IReadOnlyReactiveProperty<int> TotalPearl => m_totalPearl;
    #endregion
    #region Init
    public void Init()
    {
        m_totalPearl = new ReactiveProperty<int>(Constant.Game.Test.START_PEARL);

        SetGameSpeed(false);
    }

    public void Clear()
    {
        m_totalPearl?.Dispose();

        SetGameSpeed(false);
    }
    #endregion
    #region Control
    public void IncreasePearl(int value)
    {
        m_totalPearl.Value += value;
    }

    public void IncreaseBonusPearl()
    {
        m_totalPearl.Value += GetBonusPearl();
    }

    public bool DecreasePearl(int value)
    {
        if (m_totalPearl.Value < value)
        {
            Debug.Log("[GameManager:DecreasePearl] Not enough pearls");
            return false;
        }

        m_totalPearl.Value -= value;

        return true;
    }
    #endregion
    #region Get
    public int GetBonusPearl()
    {
        return Mathf.Min(m_totalPearl.Value / 10, Constant.Game.Test.MAX_BONUS_PEARL);
    }
    #endregion
    #region Set
    public void SetGameSpeed(bool isFast)
    {
        Time.timeScale = isFast ? Constant.Game.FAST_GAME_SPEED : Constant.Game.NORMAL_GAME_SPEED;
        Time.fixedDeltaTime = Constant.Game.FIXED_DELTA_TIME * Time.timeScale;
    }
    #endregion
}
