using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGame : SceneBase
{
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_type = Define.Scene.Game;

        CreateBackground();
        Managers.UI.ShowUIScene<UIGame>();

        Managers.Pool.Init();
        Managers.Grid.Init();
        Managers.Stage.Init();
        Managers.Game.Init();
        Managers.Card.Init();

        Debug.Log("[SceneGame:Init]");

        return true;
    }

    public override void Clear()
    {
        base.Clear();

        Managers.Card.Clear();
        Managers.Stage.Clear();
        Managers.Input.Clear();
        Managers.Fish.Clear();
        Managers.Enemy.Clear();
        Managers.Game.Clear();
        Managers.Grid.Clear();
        Managers.Pool.Clear();
    }
    #endregion
    #region InitSub
    void CreateBackground()
    {
        GameObject background = Managers.Resource.Instantiate($"BG/BG");
        SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 1));
        worldPos.y += (spriteRenderer.sprite.bounds.size.y * background.transform.localScale.y) / 2.0f;
        background.transform.position = worldPos;
    }
    #endregion
}