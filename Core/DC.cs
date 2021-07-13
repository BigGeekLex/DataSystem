using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using MSFD.Data;


namespace MSFD
{
    public class DC : SingletoneBase<DC>
    {
        [SerializeField]
        D_Container_SO dataContainer;
        static D_Container dataStore;
        [Header("Disable if you do not want to change saved data")]
        [SerializeField]
        bool isSaveAllOnAppQuit = true;

        [Header("Empty field is a dataContainer path profile")]
        [SerializeField]
        string gameProfile;

        [SerializeField]
        DisplayDataStoreMode displayDataStoreMode = DisplayDataStoreMode.display;

        [Button()]
        public void DisplayDataStore()
        {
            if (!Application.isPlaying)
            {
                RegisterSOData();
            }
            string output = dataStore.GetDataName() + ": ";
            output += dataStore.GetContainerDescription();
            if ((displayDataStoreMode & DisplayDataStoreMode.debugLog) > 0)
            {
                string s;
                foreach (DataBase x in dataStore.GetDataBases())
                {
                    s = x.DataName + " (" + x.GetType() + "): " + x.GetDataDescription();
                    Debug.Log(s);
                }
            }
            if ((displayDataStoreMode & DisplayDataStoreMode.display) > 0)
            {
                displayDataStore = output;
            }
        }
        [Button]
        public void ActivateEditorTriggerVerification()
        {
            dataContainer.ActivateRecoursiveTriggerVerification();
        }
        protected override void AwakeInitialization()
        {
            RegisterSOData();
        }
        /*public static bool TryAddData(DataBase _data, string path)
        {
            DataBase d;
            if (dataStoreOld.TryGetValue(path, out d))
            {
                Debug.LogError("Data already exists: " + path);
                return false;
            }
            else
            {
                dataStoreOld.Add(path, _data);
                return true;
            }
        }*/


        [Button]
        public static DataBase GetData(string path)
        {
            return dataStore.GetData(path);
        }
        public static T GetData<T>(string path) where T : class
        {
            return GetData(path) as T;
        }
        public static bool TryGetData(string path, out DataBase data)
        {
            return dataStore.TryGetData(path, out data);
        }
        public static bool TryGetData<T>(string path, out T data) where T : class
        {
            return dataStore.TryGetData<T>(path, out data);
        }
        [Button]
        public static bool TryRemoveData(string path)
        {
            string beginPath;
            string name = GetLastPathPart(path, out beginPath);
            D_Container dataContainer;
            if (dataStore.TryGetData<D_Container>(beginPath, out dataContainer))
            {
                return dataContainer.TryRemoveData(name);
            }
            else
            {
                return false;
            }
        }


        //[Button]
        public void RegisterSOData()
        {
            if (dataContainer != null)
            {        
                dataStore = dataContainer.GetData();

                if (!string.IsNullOrEmpty(gameProfile))
                {
                    dataStore.SetPathType(PathType.manual);
                    dataStore.SetDataPath("_Profiles/" + gameProfile);
                }

                LoadAll();
                RefreshAll();
            }
            else
            {
                Debug.LogError("There no root data container in DC!");
            }
        }
        [FoldoutGroup("RecoursiveFunctions")]
        [Button]
        public void LoadAll()
        {
            dataStore.LoadContainerRecoursively();
        }
        [FoldoutGroup("RecoursiveFunctions")]
        [Button]
        public void SaveAll()
        {
            dataStore.SaveContainerRecoursively();
        }
        [FoldoutGroup("RecoursiveFunctions")]
        [Button]
        public void RefreshAll()
        {
            dataStore.OnValueChangedContainerRecoursively();
        }
        private void OnApplicationQuit()
        {
            if (isSaveAllOnAppQuit)
            {
                SaveAll();
            }
        }


        public static List<string> GetStoredInContainerDataPathes(string path, System.Predicate<string> predicate)
        {
            List<string> allDataNamesFromContainer = Instance.GetStoredInContainerDataNamesThis(path);
            List<string> correctDataNames = new List<string>();
            foreach (string x in allDataNamesFromContainer)
            {
                if (predicate(path + "/" + x))
                {
                    correctDataNames.Add(x);
                }
            }

            return correctDataNames;
        }

        /// <summary>
        /// Return data names from container
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetStoredInContainerDataNames(string path)
        {
            return Instance.GetStoredInContainerDataNamesThis(path);
        }
        [Button]
        List<string> GetStoredInContainerDataNamesThis(string path)
        {
            DataBase dataBase = dataStore.GetData(path);
            D_Container container = dataBase as D_Container;
            if (container == null)
            {
                Debug.LogError("Type error. DataBase is not a container: " + path);
                return null;
            }
            else
            {
                return container.GetDataBaseNames();
            }
        }

        public static string GetLastPathPart(string path, out string beginPathPart)
        {
            string[] subStr = path.Split('/');
            if (subStr.Length == 0 || subStr.Length == 1)
            {
                beginPathPart = null;
                return path;
            }
            else
            {
                beginPathPart = subStr[0];
                for (int i = 1; i < subStr.Length - 1; i++)
                {
                    beginPathPart += subStr[i];
                }

                return subStr[subStr.Length - 1];
            }
        }
        public static string GetNextPathPart(string path, out string restPath)
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

        #region DCService
        public enum GetFromContainerMode { all, containersOnly, dataOnly };

        public enum SaveLoadMode { release, profile};
        //----

        [ShowIf("@IsShowDisplayString()")]
        [SerializeField]
        [PropertySpace]
        [TextArea(5, 30)]
        string displayDataStore = "";
        public bool IsShowDisplayString()
        {
            return (displayDataStoreMode & DisplayDataStoreMode.display) > 0;
        }

        [System.Flags]
        public enum DisplayDataStoreMode
        {
            debugLog = 1 << 1,
            display = 1 << 2
        };
        #endregion
    }
}