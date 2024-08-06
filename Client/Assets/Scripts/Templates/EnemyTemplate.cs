using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class EnemyTemplate : ITemplate
{
    #region Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public int Hp { get; set; }
    public float MoveSpeed { get; set; }
    public int Pearl { get; set; }
    #endregion
    #region Control
    public void Load(DataRow row)
    {
        Id = row.ToInt("id");
        Name = row.ToStringValue("name");
        Hp = row.ToInt("hp");
        MoveSpeed = row.ToFloat("moveSpeed");
        Pearl = row.ToInt("pearl");
    }
    #endregion
}