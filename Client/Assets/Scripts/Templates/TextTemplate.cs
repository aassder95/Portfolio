using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class TextTemplate : ITemplate
{
    #region Properties
    public int Id { get; set; }
    public string Eng { get; set; }
    public string Kor { get; set; }
    #endregion
    #region Control
    public void Load(DataRow row)
    {
        Id = row.ToInt("id");
        Eng = row.ToStringValue("eng");
        Kor = row.ToStringValue("kor");
    }
    #endregion
}