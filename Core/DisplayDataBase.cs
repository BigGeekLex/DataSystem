#define UNCHANGEBLE_SO
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Reflection;
using MSFD.Data;
namespace MSFD
{
   [System.Serializable]
    public abstract class DisplayDataBase : ScriptableObject
    {
        [TableColumnWidth(140, resizable: false)]
        [SerializeField]
        protected string dataName;
        public virtual void RegisterData(D_Container container)
        {
            DataBase d_value = GetDataBase();
            d_value.SetDataName(dataName);
#if !UNITY_EDITOR || !UNCHANGEBLE_SO
            container.TryAddData(d_value);
            return;
#endif
#if UNITY_EDITOR && UNCHANGEBLE_SO
            DataBase cloneValue = (DataBase)d_value.Clone();
            container.TryAddData(cloneValue);
            return;
#endif
        }

        public abstract DataBase GetDataBase();
        /*
        {
            return (DataBase)GetType().GetField("d_value", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        }*/
        public T GetBaseData<T>() where T : DataBase
        {
            return (T)GetDataBase();
        }
        public string GetDataName()
        {
            return dataName;
        }
    }
}