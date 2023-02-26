using UnityEngine;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 所有窗口的通用属性
    /// </summary>
    [System.Serializable]
    public class WindowProperties : IWindowProperties
    {
        [SerializeField]
        protected bool hideOnForegroundLost = true;

        [SerializeField]
        protected WindowPriority windowQueuePriority = WindowPriority.ForceForeground;

        [SerializeField]
        protected bool isPopup = false;

        public WindowProperties()
        {
            hideOnForegroundLost = true;
            windowQueuePriority = WindowPriority.ForceForeground;
            isPopup = false;
        }

        /// <summary>
        /// 如果另一个窗口已打开，该窗口应如何运行？
        /// </summary>
        /// <value>Force Foreground opens it immediately, Enqueue queues it so that it's opened as soon as
        /// the current one is closed. </value>
        public WindowPriority WindowQueuePriority
        {
            get { return windowQueuePriority; }
            set { windowQueuePriority = value; }
        }

        /// <summary>
        /// 当其他窗口采用其前景时，是否应隐藏此窗口？
        /// </summary>
        /// <value><c>true</c> if hide on foreground lost; otherwise, <c>false</c>.</value>
        public bool HideOnForegroundLost
        {
            get { return hideOnForegroundLost; }
            set { hideOnForegroundLost = value; }
        }

        /// <summary>
        /// 在Open（）调用中传递属性时，是否应该覆盖viewPrefab中配置的属性？
        /// </summary>
        /// <value><c>true</c> if suppress viewPrefab properties; otherwise, <c>false</c>.</value>
        public bool SuppressPrefabProperties { get; set; }

        /// <summary>
        /// 弹出窗口在其后面和所有其他窗口前面显示为黑色背景
        /// </summary>
        /// <value><c>true</c> if this window is a popup; otherwise, <c>false</c>.</value>
        public bool IsPopup
        {
            get { return isPopup; }
            set { isPopup = value; }
        }

        public WindowProperties(bool suppressPrefabProperties = false)
        {
            WindowQueuePriority = WindowPriority.ForceForeground;
            HideOnForegroundLost = false;
            SuppressPrefabProperties = suppressPrefabProperties;
        }

        public WindowProperties(WindowPriority priority, bool hideOnForegroundLost = false, bool suppressPrefabProperties = false)
        {
            WindowQueuePriority = priority;
            HideOnForegroundLost = hideOnForegroundLost;
            SuppressPrefabProperties = suppressPrefabProperties;
        }
    }
}
