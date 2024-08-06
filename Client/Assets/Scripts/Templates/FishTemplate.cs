using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class FishTemplate : ITemplate
{
    #region Properties
    public int Id { get; set; }
    public int Type { get; set; }
    public int Kind { get; set; }
    public int Tier { get; set; }
    public int Star { get; set; }
    public string Name { get; set; }
    public int DamageMin { get; set; }
    public int DamageMax { get; set; }
    public int CriRate { get; set; }
    public int CriDamage { get; set; }
    public float AttRange { get; set; }
    public float AttSpeed { get; set; }
    public string MissileName { get; set; }
    public int MissileCount { get; set; }
    public float MissileInterval { get; set; }
    public float MissileMoveSpeed { get; set; }
    public int Pearl { get; set; }
    #endregion
    #region Control
    public void Load(DataRow row)
    {
        Id = row.ToInt("id");
        Type = row.ToInt("type");
        Kind = row.ToInt("kind");
        Tier = row.ToInt("tier") - 1;
        Star = row.ToInt("star");
        Name = row.ToStringValue("name");
        DamageMin = row.ToInt("damageMin");
        DamageMax = row.ToInt("damageMax");
        CriRate = row.ToInt("criRate");
        CriDamage = row.ToInt("criDamage");
        AttRange = row.ToFloat("attRange");
        AttSpeed = row.ToFloat("attSpeed");
        MissileName = row.ToStringValue("missileName");
        MissileCount = row.ToInt("missileCount");
        MissileInterval = row.ToFloat("missileInterval");
        MissileMoveSpeed = row.ToFloat("missileMoveSpeed");
        Pearl = row.ToInt("pearl");
    }
    #endregion
}