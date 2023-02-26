namespace GameLogic.UIFramework
{
    /// <summary>
    /// 不需要特殊属性的面板的基类
    /// </summary>
    public abstract class APanelController : APanelController<PanelProperties> { }

    /// <summary>
    /// 面板的基类
    /// </summary>
    public abstract class APanelController<T> : AUIScreenController<T>, IPanelController where T : IPanelProperties
    {
        public PanelPriority Priority
        {
            get
            {
                if (Properties != null)
                {
                    return Properties.Priority;
                }
                else
                {
                    return PanelPriority.None;
                }
            }
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="props"></param>
        protected sealed override void SetProperties(T props)
        {
            base.SetProperties(props);
        }
    }
}
