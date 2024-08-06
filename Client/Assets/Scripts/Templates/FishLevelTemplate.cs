using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class FishLevelTemplate : ITemplate
{
    #region Properties
    public int Id { get; set; }
    public int Level { get; set; }
    public int CardCount { get; set; }
    public int Gold { get; set; }
    public int DamageBonusRate { get; set; }
    #endregion
    #region Control
    public void Load(DataRow row)
    {
        Id = row.ToInt("id");
        Level = row.ToInt("level");
        CardCount = row.ToInt("cardCount");
        Gold = row.ToInt("gold");
        DamageBonusRate = row.ToInt("damageBonusRate");
    }
    #endregion
}
