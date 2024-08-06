using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UIFishStar : UIBase
{
    #region Enum
    enum GameObjects
    {
        Star1,
        Star2,
        Star3,
        Star4,
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindObject(typeof(GameObjects));

        float offsetY = 0.2f;
        transform.position += Vector3.down * (transform.parent.GetComponent<BoxCollider2D>().bounds.size.y - offsetY);

        FishBase fishBase = transform.parent.GetComponent<FishBase>();
        fishBase.Template.Subscribe(OnStarChanged).AddTo(this);

        return true;
    }
    #endregion
    #region Callback
    void OnStarChanged(FishTemplate template)
    {
        int star = template == null ? 1 : template.Star;

        foreach (GameObjects obj in Enum.GetValues(typeof(GameObjects)))
            GetObject((int)obj).SetActive(((int)obj + 1) == star);
    }
    #endregion
}
