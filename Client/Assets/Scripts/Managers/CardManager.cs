using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class CardData
{
    #region Fields
    protected FishTemplate m_template;
    protected int m_id;
    protected int m_level;
    protected int m_cardCnt;
    #endregion
    #region Properties
    public FishTemplate Template => m_template;
    public int Id => m_id;
    public int Level => m_level;
    public int CardCnt => m_cardCnt;
    public int DamageMin => CalDamage(m_template.DamageMin, m_level);
    public int DamageMax => CalDamage(m_template.DamageMax, m_level);
    public int NextDamageMin => CalDamage(m_template.DamageMin, m_level + 1);
    public int NextDamageMax => CalDamage(m_template.DamageMax, m_level + 1);
    #endregion
    #region Init
    public CardData(int id, int level, int cardCnt)
    {
        m_template = Managers.Template.GetFishTemplate(id);
        m_id = id;

        SetData(level, cardCnt);
    }
    #endregion
    #region Utility
    protected virtual int CalDamage(int damage, int level)
    {
        FishLevelTemplate fishLevelTemplate = Managers.Template.GetFishLevelTemplate(level);
        if (fishLevelTemplate == null)
            return 0;

        float bonusDamage = damage * (fishLevelTemplate.DamageBonusRate / 100.0f);
        float totalDamage = damage + bonusDamage;
        return Mathf.CeilToInt(totalDamage);
    }
    #endregion
    #region Set
    public void SetData(int level, int cardCnt)
    {
        m_level = level;
        m_cardCnt = cardCnt;
    }
    #endregion
}

public class InGameCardData : CardData
{
    #region Fields
    ReactiveProperty<int> m_upgradeLv = new ReactiveProperty<int>();
    ReactiveProperty<int> m_upgradeCost = new ReactiveProperty<int>();
    #endregion
    #region Properties
    public IReadOnlyReactiveProperty<int> UpgradeLv => m_upgradeLv;
    public IReadOnlyReactiveProperty<int> UpgradeCost => m_upgradeCost;
    #endregion
    #region Init
    public InGameCardData(int id, int level, int cardCnt) : base(id, level, cardCnt)
    {
        m_upgradeLv.Value = 0;
        m_upgradeCost.Value = Constant.Game.Test.START_UPGRADE;
    }

    public void Clear()
    {
        m_upgradeLv.Dispose();
        m_upgradeCost.Dispose();
    }
    #endregion
    #region Control
    public void IncreaseUpgradeLv()
    {
        if (m_upgradeLv.Value == Constant.Game.Test.MAX_UPGRADE)
            return;

        m_upgradeLv.Value++;
        m_upgradeCost.Value += Constant.Game.Test.COST_UPGRADE;
    }
    #endregion
    #region Utility
    protected override int CalDamage(int damage, int level)
    {
        int baseDamage = base.CalDamage(damage, level);
        float bonusDamage = baseDamage * m_upgradeLv.Value * (Constant.Game.Test.UPGRADE_BONUS_RATE / 100.0f);
        float totalDamage = baseDamage + bonusDamage;
        return Mathf.CeilToInt(totalDamage);
    }
    #endregion
}

public class CardManager
{
    #region Fields
    UICardItem m_selectedDeckCardItem;
    int[] m_deckIds = new int[Constant.Deck.MAX_FISH_CNT];
    InGameCardData[] m_inGameCardDatas = new InGameCardData[Constant.Deck.MAX_FISH_CNT];
    List<CardData> m_cardDatas = new List<CardData>();
    #endregion
    #region Properties
    public UICardItem SelectedDeckCardItem => m_selectedDeckCardItem;
    public int[] DeckIds => m_deckIds;
    public List<CardData> CardDatas => m_cardDatas;
    #endregion
    #region Init
    public void Init()
    {
        for (int i = 0; i < m_inGameCardDatas.Length; i++)
        {
            CardData data = GetCardData(m_deckIds[i]);
            m_inGameCardDatas[i] = new InGameCardData(data.Id, data.Level, data.CardCnt);
        }
    }

    public void Clear()
    {
        foreach (InGameCardData data in m_inGameCardDatas)
        {
            data?.Clear();
        }
        Array.Clear(m_inGameCardDatas, 0, m_inGameCardDatas.Length);
    }
    #endregion
    #region Control
    public void RefreshDeck()
    {
        Managers.UI.FindUIScene<UIHome>().RefreshUIDeckCardItem();
        ClearSelectedDeckCardItem();
    }

    public void ClearSelectedDeckCardItem()
    {
        m_selectedDeckCardItem?.SetSelected(false);
        m_selectedDeckCardItem = null;
    }
    #endregion
    #region Get
    public CardData GetCardData(int id)
    {
        return m_cardDatas.FirstOrDefault(data => data.Id == id);
    }

    public InGameCardData GetInGameCardData(int id)
    {
        return m_inGameCardDatas.FirstOrDefault(data => data.Id == id);
    }

    public InGameCardData GetInGameCardData(FishTemplate template)
    {
        return m_inGameCardDatas.FirstOrDefault(data => data.Template.Type == template.Type && data.Template.Kind == template.Kind);
    }

    public InGameCardData GetRandInGameCardData()
    {
        return m_inGameCardDatas[UnityEngine.Random.Range(0, m_inGameCardDatas.Length)];
    }
    #endregion
    #region Set
    public void SetData(JObject data)
    {
        SetCardDatas(data.ToJArray("cards"));
        SetDeckData(data.ToJArray("deck"));
    }

    public void SetCardData(JObject cardData)
    {
        CardData card = GetCardData(cardData.ToInt("templateId"));
        card.SetData(cardData.ToInt("level"), cardData.ToInt("cnt"));
    }

    void SetCardDatas(JArray cardDatas)
    {
        foreach (JToken cardData in cardDatas)
        {
            int templateId = cardData.ToInt("templateId");
            int level = cardData.ToInt("level");
            int count = cardData.ToInt("cnt");

            CardData data = new CardData(templateId, level, count);
            m_cardDatas.Add(data);
        }

        Dictionary<int, FishTemplate> fishTemplates = Managers.Template.FishTemplates;
        foreach (FishTemplate template in fishTemplates.Values.Where(template => template.Star == 1))
        {
            if (m_cardDatas.Any(data => data.Id == template.Id))
                continue;

            m_cardDatas.Add(new CardData(template.Id, 0, 0));
        }

        m_cardDatas = m_cardDatas.
            OrderBy(data => data.Template.Tier).
            ThenByDescending(data => data.Level).
            ThenByDescending(data => data.CardCnt).ToList();
    }

    public void SetDeckData(JArray deckData)
    {
        for (int i = 0; i < m_deckIds.Length; i++)
        {
            m_deckIds[i] = (int)deckData[i];
        }
    }

    public void SetSelectedDeckCardItem(UICardItem uiCardItem)
    {
        if (uiCardItem != null)
            ClearSelectedDeckCardItem();

        m_selectedDeckCardItem = uiCardItem;
    }
    #endregion
}
