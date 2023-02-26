using UnityEngine;
using System.Collections.Generic;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// UI层的基类。层在打开、关闭等时为屏幕类型实现自定义逻辑。
    /// </summary>
    public abstract class AUILayer<TScreen> : MonoBehaviour where TScreen : IUIScreenController
    {
        protected Dictionary<string, TScreen> registeredScreens;

        /// <summary>
        /// 显示屏幕
        /// </summary>
        /// <param name="screen">要显示的屏幕控制器</param>
        public abstract void ShowScreen(TScreen screen);

        /// <summary>
        /// 根据属性显示屏幕
        /// </summary>
        /// <param name="screen">The ScreenController to show</param>
        /// <param name="properties">The data payload</param>
        /// <typeparam name="TProps">数据类型</typeparam>
        public abstract void ShowScreen<TProps>(TScreen screen, TProps properties) where TProps : IScreenProperties;

        /// <summary>
        /// 隐藏屏幕
        /// </summary>
        /// <param name="screen">要隐藏的ScreenController</param>
        public abstract void HideScreen(TScreen screen);

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            registeredScreens = new Dictionary<string, TScreen>();
        }

        /// <summary>
        /// 注册屏幕到此层的Transform
        /// </summary>
        /// <param name="controller">屏幕控制器</param>
        /// <param name="screenTransform">父对象</param>
        public virtual void ReparentScreen(IUIScreenController controller, Transform screenTransform)
        {
            screenTransform.SetParent(transform, false);
        }

        /// <summary>
        /// 将 ScreenController 注册到特定的 ScreenId
        /// </summary>
        /// <param name="screenId">目标ScreenId</param>
        /// <param name="controller">要注册的屏幕控制器</param>
        public void RegisterScreen(string screenId, TScreen controller)
        {
            if (!registeredScreens.ContainsKey(screenId))
            {
                ProcessScreenRegister(screenId, controller);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen controller already registered for id: " + screenId);
            }
        }

        /// <summary>
        /// 根据ScreenId注销屏幕
        /// </summary>
        /// <param name="screenId">ScreenId</param>
        /// <param name="controller">要注销的控制器</param>
        public void UnregisterScreen(string screenId, TScreen controller)
        {
            if (registeredScreens.ContainsKey(screenId))
            {
                ProcessScreenUnregister(screenId, controller);
            }
            else
            {
                Debug.LogError("[AUILayerController] Screen controller not registered for id: " + screenId);
            }
        }

        /// <summary>
        /// 尝试找到与ID匹配的已注册屏幕并显示它。
        /// </summary>
        /// <param name="screenId">所需的ScreenId</param>
        public void ShowScreenById(string screenId)
        {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenId, out ctl))
            {
                ShowScreen(ctl);
            }
            else
            {
                Debug.LogError("[AUILayerController] ScreenId [" + screenId + "]没有注册到此层!");
            }
        }

        /// <summary>
        /// 尝试找到与ScreenId匹配的注册屏幕并显示它，传递数据负载。
        /// </summary>
        /// <param name="screenId">此屏幕的ID（默认情况下，它是预置的名称）</param>
        /// <param name="properties">此屏幕要使用的数据负载</param>
        /// <typeparam name="TProps">该屏幕使用的属性类的类型</typeparam>
        public void ShowScreenById<TProps>(string screenId, TProps properties) where TProps : IScreenProperties
        {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenId, out ctl))
            {
                ShowScreen(ctl, properties);
            }
            else
            {
                Debug.LogError("[AUILayerController] ScreenId [" + screenId + "]没有注册!");
            }
        }

        /// <summary>
        /// 尝试查找与ScreenId匹配的注册屏幕并将其隐藏
        /// </summary>
        /// <param name="screenId">此屏幕的ID（默认情况下，它是预置的名称）</param>
        public void HideScreenById(string screenId)
        {
            TScreen ctl;
            if (registeredScreens.TryGetValue(screenId, out ctl))
            {
                HideScreen(ctl);
            }
            else
            {
                Debug.LogError("[AUILayerController] 不能隐藏ID为[" + screenId + "]的屏幕，因为没有注册到此层！");
            }
        }

        /// <summary>
        /// 检查屏幕是否已注册到此UI层
        /// </summary>
        /// <param name="screenId">ScreenId（默认情况下，它是预制件的名称）</param>
        /// <returns>如果屏幕已注册，则为Ture，否则为False</returns>
        public bool IsScreenRegistered(string screenId)
        {
            return registeredScreens.ContainsKey(screenId);
        }

        /// <summary>
        /// 隐藏所有注册在此层的屏幕
        /// </summary>
        /// <param name="shouldAnimateWhenHiding">隐藏时播放过度动画</param>
        public virtual void HideAll(bool shouldAnimateWhenHiding = true)
        {
            foreach (var screen in registeredScreens)
            {
                screen.Value.Hide(shouldAnimateWhenHiding);
            }
        }
        /// <summary>
        /// 处理注册屏幕
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        protected virtual void ProcessScreenRegister(string screenId, TScreen controller)
        {
            controller.ScreenId = screenId;
            registeredScreens.Add(screenId, controller);
            controller.ScreenDestroyed += OnScreenDestroyed;
        }
        /// <summary>
        /// 处理注销屏幕
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        protected virtual void ProcessScreenUnregister(string screenId, TScreen controller)
        {
            controller.ScreenDestroyed -= OnScreenDestroyed;
            registeredScreens.Remove(screenId);
        }
        /// <summary>
        /// 屏幕销毁
        /// </summary>
        /// <param name="screen"></param>
        private void OnScreenDestroyed(IUIScreenController screen)
        {
            if (!string.IsNullOrEmpty(screen.ScreenId) && registeredScreens.ContainsKey(screen.ScreenId))
            {
                UnregisterScreen(screen.ScreenId, (TScreen)screen);
            }
        }
    }
}
