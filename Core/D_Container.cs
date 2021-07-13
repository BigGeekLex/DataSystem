using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using MSFD.Data;

namespace MSFD
{
    [System.Serializable]
    public class D_Container : DataBase
    {
        public TriggerProcessor TriggerProcessor { get; private set; } = new TriggerProcessor();
        Dictionary<string, DataBase> dataBases = new Dictionary<string, DataBase>();

        public D_Container(string dataPath, DataType dataType) : base(dataPath, dataType)
        {
            dataBases = new Dictionary<string, DataBase>();
            displayLevel = int.MaxValue;
        }

        public D_Container()
        {
            dataBases = new Dictionary<string, DataBase>();
            displayLevel = int.MaxValue;
        }


        #region DataAccessMetods
        public void AddData(DataBase _data)
        {
            DataBase d;
            if (dataBases.TryGetValue(_data.GetDataName(), out d))
            {
                Debug.LogError("Data already exists: " + _data.GetDataName());
            }
            else
            {
                dataBases.Add(_data.GetDataName(), _data);
                _data.SetParentContainer(this);
            }
        }
        public bool TryAddData(DataBase _data)
        {
            DataBase d;
            if (dataBases.TryGetValue(_data.GetDataName(), out d))
            {
                //Debug.LogError("Data already exists: " + _data.GetDataPath());
                return false;
            }
            else
            {
                dataBases.Add(_data.GetDataName(), _data);
                _data.SetParentContainer(this);
                return true;
            }
        }
        public DataBase GetData(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return this;
            }
            DataBase d;
            D_Container container = this;
            string restPath = path;
            while (true)
            {
                string nextName = GetNextPathPart(restPath, out restPath);
                if (container.TryGetDataFromContainer(nextName, out d))
                {
                    if (restPath == null)
                    {
                        return d;
                    }
                    else
                    {
                        container = d as D_Container;
                        if (container == null)
                        {
                            Debug.LogError("Data is not found (part of path is not a container): " + path + "(rest path is: " + restPath + ")");
                            return null;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Data is not found: " + path);
                    return null;
                }
            }
        }
        public T GetData<T>(string path) where T : class
        {
            return GetData(path) as T;
        }
        public bool TryGetData(string path, out DataBase data)
        {
            if (string.IsNullOrEmpty(path))
            {
                data = this;
                return true;
            }
            D_Container container = this;
            string restPath = path;
            while (true)
            {
                string nextName = GetNextPathPart(restPath, out restPath);
                if (container.TryGetDataFromContainer(nextName, out data))
                {
                    if (restPath == null)
                    {
                        return true;
                    }
                    else
                    {
                        container = data as D_Container;
                        if (container == null)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public bool TryGetData<T>(string path, out T data) where T : class
        {
            DataBase d;
            bool res = TryGetData(path, out d);
            data = d as T;
            return res;
        }
        bool TryGetDataFromContainer(string name, out DataBase data)
        {
            return dataBases.TryGetValue(name, out data);
        }
        public bool TryRemoveData(string name)
        {
            DataBase data;

            if (TryGetDataFromContainer(name, out data))
            {
                dataBases.Remove(name);
                data.SetParentContainer(null);
                return true;
            }
            else
                return false;
        }

        public List<DataBase> GetDataBases()
        {
            return new List<DataBase>(dataBases.Values);
        }
        public List<string> GetDataBaseNames()
        {
            return new List<string>(dataBases.Keys);
        }
        public List<D_Container> GetContainers()
        {
            var containers = from c in dataBases.Values
                             where c is D_Container
                             select c as D_Container;
            return containers.ToList();
        }
        public List<string> GetContainersNames()
        {
            var containers = from c in dataBases.Values
                             where c is D_Container
                             select c.DataName;
            return containers.ToList();
        }
        #endregion

        public override object Clone()
        {
            D_Container container = new D_Container(DataName, dataType);
            container.SetBroadcastEvent(broadcastEvent);
            container.SetPathType(this.GetPathType());
            container.SetDataPath(GetDataPath());
            foreach (var d in dataBases)
            {
                container.TryAddData((DataBase)d.Value.Clone());
            }
            return container;
        }

        #region Recoursive methods Save/Load/OnvalueChanged
        bool traversalFlag = false;
        bool IsTraversalNeeded(ref bool localTraversalFlag)
        {
            if (localTraversalFlag)
            {
                return false;
            }
            else
            {
                localTraversalFlag = true;
                return true;
            }
        }
        [Obsolete("Use ContainerRecoursiveMethod instead")]
        public override void Load()
        {
            if (IsTraversalNeeded(ref traversalFlag))
            {
                foreach (var keyValuePair in dataBases)
                {
                    DataBase dataBase = keyValuePair.Value;
                    if (dataBase is D_Container)
                    {
                        dataBase.Load();
                    }
                    else if (dataBase.GetDataType() == DataType.readWriteSave)
                    {
                        dataBase.Load();
                    }
                }
            }
        }
        [Obsolete("Use ContainerRecoursiveMethod instead")]
        public override void Save()
        {
            if (IsTraversalNeeded(ref traversalFlag))
            {
                foreach (var keyValuePair in dataBases)
                {
                    DataBase dataBase = keyValuePair.Value;
                    if (dataBase is D_Container)
                    {
                        dataBase.Save();
                    }
                    else if (dataBase.GetDataType() == DataType.readWriteSave)
                    {
                        dataBase.Save();
                    }
                }
            }
        }
        [Obsolete("Use ContainerRecoursiveMethod instead")]
        public override void OnValueChanged()
        {
            if (IsTraversalNeeded(ref traversalFlag))
            {
                base.OnValueChanged();
                foreach (var keyValuePair in dataBases)
                {
                    DataBase dataBase = keyValuePair.Value;
                    dataBase.OnValueChanged();
                }
            }
        }
        public void LoadContainerRecoursively()
        {
            Load();
            ResetTraversalFlagRecoursively();
        }
        public void SaveContainerRecoursively()
        {
            Save();
            ResetTraversalFlagRecoursively();
        }
        public void OnValueChangedContainerRecoursively()
        {
            OnValueChanged();
            ResetTraversalFlagRecoursively();
        }
        public void ResetTraversalFlagRecoursively()
        {
            if (traversalFlag)
            {
                traversalFlag = false;
                foreach (var keyValuePair in dataBases)
                {
                    DataBase dataBase = keyValuePair.Value;
                    D_Container container = dataBase as D_Container;
                    if (container != null)
                    {
                        container.ResetTraversalFlagRecoursively();
                    }
                }
            }
        }
        #endregion

        #region Display Data
        int displayLevel;
        void TrySetDisplayLevel(int level)
        {
            displayLevel = Mathf.Min(displayLevel, level);
        }
        public string GetContainerDescription(int level = 0)
        {
            foreach (var data in dataBases)
            {
                D_Container container = data.Value as D_Container;
                if (container != null)
                {
                    container.TrySetDisplayLevel(level + 1);
                }

            }
            string description = $"[{dataBases.Count}]";// + "  " + GetDataType().ToString();//---Container";// = "D_Container: " + GetDataName();
            if (level <= displayLevel)
            {
                int i = 1;
                string displacement = "";
                for (int j = -1; j < level; j++)
                {
                    displacement += "\t";
                }
                foreach (var data in dataBases)
                {
                    string innerDescription;
                    if (data.Value is D_Container)
                    {
                        innerDescription = "\n" + displacement + i + ". " + data.Key + " (" + data.Value + ")" + ": " + ((D_Container)data.Value).GetContainerDescription(level + 1);
                    }
                    else
                    {
                        innerDescription = "\n" + displacement + i + ". " + data.Key + " (" + data.Value + ")" + ": " + data.Value.GetDataDescription().Replace("\n\t", "\n\t" + displacement);// + "  " + data.Value.GetDataType().ToString();
                    }
                    description += innerDescription;// + "\n";
                    i++;
                }
            }
            else
            {
                description += " //Nested Container//";
            }
            return description;
        }
        //[System.Obsolete("Don't recommend to use. Use GetContainerDescription() instead")]
        public override string GetDataDescription()
        {
            return GetContainerDescription();
        }
        #endregion
        string GetNextPathPart(string path, out string restPath)
        {
            string[] subStr = path.Split('/');
            if (subStr.Length == 0 || subStr.Length == 1)
            {
                restPath = null;
                return path;
            }
            else
            {
                restPath = subStr[1];
                for (int i = 2; i < subStr.Length; i++)
                {
                    restPath += "/" + subStr[i];
                }
                return subStr[0];
            }
        }

        /*
[System.Obsolete("Don't recommend to use. Use GetContainerDescription instead")]
public override string GetDataDescription()
{
    string description = $"[{dataBases.Count}]";//---Container";// = "D_Container: " + GetDataName();
    if (!isContainerWasDisplayed)
    {
        isContainerWasDisplayed = true;
        int i = 1;
        foreach (var data in dataBases)
        {
            string innerDescription;
            innerDescription = "\n\t" + i + ". " + data.Key + " (" + data.Value + ") " + ": " + data.Value.GetDataDescription().Replace("\n\t", "\n\t\t");
            description += innerDescription;// + "\n";
            i++;
        }
    }
    else
    {
        description += " //Nested Container//";
    }
    isContainerWasDisplayed = false;
    return description;


}
        
         
                 DataBase GetDataFromContainer(string name)
        {
            DataBase d;
            if (dataBases.TryGetValue(name, out d))
            {
                return d;
            }
            else
            {
                Debug.LogError("Data is not found: " + GetDataName() + "/" + name);
                return null;
            }
        }
        public T GetDataFromContainer<T>(string name) where T : class
        {
            DataBase d;
            if (dataBases.TryGetValue(name, out d))
            {
                return d as T;
            }
            else
            {
                Debug.LogError("Data is not found: " + GetDataName() + "/" + name + " (" + typeof(T) + ")");
                return null;
            }
        }
                public bool TryGetDataFromContainer<T>(string name, out T data) where T : class
        {
            DataBase d;
            bool res = TryGetDataFromContainer(name, out d);
            data = d as T;
            return res;
        }
        */
        public void InitializeTriggers(List<TriggerBase> triggerBases)
        {
            foreach (TriggerBase x in triggerBases)
            {
                TriggerProcessor.AddTrigger(x);
            }
        }
    }
}
