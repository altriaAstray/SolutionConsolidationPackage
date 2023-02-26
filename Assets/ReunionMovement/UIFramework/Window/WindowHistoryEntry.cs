namespace GameLogic.UIFramework
{
    /// <summary>
    /// 用于控制窗口历史记录和队列的入口
    /// </summary>
    public struct WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IWindowProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IWindowProperties properties)
        {
            Screen = screen;
            Properties = properties;
        }
        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            Screen.Show(Properties);
        }
    }
}
