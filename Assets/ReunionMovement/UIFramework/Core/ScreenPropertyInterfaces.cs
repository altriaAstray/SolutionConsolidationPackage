namespace GameLogic.UIFramework
{
    /// <summary>
    /// 所有屏幕属性的基本接口
    /// </summary>
    public interface IScreenProperties { }

    /// <summary>
    /// 面板属性的基本接口
    /// </summary>
    public interface IPanelProperties : IScreenProperties
    {
        PanelPriority Priority { get; set; }
    }

    /// <summary>
    /// 窗口属性的基本接口
    /// </summary>
    public interface IWindowProperties : IScreenProperties
    {
        WindowPriority WindowQueuePriority { get; set; }
        bool HideOnForegroundLost { get; set; }
        bool IsPopup { get; set; }
        bool SuppressPrefabProperties { get; set; }
    }
}
