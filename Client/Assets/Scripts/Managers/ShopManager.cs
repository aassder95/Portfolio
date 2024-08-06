using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager
{
    #region Fields
    List<int> m_allCards = new List<int>();
    List<int> m_purchasedCards = new List<int>();
    #endregion
    #region Properties
    public List<int> AllCards => m_allCards;
    public List<int> PurchasedCards => m_purchasedCards;
    #endregion
    #region Set
    public void SetData(JObject data)
    {
        JArray allDatas = data.ToJArray("allCards");
        JArray purchasedDatas = data.ToJArray("purchasedCards");

        m_allCards.Clear();
        m_purchasedCards.Clear();

        for (int i = 0; i < allDatas.Count; i++)
        {
            m_allCards.Add((int)allDatas[i]);
        }

        for (int i = 0; i < purchasedDatas.Count; i++)
        {
            m_purchasedCards.Add((int)purchasedDatas[i]);
        }

        Managers.UI.FindUIScene<UIHome>()?.RefreshUIShopCardItem();
    }
    #endregion
    #region Is
    public bool IsCardPurchased(int id)
    {
        return m_purchasedCards.Contains(id);
    }
    #endregion
}
