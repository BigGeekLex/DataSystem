using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MSFD.Data;

namespace MSFD
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "_D_Container_", menuName = "DataSystem/D_Container")]
    public class D_Container_SO : DisplayDataBase
    {
        [ListDrawerSettings(ShowPaging = true)]
        [TabGroup("Values")]
        [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.Foldout)]
        [SerializeField]
        List<DisplayDataBase> baseData;

        [ListDrawerSettings(ShowPaging = true)]
        [FoldoutGroup("Triggers")]
        [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.Foldout)]
        [SerializeField]
        List<DisplayTriggerBase> triggerBases;

        [ListDrawerSettings(ShowPaging = true)]
        [FoldoutGroup("Triggers")]
        [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.Foldout)]
        [SerializeField]
        List<TriggerEditorBase> triggerEditorBases;
        //[HorizontalGroup("System")]

        [TabGroup("System")]
        [HideLabel]
        [SerializeField]
        D_Container refContainer = new D_Container();
        [System.NonSerialized]
        D_Container d_value;
        [System.NonSerialized]
        bool isDataRegistered = false;
        public override DataBase GetDataBase()
        {
            TryInitializeContainer();
            return d_value;
        }
        public D_Container GetData()
        {
            TryInitializeContainer();
            return d_value;
        }
        public static implicit operator D_Container(D_Container_SO display)
        {            
            return display.GetData();
        }

        public override void RegisterData(D_Container container)
        {
            TryInitializeContainer();
            container.TryAddData(d_value);
        }
        void TryInitializeContainer()
        {
            if(isDataRegistered)
            {
                return;
            }
            else
            {
                isDataRegistered = true;
                d_value = new D_Container(GetDataName(), refContainer.GetDataType());
                d_value.SetBroadcastEvent(refContainer.GetBroadcastEvent());
                d_value.TriggerProcessor.InitializeTriggers(triggerBases);

                d_value.SetPathType(refContainer.GetPathType());
                d_value.SetDataPath(refContainer.GetDataPath());
                
                foreach (DisplayDataBase x in baseData)
                {
                    x.RegisterData(d_value);
                }
            }
        }
        void CorrectSOReferences()
        {
            AuxiliarySystem.RemoveDuplicateReferences(baseData);

            AuxiliarySystem.RemoveNullReferencesSO(baseData.ConvertAll((x) => x as ScriptableObject));
            AuxiliarySystem.RemoveNullReferencesSO(triggerBases.ConvertAll((x) => x as ScriptableObject));
            AuxiliarySystem.RemoveNullReferencesSO(triggerEditorBases.ConvertAll((x) => x as ScriptableObject));
        }
        [Button]
        public void ActivateTriggerVerification()
        {
            CorrectSOReferences();
            TryInitializeContainer();
            foreach(TriggerEditorBase x in triggerEditorBases)
            {
                x.ActivateTriggerEditor(this);
            }
            d_value.TriggerProcessor.InitializeTriggers(triggerBases);
            d_value.TriggerProcessor.ActivateTriggerVerification(d_value);
        }
        public List<DisplayDataBase> GetDisplayDataBases()
        {
            return baseData;
        }
        public List<DisplayTriggerBase> GetDisplayTriggerBases()
        {
            return triggerBases;
        }
        public List<TriggerEditorBase> GetTriggerEditorBases()
        {
            return triggerEditorBases;
        }
        public void AddDisplayDataBase(DisplayDataBase displayDataBase)
        {
            baseData.Add(displayDataBase);
        }

        bool traversalFlag = false;

        [Button]
        public void ActivateRecoursiveTriggerVerification()
        {
            if(!traversalFlag)
            {
                traversalFlag = true;
                ActivateTriggerVerification();
                foreach (var dataDisplay in baseData)
                {
                    D_Container_SO container = dataDisplay as D_Container_SO;
                    if (container != null)
                    {
                        container.ActivateRecoursiveTriggerVerification();
                    }
                }
                ResetTraversalFlagRecoursively();
            }
        }
        public void ResetTraversalFlagRecoursively()
        {
            if (traversalFlag)
            {
                traversalFlag = false;
                foreach (var dataDisplay in baseData)
                {
                    D_Container_SO container = dataDisplay as D_Container_SO;
                    if (container != null)
                    {
                        container.ResetTraversalFlagRecoursively();
                    }
                }
            }
        }
    }
}