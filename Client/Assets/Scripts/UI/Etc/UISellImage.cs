using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UISellImage : UIBase
{
    #region Enum
    enum Texts
    {
        SellText,
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindText(typeof(Texts));

        GetText((int)Texts.SellText).SetText(110000011, $"{10}");

        Managers.Input.SelectedFish
            .Select(fish => fish != null ? fish.OverlapCtrl.IsDrag : Observable.Empty<bool>())
            .Switch().Subscribe(OnFishDrag).AddTo(this);

        gameObject.SetActive(false);

        return true;
    }
    #endregion
    #region UnityCallback
    void OnTriggerEnter2D(Collider2D coll)
    {
        FishBase fish = coll.GetComponent<FishBase>();
        if (fish != null)
            fish.OverlapCtrl.IsSell = true;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        FishBase fish = coll.GetComponent<FishBase>();
        if (fish != null)
            fish.OverlapCtrl.IsSell = false;
    }
    #endregion
    #region Callback
    void OnFishDrag(bool isDrag)
    {
        gameObject.SetActive(isDrag);
    }
    #endregion
}
