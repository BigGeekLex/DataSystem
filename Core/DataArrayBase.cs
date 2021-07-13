using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MSFD
{
    /// <summary>
    /// Attention! Not displayed in inspector!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class DataArrayBase<T> where T : D_String
    {
        [TableList]
        [TabGroup("Values")]
        [SerializeField]
        List<StoreType<T>> assets;
        public bool Get(string dataName, out T data)
        {
            foreach (StoreType<T> x in assets)
            {
                if (x.name == dataName)
                {
                    data = x.asset;
                    return true;
                }
            }
            data = default(T);
            return false;
        }

        [System.Serializable]
        public class StoreType<V>
        {
            [HideLabel]
            public string name;
            [AssetsOnly]
            [HideLabel]
            public V asset;
        }
    }
}