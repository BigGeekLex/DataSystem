using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using MSFD.Data;
namespace MSFD
{
    [System.Serializable]
    public abstract class DataBase : ICloneable, IReplicateType
    {
        //[SerializeField]
        //[TabGroup("System", order: 0)]
        protected string dataName;
        [TabGroup("System", order: 0)]
        [SerializeField]
        protected DataType dataType;

        [TabGroup("System", order: 0)]
        [SerializeField]
        protected PathType pathType = PathType.fullauto;
        [ShowIf("@IsShowPathField()")]
        [TabGroup("System", order: 0)]
        [SerializeField]
        protected string dataPath;

        [TabGroup("System", order: 0)]
        [SerializeField]
        protected ReplicateType replicateType = ReplicateType.value;
        [TabGroup("System", order: 0)]
        [SerializeField]
        protected bool isSaveOnValueChanged = false;

        [Title("Broadcast On Value Changed", horizontalLine: true)]
        [TabGroup("System", order: 0)]
        [SerializeField]
        protected BroadcastEvent broadcastEvent = 0;
        [ShowIf("@IsShowMessageField()")]
        [TabGroup("System", order: 0)]
        [SerializeField]
        [LabelText("By default message is dataPath")]
        string message;
        /// <summary>
        /// Don't invoke it. Call instead OnValueChanged()
        /// </summary>
        protected Action onValueChanged;

        protected D_Container parentContainer;
        public string DataName
        {
            get
            {
                return GetDataName();
            }
            set
            {
                SetDataName(value);
            }
        }

        protected DataBase(string dataName, DataType dataType)
        {
            this.dataName = dataName;
            this.dataType = dataType;
        }
        protected DataBase()
        {

        }
        public ReplicateType GetReplicateType()
        {
            return replicateType;
        }
        public virtual void Save()
        {
            Debug.LogError("Save is not implemented to " + dataName);
        }
        public virtual void Load()
        {
            Debug.LogError("Load is not implemented to " + dataName);
        }

        /// <summary>
        /// Attention! If you change DataName in data which is lied in container then key in container will not change. You need to Remove/Add data to refresh key
        /// </summary>
        /// <param name="_dataPath"></param>
        public void SetDataName(string _dataPath)
        {
            dataName = _dataPath;
        }
        public string GetDataName()
        {
            return dataName;
        }

        public DataType GetDataType()
        {
            return dataType;
        }
        public BroadcastEvent GetBroadcastEvent()
        {
            return broadcastEvent;
        }
        public void SetBroadcastEvent(BroadcastEvent _broadcastEvent)
        {
            broadcastEvent = _broadcastEvent;
        }

        public virtual void AddListenerOnValueChanged(Action _action)
        {
            onValueChanged += _action;
        }
        public virtual void RemoveListenerOnValueChanged(Action _action)
        {
            onValueChanged -= _action;
        }

        public virtual void OnValueChanged()
        {
            if (onValueChanged != null)
                onValueChanged.Invoke();

            if (isSaveOnValueChanged)
            {
                if (dataType == DataType.readWriteSave)
                {
                    Save();
                }
                else
                {
                    Debug.LogError(GetDataPath() + ": Attempt to Save in incorrect mode");
                }
            }
            if ((broadcastEvent & BroadcastEvent.message) > 0)
            {
                Messenger.Broadcast(GetBroadcastedMessage(), MessengerMode.DONT_REQUIRE_LISTENER);
            }
            if ((broadcastEvent & BroadcastEvent.messageDataBase) > 0)
                Messenger<DataBase>.Broadcast(MessengerValues.dataBaseEventPrefix + GetBroadcastedMessage(), this, MessengerMode.DONT_REQUIRE_LISTENER);
        }
        protected string GetBroadcastedMessage()
        {
            if (string.IsNullOrEmpty(message))
            {
                return dataName;
            }
            else
            {
                return message;
            }
        }

        public void SetPathType(PathType _pathType)
        {
            pathType = _pathType;
        }
        public PathType GetPathType()
        {
            return pathType;
        }
        [Button]
        public virtual string GetDataPath()
        {
            if (pathType == PathType.manual) 
            {
                return dataPath;
            }
            else if(parentContainer == null)
            {
                return DEFAULT_SAVE_PATH + dataName;
            }
            else// if(pathType == PathType.fullauto)
            {
                return parentContainer.GetDataPath() + "/" + dataName;
            }
        }
        public virtual void SetDataPath(string _dataPath)
        {
            dataPath = _dataPath;
        }
        public void SetParentContainer(D_Container container)
        {
            parentContainer = container;
        }
        public D_Container GetParentContainer()
        {
            return parentContainer;
        }
        /// <summary>
        /// Override this method to display data in DC. Method should return fields values 
        /// </summary>
        /// <returns></returns>
        public virtual string GetDataDescription()
        {
            return "---";
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

#if UNITY_EDITOR
        bool IsShowMessageField()
        {
            bool b1 = (broadcastEvent & BroadcastEvent.message) > 0;
            bool b2 = (broadcastEvent & BroadcastEvent.messageDataBase) > 0;
            bool res = b1 || b2;
            return res;
        }
        bool IsShowPathField()
        {
            return pathType == PathType.manual;
        }
#endif

        public static string DEFAULT_SAVE_PATH = "Default/";
    }
    public enum DataType { /*read, */readWrite, readWriteSave };
    public enum PathType { fullauto, manual/*, relativeAuto*/ };

    [System.Flags]
    public enum BroadcastEvent {
        message = 1 << 1,
        messageDataBase = 1 << 2,
    };
   
}
