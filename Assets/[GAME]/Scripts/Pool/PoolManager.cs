using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Singleton
    public static PoolManager Instance = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        StartCreation();
    }
    #endregion

    [Header("Objects For Pooling")]
    public GameObject poolingObj;

    [Header("Pools")]
    [HideInInspector] public PoolingPattern pool;

    void StartCreation()
    {
        pool = new PoolingPattern(poolingObj);
        pool.FillPool(20);
    }
}
