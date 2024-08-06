using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[Serializable]
public class ShopTemplate : ITemplate
{
    #region Properties
    public int Id { get; set; }
    public int Type { get; set; }
    public int TemplateId { get; set; }
    public int Count { get; set; }
    public int PriceType { get; set; }
    public int Price { get; set; }
    public int Rate { get; set; }
    #endregion
    #region Control
    public void Load(DataRow row)
    {
        Id = row.ToInt("id");
        Type = row.ToInt("type");
        TemplateId = row.ToInt("templateId");
        Count = row.ToInt("count");
        PriceType = row.ToInt("priceType");
        Price = row.ToInt("price");
        Rate = row.ToInt("rate");
    }
    #endregion
}