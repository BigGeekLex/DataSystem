using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MSFD
{
    [System.Serializable]
    public class D_Assets : DataBase
    {
        [TableList]
        [TabGroup("Values")]
        [SerializeField]
        List<Asset> assets;

        public D_Assets(string dataPath, DataType dataType) : base(dataPath, dataType)
        {
        }
        
        public object GetAsset(string assetName)
        {
            foreach(Asset x in assets)
            {
                if(x.name == assetName)
                {
                    return x.asset;
                }
            }
            Debug.LogError("Array element " + assetName + " not found");
            return null;
        }
        public T GetAsset<T>(string assetName)
        {
            return (T)GetAsset(assetName);
        }
        public object GetAsset(int index)
        {
            return assets[index];
        }
        public T GetAsset<T>(int index)
        {
            return (T)GetAsset(index);
        }
        public override string GetDataDescription()
        {
            string description = "(" +assets.Count + ")";
            for (int i = 0; i < assets.Count; i++)
            {
                description += "\n\t" + i + ")" + assets[i].name + ": " + assets[i].asset.name;
            }
            return description;
        }

        [System.Serializable]
        public struct Asset
        {
            [HideLabel]
            public string name;
            [AssetsOnly]
            [HideLabel]
            public Object asset;
        }
    }
}