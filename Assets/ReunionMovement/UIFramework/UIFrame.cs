using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 这是所有UI的集中访问点。你所有的通讯都应该针对这个
    /// </summary>
    public class UIFrame : MonoBehaviour
    {
        [Tooltip("如果要手动初始化此UI框架，请将其设置为false。")]
        [SerializeField] private bool initializeOnAwake = true;

        private PanelUILayer panelLayer;
        private WindowUILayer windowLayer;

        private Canvas mainCanvas;
        private GraphicRaycaster graphicRaycaster;

        /// <summary>
        /// UI的主画布
        /// </summary>
        public Canvas MainCanvas
        {
            get
            {
                if (mainCanvas == null)
                {
                    mainCanvas = GetComponent<Canvas>();
                }

                return mainCanvas;
            }
        }

        /// <summary>
        /// 主UI画布使用的相机
        /// </summary>
        public Camera UICamera
        {
            get { return MainCanvas.worldCamera; }
        }

        private void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 初始化此UI框架。初始化包括初始化面板和窗口层。
        /// 尽管迄今为止我遇到的所有案例都被“窗口和面板”方法所涵盖，
        /// 我将其虚拟化，以防您需要额外的层或其他特殊的初始化。
        /// </summary>
        public virtual void Initialize()
        {
            if (panelLayer == null)
            {
                panelLayer = gameObject.GetComponentInChildren<PanelUILayer>(true);
                if (panelLayer == null)
                {
                    Debug.LogError("[UI Frame] UI框架缺少面板层!");
                }
                else
                {
                    panelLayer.Initialize();
                }
            }

            if (windowLayer == null)
            {
                windowLayer = gameObject.GetComponentInChildren<WindowUILayer>(true);
                if (panelLayer == null)
                {
                    Debug.LogError("[UI Frame] UI框架缺少窗口层!");
                }
                else
                {
                    windowLayer.Initialize();
                    windowLayer.RequestScreenBlock += OnRequestScreenBlock;
                    windowLayer.RequestScreenUnblock += OnRequestScreenUnblock;
                }
            }

            graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }

        /// <summary>
        ///根据ScreenId显示面板
        /// </summary>
        /// <param name="screenId">Panel Id</param>
        public void ShowPanel(string screenId)
        {
            panelLayer.ShowScreenById(screenId);
        }

        /// <summary>
        /// 根据ScreenId显示面板，传递参数。
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        /// <param name="properties">Properties.</param>
        /// <typeparam name="T">The type of properties to be passed in.</typeparam>
        /// <seealso cref="IPanelProperties"/>
        public void ShowPanel<T>(string screenId, T properties) where T : IPanelProperties
        {
            panelLayer.ShowScreenById<T>(screenId, properties);
        }

        /// <summary>
        /// 根据ID隐藏Panel
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        public void HidePanel(string screenId)
        {
            panelLayer.HideScreenById(screenId);
        }

        /// <summary>
        /// 通过ScreenId打开Window
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        public void OpenWindow(string screenId)
        {
            windowLayer.ShowScreenById(screenId);
        }

        /// <summary>
        /// 通过ScreenId关闭Window
        /// </summary>
        /// <param name="screenId">ScreenId</param>
        public void CloseWindow(string screenId)
        {
            windowLayer.HideScreenById(screenId);
        }

        /// <summary>
        /// 关闭当前打开的Window（如果有）
        /// </summary>
        public void CloseCurrentWindow()
        {
            if (windowLayer.CurrentWindow != null)
            {
                CloseWindow(windowLayer.CurrentWindow.ScreenId);
            }
        }

        /// <summary>
        /// 通过ScreenId打开Window，并传递参数
        /// </summary>
        /// <param name="screenId">Identifier.</param>
        /// <param name="properties">Properties.</param>
        /// <typeparam name="T">The type of properties to be passed in.</typeparam>
        /// <seealso cref="IWindowProperties"/>
        public void OpenWindow<T>(string screenId, T properties) where T : IWindowProperties
        {
            windowLayer.ShowScreenById<T>(screenId, properties);
        }

        /// <summary>
        /// 在层中根据ID搜索，如果找到则打开屏幕
        /// </summary>
        /// <param name="screenId">ScreenID</param>
        public void ShowScreen(string screenId)
        {
            Type type;
            if (IsScreenRegistered(screenId, out type))
            {
                if (type == typeof(IWindowController))
                {
                    OpenWindow(screenId);
                }
                else if (type == typeof(IPanelController))
                {
                    ShowPanel(screenId);
                }
            }
            else
            {
                Debug.LogError(string.Format("尝试打开屏幕id｛0｝，但它未注册为窗口或面板！",screenId));
            }
        }

        /// <summary>
        /// 注册屏幕。如果传递了Transform，则层将对其自身进行重新分配。屏幕只能在注册后显示。
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <param name="screenTransform">Screen transform. If not null, will be reparented to proper layer</param>
        public void RegisterScreen(string screenId, IUIScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window != null)
            {
                windowLayer.RegisterScreen(screenId, window);
                if (screenTransform != null)
                {
                    windowLayer.ReparentScreen(controller, screenTransform);
                }

                return;
            }

            IPanelController panel = controller as IPanelController;
            if (panel != null)
            {
                panelLayer.RegisterScreen(screenId, panel);
                if (screenTransform != null)
                {
                    panelLayer.ReparentScreen(controller, screenTransform);
                }
            }
        }

        /// <summary>
        /// 注册Panel，Panels只能在注册后打开
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TPanel">The Controller type.</typeparam>
        public void RegisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注销Panel
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TPanel">The Controller type.</typeparam>
        public void UnregisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.UnregisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注册Window，Windows只能在注册后打开
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TWindow">The Controller type.</typeparam>
        public void RegisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注销Window
        /// </summary>
        /// <param name="screenId">Screen identifier.</param>
        /// <param name="controller">Controller.</param>
        /// <typeparam name="TWindow">The Controller type.</typeparam>
        public void UnregisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.UnregisterScreen(screenId, controller);
        }

        /// <summary>
        /// 通过ID检查Panel是否打开
        /// </summary>
        /// <param name="panelId">PanelID</param>
        public bool IsPanelOpen(string panelId)
        {
            return panelLayer.IsPanelVisible(panelId);
        }

        /// <summary>
        /// 隐藏所有屏幕
        /// </summary>
        /// <param name="animate">隐藏时播放过度动画</param>
        public void HideAll(bool animate = true)
        {
            CloseAllWindows(animate);
            HideAllPanels(animate);
        }

        /// <summary>
        /// 隐藏Panel层所有屏幕
        /// </summary>
        /// <param name="animate">隐藏时播放过度动画</param>
        public void HideAllPanels(bool animate = true)
        {
            panelLayer.HideAll(animate);
        }

        /// <summary>
        /// 隐藏Window层所有屏幕
        /// </summary>
        /// <param name="animate">隐藏时播放过度动画</param>
        public void CloseAllWindows(bool animate = true)
        {
            windowLayer.HideAll(animate);
        }

        /// <summary>
        /// 检查ScreenId是否已注册到Window或Panel层
        /// </summary>
        /// <param name="screenId">要检查的ID</param>
        public bool IsScreenRegistered(string screenId)
        {
            if (windowLayer.IsScreenRegistered(screenId))
            {
                return true;
            }

            if (panelLayer.IsScreenRegistered(screenId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查ScreenId是否已注册到Window或Panel层，同时返回屏幕类型
        /// </summary>
        /// <param name="screenId">要检查的ID</param>
        /// <param name="type">屏幕的类型</param>
        public bool IsScreenRegistered(string screenId, out Type type)
        {
            if (windowLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IWindowController);
                return true;
            }

            if (panelLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IPanelController);
                return true;
            }

            type = null;
            return false;
        }

        /// <summary>
        /// 禁止屏幕交互
        /// </summary>
        private void OnRequestScreenBlock()
        {
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = false;
            }
        }

        /// <summary>
        /// 允许屏幕交互
        /// </summary>
        private void OnRequestScreenUnblock()
        {
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = true;
            }
        }
    }
}
