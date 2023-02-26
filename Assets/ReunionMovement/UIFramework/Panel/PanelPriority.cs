using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 定义面板将作为父级到哪个副层
    /// </summary>
    public enum PanelPriority
    {
        None = 0,
        Prioritary = 1,
        Tutorial = 2,
        Blocker = 3,
    }

    [System.Serializable]
    public class PanelPriorityLayerListEntry
    {
        [SerializeField]
        [Tooltip("给定目标副层的面板优先级类型")]
        private PanelPriority priority;
        [SerializeField]
        [Tooltip("应该包含所有标记有此优先级的面板的游戏对象")]
        private Transform targetParent;

        public Transform TargetParent
        {
            get { return targetParent; }
            set { targetParent = value; }
        }

        public PanelPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public PanelPriorityLayerListEntry(PanelPriority prio, Transform parent)
        {
            priority = prio;
            targetParent = parent;
        }
    }

    [System.Serializable]
    public class PanelPriorityLayerList
    {
        [SerializeField]
        [Tooltip("根据优先级查找游戏对象以存储面板。渲染优先级由这些游戏对象的层次顺序设置")]
        private List<PanelPriorityLayerListEntry> paraLayers = null;

        private Dictionary<PanelPriority, Transform> lookup;

        public Dictionary<PanelPriority, Transform> ParaLayerLookup
        {
            get
            {
                if (lookup == null || lookup.Count == 0)
                {
                    CacheLookup();
                }

                return lookup;
            }
        }

        /// <summary>
        /// 缓存查找
        /// </summary>
        private void CacheLookup()
        {
            lookup = new Dictionary<PanelPriority, Transform>();
            for (int i = 0; i < paraLayers.Count; i++)
            {
                lookup.Add(paraLayers[i].Priority, paraLayers[i].TargetParent);
            }
        }

        public PanelPriorityLayerList(List<PanelPriorityLayerListEntry> entries)
        {
            paraLayers = entries;
        }
    }
}