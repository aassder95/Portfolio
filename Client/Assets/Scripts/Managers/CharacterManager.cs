using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public abstract class CharacterManager<T> where T : MonoBehaviour
{
    #region Fields
    protected HashSet<T> m_characters = new HashSet<T>();
    Subject<int> m_onCntChanged = new Subject<int>();
    #endregion
    #region Properties
    public HashSet<T> Characters => m_characters;
    public IObservable<int> OnCntChanged => m_onCntChanged;
    #endregion
    #region Init
    public virtual void Clear()
    {
        while (m_characters.Count > 0)
            Despawn(m_characters.First());

        m_characters.Clear();
    }
    #endregion
    #region Control
    public abstract GameObject Spawn(int id);

    public virtual void Despawn(T character)
    {
        if (character == null)
            return;

        RemoveCharacter(character);

        Managers.Resource.Destroy(character.gameObject);
    }

    protected void AddCharacter(T character)
    {
        m_characters.Add(character);
        m_onCntChanged.OnNext(m_characters.Count);
    }

    void RemoveCharacter(T character)
    {
        if (m_characters.Contains(character))
            m_characters.Remove(character);

        m_onCntChanged.OnNext(m_characters.Count);
    }
    #endregion
}
