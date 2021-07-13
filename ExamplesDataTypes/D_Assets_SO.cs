using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace MSFD
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "D_Assets_", menuName = "DataSystem/D_Assets")]
    public class D_Assets_SO : DisplayDataBase
    {
        [HorizontalGroup("Values")]
        [HideLabel]
        [SerializeField]
        D_Assets d_value;

        public D_Assets_SO()
        {
            dataName = DV.assetsName;
        }

        public override DataBase GetDataBase()
        {
            return d_value;
        }
        public D_Assets GetData()
        {
            return d_value;
        }
        public static implicit operator D_Assets(D_Assets_SO display)
        {
            return display.d_value;
        }
    }
}
