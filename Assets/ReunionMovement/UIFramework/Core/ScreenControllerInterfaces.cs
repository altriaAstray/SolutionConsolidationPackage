using System;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 所有UI屏幕必须直接或间接实现的界面
    /// </summary>
    public interface IUIScreenController
    {
        string ScreenId { get; set; }
        bool IsVisible { get; }

        void Show(IScreenProperties props = null);
        void Hide(bool animate = true);

        Action<IUIScreenController> InTransitionFinished { get; set; }
        Action<IUIScreenController> OutTransitionFinished { get; set; }
        Action<IUIScreenController> CloseRequest { get; set; }
        Action<IUIScreenController> ScreenDestroyed { get; set; }
    }

    /// <summary>
    /// 所有窗口必须实现的接口
    /// </summary>
    public interface IWindowController : IUIScreenController
    {
        bool HideOnForegroundLost { get; }
        bool IsPopup { get; }
        WindowPriority WindowPriority { get; }
    }

    /// <summary>
    /// 所有面板必须实现的接口
    /// </summary>
    public interface IPanelController : IUIScreenController
    {
        PanelPriority Priority { get; }
    }
}
