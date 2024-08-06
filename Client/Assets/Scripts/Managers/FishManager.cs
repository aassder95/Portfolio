using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : CharacterManager<FishBase>
{
    #region Control
    public override GameObject Spawn(int id)
    {
        InGameCardData data = Managers.Card.GetRandInGameCardData();

        GameObject go = Managers.Resource.Instantiate($"Fish/{data.Template.Name}");
        Managers.Grid.PlaceFishInRandPos(go);

        FishBase fish = go.GetOrAddComponent<FishBase>();
        fish.Activate(data.Template);
        AddCharacter(fish);

        return go;
    }

    public override void Despawn(FishBase fish)
    {
        if (fish == null)
            return;

        Managers.Grid.RemoveFish(fish.gameObject);
        base.Despawn(fish);
    }
    #endregion
}
