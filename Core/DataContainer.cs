using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MSFD.Data;
namespace MSFD
{
    [System.Serializable]
    //[CreateAssetMenu(fileName = "_Container_", menuName = "DataSystem/DataContainer")]
    public class DataContainer : DisplayDataBase
    {
        [ListDrawerSettings(ShowPaging = true)]
        [HorizontalGroup("Values")]      
        //[TableList()]
        [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.Foldout)]
        [SerializeField]
        List<DisplayDataBase> baseData;

        public override DataBase GetDataBase()
        {
            throw new System.NotImplementedException();
        }
        public DataContainer GetStoredContainer(string containerName)
        {
            foreach(DisplayDataBase x in baseData)
            {
                if(containerName == x.GetDataName())
                {
                    return x as DataContainer;
                }
            }
            return null;
        }
        /*
         public DataBase GetStoredData(string path)
         {
             string dataName;
             string nextPath;
             dataName = DC.GetNextDataPathPart(path, out nextPath);

             foreach (DisplayDataBase x in baseData)
             {
                 if (x.GetDataName() == path)
                 {
                     if (nextPath == null)
                     {
                         StoreData storeData = new StoreData(x.name, DataType.readWrite, x);
                     }
                     else
                     {
                         return ((DataContainer)x).GetStoredData(nextPath);
                     }
                 }
             }
             return null;
         }*/
/*
        public override void RegisterData(IDataBase dataBase, string previousPath)
        {
            foreach (DisplayDataBase x in baseData)
            {
                if (string.IsNullOrEmpty(previousPath))
                {
                    x.RegisterData(dataBase, dataName);
                }
                else
                {
                    if (x != null)
                    {
                        x.RegisterData(dataBase, previousPath + "/" + dataName);
                    }
                    else
                    {
                        Debug.LogError("Can't register data in " + previousPath + "/" + dataName + 
                            " because it is a null value. Probably you lose some data or add unnecessary reference");
                    }
                }
            }
        }
*/
        public List<string> GetStoredDataNames(DC.GetFromContainerMode getFromContainerMode)
        {
            List<string> storedDataNames = new List<string>();
            for (int i = 0; i < baseData.Count; i++)
            {
                if ((getFromContainerMode == DC.GetFromContainerMode.all)
                    || (getFromContainerMode == DC.GetFromContainerMode.containersOnly && baseData[i] is DataContainer)
                    || (getFromContainerMode == DC.GetFromContainerMode.dataOnly && !(baseData[i] is DataContainer)))
                {
                    storedDataNames.Add(baseData[i].GetDataName());
                }
            }
            return storedDataNames;
        }
        /*
        public string[] GetStoredNames()
        {
            string[] storedDataNames = new string[baseData.Count];
            for(int i = 0; i < storedDataNames.Length; i++)
            {
                storedDataNames[i] = baseData[i].GetDataName();
            }
            return storedDataNames;
        }
        public string[] GetStoredContainersNames()
        {
            List<string> storedContainersNames = new List<string>();
            for (int i = 0; i < baseData.Count; i++)
            {
                if (baseData[i] is DataContainer)
                {
                    storedContainersNames.Add(baseData[i].GetDataName());
                }
            }
            return storedContainersNames.ToArray();
        }
        public string[] GetStoredDataNames()
        {
            List<string> storedDataNames = new List<string>();
            for (int i = 0; i < baseData.Count; i++)
            {
                if (!(baseData[i] is DataContainer))
                {
                    storedDataNames.Add(baseData[i].GetDataName());
                }
            }
            return storedDataNames.ToArray();
        }*/
    }
}