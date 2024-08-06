using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBase : MonoBehaviour, IPoolable
{
    #region Interface
    public int PoolCnt => 10;
    public GameObject GameObject => gameObject;
    #endregion
    public void Deactivate()
    {
        
    }
}
