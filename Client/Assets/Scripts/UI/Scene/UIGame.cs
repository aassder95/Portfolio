using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : UIScene
{
    #region Enum
    enum GameObjects
    {
        EnemyCount,
        UpgradePanel,
    }

    enum Texts
    {
        FastPlayText,
        StageText,
        TimeText,
        EnemyCountText,
        BossText,
        RepositionText,
        RepositionCostText,
        SpawnText,
        SpawnCostText,
        SpawnFishCountText,
        TotalPearlText,
        BonusPearlText,
    }

    enum Images
    {
        BossImage,
    }

    enum Buttons
    {
        FastPlayButton,
        SettingButton,
        RepositionButton,
        SpawnButton,
    }
    #endregion
    #region Fields
    bool m_isFast = false;
    int m_curPearl;
    float m_curEnemyCntRatio = 0.0f;
    float m_targetEnemyCntRatio = 0.0f;
    Slider m_enemyCnt;
    TextMeshProUGUI m_fastPlayTxt;
    TextMeshProUGUI m_timeTxt;
    TextMeshProUGUI m_bossTxt;
    Image m_bossImg;
    Button m_fastPlayBtn;
    Button m_reposBtn;
    Button m_spawnBtn;
    Coroutine m_changePearlCoroutine;
    #endregion
    #region Unity
    void Update()
    {
        UpdateEnemyCnt();
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_curPearl = Managers.Game.TotalPearl.Value;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        InitObject();
        InitText();
        InitImage();
        InitButton();
        InitEvent();

        RefreshUI();

        return true;
    }

    public override void RefreshUI()
    {
        GetText((int)Texts.RepositionText).SetText(110000042);
        GetText((int)Texts.SpawnText).SetText(110000043);

        if (Managers.Stage.Type == Define.StageType.Single)
            GetText((int)Texts.StageText).SetText(110000046, $"{Managers.Stage.Template.Stage}", $"{Managers.Stage.Wave}");
        else if (Managers.Stage.Type == Define.StageType.Infinite)
            GetText((int)Texts.StageText).SetText(110000067, $"{Managers.Stage.Wave}");
    }
    #endregion
    #region InitSub
    void InitObject()
    {
        m_enemyCnt = GetObject((int)GameObjects.EnemyCount).GetComponent<Slider>();
        GameObject upgradePanel = GetObject((int)GameObjects.UpgradePanel);

        foreach (int id in Managers.Card.DeckIds)
        {
            UIUpgradeItem uiSubItem = Managers.UI.InstantiateUI<UIUpgradeItem>("SubItem/UIUpgradeItem", upgradePanel);
            uiSubItem.Type = id;
        }
    }

    void InitText()
    {
        GetText((int)Texts.EnemyCountText).SetText($"0/{Constant.Game.MAX_ENEMY_CNT}");
        GetText((int)Texts.RepositionText).SetText(110000042);
        GetText((int)Texts.RepositionCostText).SetText($"{Constant.Game.Test.COST_REPOS}");
        GetText((int)Texts.SpawnText).SetText(110000043);
        GetText((int)Texts.SpawnCostText).SetText($"{Constant.Game.Test.COST_SPAWN}");
        GetText((int)Texts.SpawnFishCountText).SetText($"0/{Constant.Game.MAX_FISH_CNT}");

        m_bossTxt = GetText((int)Texts.BossText);
        m_fastPlayTxt = GetText((int)Texts.FastPlayText);
        m_timeTxt = GetText((int)Texts.TimeText);
        m_bossTxt.SetText(110000069);
        m_fastPlayTxt.SetText(110000045);
    }

    void InitImage()
    {
        m_bossImg = GetImage((int)Images.BossImage);
        m_bossImg.gameObject.SetActive(false);
    }

    void InitButton()
    {
        GetButton((int)Buttons.SettingButton).SubButtonClick(OnSetting, this);
        m_fastPlayBtn = GetButton((int)Buttons.FastPlayButton);
        m_reposBtn = GetButton((int)Buttons.RepositionButton);
        m_spawnBtn = GetButton((int)Buttons.SpawnButton);

        m_fastPlayBtn.SubButtonClick(OnFastPlay, this);
        m_reposBtn.SubButtonClick(OnReposition, this);
        m_spawnBtn.SubButtonClick(OnSpawn, this);
    }

    void InitEvent()
    {
        Managers.Enemy.OnCntChanged.Subscribe(OnEnemyCntChanged).AddTo(this);
        Managers.Fish.OnCntChanged.Subscribe(OnFishCntChanged).AddTo(this);
        Managers.Game.TotalPearl.Subscribe(OnPearlChanged).AddTo(this);
        Managers.Stage.Wave.Subscribe(OnWaveChanged).AddTo(this);
        Managers.Stage.RemainTime.Subscribe(OnTimeChanged).AddTo(this);
    }

    void RefreshButtonActive()
    {
        int pearl = Managers.Game.TotalPearl.Value;
        m_reposBtn.interactable = pearl >= Constant.Game.Test.COST_REPOS && Managers.Fish.Characters.Count > 0;
        m_spawnBtn.interactable = pearl >= Constant.Game.Test.COST_SPAWN && Managers.Grid.HasAvailFishes();
    }

    void RefreshPearl(int pearl)
    {
        m_curPearl = pearl;
        GetText((int)Texts.TotalPearlText).SetText($"{m_curPearl}");
    }
    #endregion
    #region Control
    void UpdateEnemyCnt()
    {
        m_curEnemyCntRatio = Mathf.Lerp(m_curEnemyCntRatio, m_targetEnemyCntRatio, Time.deltaTime * Constant.UI.ENEMY_CNT_SPEED);
        m_enemyCnt.value = m_curEnemyCntRatio;
    }

    public void StartBlinkBoss()
    {
        m_bossImg.gameObject.SetActive(true);

        StartCoroutine(BlinkBoss());
    }

    void StartChangePearl(int pearl)
    {
        StopChangePearl();

        m_changePearlCoroutine = StartCoroutine(ChangePearlCoroutine(pearl));
    }

    void StopChangePearl()
    {
        if (m_changePearlCoroutine != null)
        {
            StopCoroutine(m_changePearlCoroutine);
            m_changePearlCoroutine = null;
        }
    }
    #endregion
    #region Coroutines
    IEnumerator BlinkBoss()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 3.0f)
        {
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f * Time.timeScale;

            yield return StartCoroutine(Util.FadeOut(0.75f, m_bossTxt, m_bossImg));
            yield return StartCoroutine(Util.FadeIn(0.75f, m_bossTxt, m_bossImg));
        }

        m_bossImg.gameObject.SetActive(false);
    }

    IEnumerator ChangePearlCoroutine(int endPearl)
    {
        float elapsedTime = 0.0f;
        float duration = 0.25f;
        int startPearl = m_curPearl;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            RefreshPearl(Mathf.FloorToInt(Mathf.Lerp(startPearl, endPearl, elapsedTime / duration)));
            yield return null;
        }

        RefreshPearl(endPearl);
        StopChangePearl();
    }
    #endregion
    #region Callback
    void OnEnemyCntChanged(int cnt)
    {
        m_targetEnemyCntRatio = cnt / (float)Constant.Game.MAX_ENEMY_CNT;
        GetText((int)Texts.EnemyCountText).SetText($"{cnt}/{Constant.Game.MAX_ENEMY_CNT}");
    }

    void OnFishCntChanged(int cnt)
    {
        GetText((int)Texts.SpawnFishCountText).SetText($"{cnt}/{Constant.Game.MAX_FISH_CNT}");
        RefreshButtonActive();
    }

    void OnPearlChanged(int value)
    {
        StartChangePearl(value);
        GetText((int)Texts.BonusPearlText).SetText($"+{Managers.Game.GetBonusPearl()}");
        RefreshButtonActive();
    }

    void OnWaveChanged(int value)
    {
        if (Managers.Stage.Type == Define.StageType.Single)
            GetText((int)Texts.StageText).SetText(110000046, $"{Managers.Stage.Template.Stage}", $"{value}");
        else if (Managers.Stage.Type == Define.StageType.Infinite)
            GetText((int)Texts.StageText).SetText(110000067, $"{value}");
    }

    void OnTimeChanged(float value)
    {
        m_timeTxt.SetText($"{value:F1}");
        m_timeTxt.color = value < Constant.UI.REMAIN_TIME_WARNING_THRESHOLD ? Color.red : Color.white;
    }

    void OnFastPlay()
    {
        Debug.Log("[UIGame:OnFastPlay] Click");

        m_isFast = !m_isFast;

        m_fastPlayTxt.SetText(m_isFast ? 110000044 : 110000045);

        Managers.Game.SetGameSpeed(m_isFast);
    }

    void OnSetting()
    {
        Debug.Log("[UIGame:OnSetting] Click");

        Managers.UI.ShowUIPopup<UISetting>();
    }

    void OnReposition()
    {
        Debug.Log("[UIGame:OnReposition] Click");

        if (Managers.Fish.Characters.Count <= 0)
            return;

        if (!Managers.Game.DecreasePearl(Constant.Game.Test.COST_REPOS))
            return;

        Managers.Grid.RepositionFishes();
    }

    void OnSpawn()
    {
        Debug.Log("[UIGame:OnSpawn] Click");

        if (!Managers.Grid.HasAvailFishes())
            return;

        if (!Managers.Game.DecreasePearl(Constant.Game.Test.COST_SPAWN))
            return;

        Managers.Fish.Spawn(0);
    }
    #endregion
}
