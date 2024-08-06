using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class StageTemplate : ITemplate
{
    #region Properties
    public int Id { get; set; }
    public int Type { get; set; }
    public int Stage { get; set; }
    public int Wave { get; set; }
    public float Time { get; set; }
    public float BreakTime { get; set; }
    public float TotalTime { get; set; }
    public int EnemyCount { get; set; }
    public int SpecialEnemyCount { get; set; }
    public int TotalEnemyCount { get; set; }
    public int EnemyId { get; set; }
    public int SpecialEnemyId { get; set; }
    public float BossTime { get; set; }
    public float BossBreakTime { get; set; }
    public float TotalBossTime { get; set; }
    public int BossId { get; set; }
    public int Gold { get; set; }
    #endregion
    #region Control
    public void Load(DataRow row)
    {
        Id = row.ToInt("id");
        Type = row.ToInt("type");
        Stage = row.ToInt("stage");
        Wave = row.ToInt("wave");
        Time = row.ToFloat("time");
        BreakTime = row.ToFloat("breakTime");
        TotalTime = Time + BreakTime;
        EnemyCount = row.ToInt("EnemyCount");
        SpecialEnemyCount = row.ToInt("specialEnemyCount");
        TotalEnemyCount = EnemyCount + SpecialEnemyCount;
        EnemyId = row.ToInt("enemyId");
        SpecialEnemyId = row.ToInt("specialEnemyId");
        BossTime = row.ToFloat("bossTime");
        BossBreakTime = row.ToFloat("bossBreakTime");
        TotalBossTime = BossTime + BossBreakTime;
        BossId = row.ToInt("bossId");
        Gold = row.ToInt("gold");
    }
    #endregion
}