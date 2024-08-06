using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopup : UIBase, IPointerClickHandler
{
    #region Fields
    protected GameObject m_popupPanel;
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceOverlay);

        return true;
    }
    #endregion
    #region Control
    public virtual void Close()
    {
        Managers.UI.CloseUIPopup(this);
    }
    #endregion
    #region UnityCallback
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(m_popupPanel.GetComponent<RectTransform>(), eventData.position, null))
            Managers.UI.CloseUIPopup(this);
    }
    #endregion
}
