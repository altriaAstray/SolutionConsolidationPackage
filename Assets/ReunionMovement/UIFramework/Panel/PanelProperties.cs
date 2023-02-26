using UnityEngine;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 所有面板通用的属性
    /// </summary>
    [System.Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField]
        [Tooltip("根据其优先级，面板将转到不同的副层。可以在“面板图层”中设置副层。")]
        private PanelPriority priority;

        public PanelPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
