namespace GameLogic.UIFramework
{
    /// <summary>
    /// 不需要特殊属性的 Window ScreenControllers 的基本实现
    /// </summary>
    public abstract class AWindowController : AWindowController<WindowProperties> { }

    /// <summary>
    /// 窗口屏幕控制器的基本实现。 
    /// 它的参数是一个特定类型的 IWindowProperties。 
    /// 如果您的窗口不需要特殊属性，请从 AWindowScreenController 继承，不带 Generic 参数。
    /// <seealso cref="IWindowProperties"/>
    /// <seealso cref="AWindowController"/>
    /// </summary>
    public abstract class AWindowController<TProps> : AUIScreenController<TProps>, IWindowController
        where TProps : IWindowProperties
    {
        public bool HideOnForegroundLost
        {
            get { return Properties.HideOnForegroundLost; }
        }
        /// <summary>
        /// 是弹出窗口
        /// </summary>
        public bool IsPopup
        {
            get { return Properties.IsPopup; }
        }
        /// <summary>
        /// 窗口队列优先级
        /// </summary>
        public WindowPriority WindowPriority
        {
            get { return Properties.WindowQueuePriority; }
        }

        /// <summary>
        /// 请求关闭此窗口，便于直接在编辑器中装配它。
        /// 我使用 UI_ 前缀对所有应在编辑器中装配的方法进行分组，以便轻松找到特定于屏幕的方法。 
        /// 它打破了命名约定，但随着方法数量的增加利大于弊。 
        /// 这是*Not*每次关闭时调用，只是在用户输入时调用 - 对于该行为，
        /// 请参见 WhileHiding();
        /// </summary>
        public virtual void UI_Close()
        {
            CloseRequest(this);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="props"></param>
        protected sealed override void SetProperties(TProps props)
        {
            if (props != null)
            {
                // 如果预制件上设置的Properties不应该被覆盖，将默认值复制到传入的properties
                if (!props.SuppressPrefabProperties)
                {
                    props.HideOnForegroundLost = Properties.HideOnForegroundLost;
                    props.WindowQueuePriority = Properties.WindowQueuePriority;
                    props.IsPopup = Properties.IsPopup;
                }

                Properties = props;
            }
        }
        /// <summary>
        /// 层次结构固定显示
        /// </summary>
        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
    }
}
