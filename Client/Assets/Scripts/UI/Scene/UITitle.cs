using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITitle : UIScene, IPointerClickHandler
{
    #region Enum
    enum GameObjects
    {
        Loading,
        LoadingSlider,
    }

    enum Texts
    {
        TitleText,
        ClientVerText,
        LoadingText,
        LoadingDescText,
        PressedText,
    }
    #endregion
    #region Fields
    float m_curLoadingRatio = 0.0f;
    float m_targetLoadingRatio = 0.0f;
    GameObject m_loading;
    Slider m_loadingSlider;
    TextMeshProUGUI m_loadingText;
    TextMeshProUGUI m_loadingDescText;
    TextMeshProUGUI m_pressedText;
    #endregion
    #region Unity
    protected override async void Start()
    {
        base.Start();

        await Managers.Template.LoadTemplates();
    }

    void Update()
    {
        m_curLoadingRatio = Mathf.Lerp(m_curLoadingRatio, m_targetLoadingRatio, Time.deltaTime * 30.0f);
        m_loadingText.SetText($"{(m_curLoadingRatio * 100.0f):F1}%");
        m_loadingSlider.value = m_curLoadingRatio;

        if (m_curLoadingRatio >= 1.0f && m_loading.activeSelf == true)
        {
            m_loading.SetActive(false);
            m_pressedText.gameObject.SetActive(true);

            StartCoroutine(BlinkText());
        }
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        Managers.Template.OnLoadTemplate.Subscribe(OnLoadTemplate).AddTo(this);

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        InitObject();
        InitText();

        return true;
    }
    #endregion
    #region InitSub
    void InitObject()
    {
        m_loading = GetObject((int)GameObjects.Loading);
        m_loadingSlider = GetObject((int)GameObjects.LoadingSlider).GetComponent<Slider>();
        m_loadingSlider.value = 0.0f;
    }

    void InitText()
    {
        GetText((int)Texts.TitleText).SetText(110000001);
        GetText((int)Texts.ClientVerText).SetText(110000002, $"{Application.version}");

        m_loadingText = GetText((int)Texts.LoadingText);
        m_loadingDescText = GetText((int)Texts.LoadingDescText);
        m_pressedText = GetText((int)Texts.PressedText);
        m_loadingText.SetText("0%");
        m_loadingDescText.SetText(110000003);
        m_pressedText.SetText(110000004);
        m_pressedText.gameObject.SetActive(false);
    }
    #endregion
    #region Coroutines
    IEnumerator BlinkText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(Util.FadeOut(0.75f, m_pressedText));
            yield return StartCoroutine(Util.FadeIn(0.75f, m_pressedText));
        }
    }
    #endregion
    #region UnityCallback
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_loading.activeSelf)
            return;

        Managers.Network.RequestLogin();
    }
    #endregion
    #region Callback
    void OnLoadTemplate(float value)
    {
        m_targetLoadingRatio = value;
    }
    #endregion
}
