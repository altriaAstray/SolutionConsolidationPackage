using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 此层控制所有窗口。Windows是遵循历史和队列的屏幕，一次显示一个（可能是模态也可能不是模态）。这还包括弹出窗口。
    /// </summary>
    public class WindowUILayer : AUILayer<IWindowController>
    {
        [SerializeField] private WindowParaLayer priorityParaLayer = null;

        public IWindowController CurrentWindow { get; private set; }

        private Queue<WindowHistoryEntry> windowQueue;
        private Stack<WindowHistoryEntry> windowHistory;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnblock;

        private bool IsScreenTransitionInProgress
        {
            get { return screensTransitioning.Count != 0; }
        }

        private HashSet<IUIScreenController> screensTransitioning;
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            registeredScreens = new Dictionary<string, IWindowController>();
            windowQueue = new Queue<WindowHistoryEntry>();
            windowHistory = new Stack<WindowHistoryEntry>();
            screensTransitioning = new HashSet<IUIScreenController>();
        }
        /// <summary>
        /// 处理屏幕注册
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        protected override void ProcessScreenRegister(string screenId, IWindowController controller)
        {
            base.ProcessScreenRegister(screenId, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByWindow;
        }
        /// <summary>
        /// 处理屏幕注销
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        protected override void ProcessScreenUnregister(string screenId, IWindowController controller)
        {
            base.ProcessScreenUnregister(screenId, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByWindow;
        }
        /// <summary>
        /// 显示屏幕
        /// </summary>
        /// <param name="screen"></param>
        public override void ShowScreen(IWindowController screen)
        {
            ShowScreen<IWindowProperties>(screen, null);
        }
        /// <summary>
        /// 显示屏幕
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="screen"></param>
        /// <param name="properties"></param>
        public override void ShowScreen<TProp>(IWindowController screen, TProp properties)
        {
            IWindowProperties windowProp = properties as IWindowProperties;

            if (ShouldEnqueue(screen, windowProp))
            {
                EnqueueWindow(screen, properties);
            }
            else
            {
                DoShow(screen, windowProp);
            }
        }
        /// <summary>
        /// 隐藏屏幕
        /// </summary>
        /// <param name="screen"></param>
        public override void HideScreen(IWindowController screen)
        {
            if (screen == CurrentWindow)
            {
                windowHistory.Pop();
                AddTransition(screen);
                screen.Hide();

                CurrentWindow = null;

                if (windowQueue.Count > 0)
                {
                    ShowNextInQueue();
                }
                else if (windowHistory.Count > 0)
                {
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogError(
                    string.Format(
                        "[WindowUILayer] Hide requested on WindowId {0} but that's not the currently open one ({1})! Ignoring request.",
                        screen.ScreenId, CurrentWindow != null ? CurrentWindow.ScreenId : "current is null"));
            }
        }
        /// <summary>
        /// 全部隐藏
        /// </summary>
        /// <param name="shouldAnimateWhenHiding"></param>
        public override void HideAll(bool shouldAnimateWhenHiding = true)
        {
            base.HideAll(shouldAnimateWhenHiding);
            CurrentWindow = null;
            priorityParaLayer.RefreshDarken();
            windowHistory.Clear();
        }

        /// <summary>
        /// 重定屏幕父级
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="screenTransform"></param>
        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;

            if (window == null)
            {
                Debug.LogError("[WindowUILayer] Screen " + screenTransform.name + " is not a Window!");
            }
            else
            {
                if (window.IsPopup)
                {
                    priorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }

            base.ReparentScreen(controller, screenTransform);
        }
        /// <summary>
        /// 排队窗口
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="screen"></param>
        /// <param name="properties"></param>
        private void EnqueueWindow<TProp>(IWindowController screen, TProp properties) where TProp : IScreenProperties
        {
            windowQueue.Enqueue(new WindowHistoryEntry(screen, (IWindowProperties)properties));
        }
        /// <summary>
        /// 是否在排队
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="windowProp"></param>
        /// <returns></returns>
        private bool ShouldEnqueue(IWindowController controller, IWindowProperties windowProp)
        {
            if (CurrentWindow == null && windowQueue.Count == 0)
            {
                return false;
            }

            if (windowProp != null && windowProp.SuppressPrefabProperties)
            {
                return windowProp.WindowQueuePriority != WindowPriority.ForceForeground;
            }

            if (controller.WindowPriority != WindowPriority.ForceForeground)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 显示历史记录中的上一个
        /// </summary>
        private void ShowPreviousInHistory()
        {
            if (windowHistory.Count > 0)
            {
                WindowHistoryEntry window = windowHistory.Pop();
                DoShow(window);
            }
        }
        /// <summary>
        /// 显示队列中的下一个
        /// </summary>
        private void ShowNextInQueue()
        {
            if (windowQueue.Count > 0)
            {
                WindowHistoryEntry window = windowQueue.Dequeue();
                DoShow(window);
            }
        }
        /// <summary>
        /// 动画显示
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="properties"></param>
        private void DoShow(IWindowController screen, IWindowProperties properties)
        {
            DoShow(new WindowHistoryEntry(screen, properties));
        }
        /// <summary>
        /// 动画显示
        /// </summary>
        /// <param name="windowEntry"></param>
        private void DoShow(WindowHistoryEntry windowEntry)
        {
            if (CurrentWindow == windowEntry.Screen)
            {
                Debug.LogWarning(
                    string.Format(
                        "[WindowUILayer] The requested WindowId ({0}) is already open! This will add a duplicate to the " +
                        "history and might cause inconsistent behaviour. It is recommended that if you need to open the same" +
                        "screen multiple times (eg: when implementing a warning message pop-up), it closes itself upon the player input" +
                        "that triggers the continuation of the flow."
                        , CurrentWindow.ScreenId));
            }
            else if (CurrentWindow != null
                     && CurrentWindow.HideOnForegroundLost
                     && !windowEntry.Screen.IsPopup)
            {
                CurrentWindow.Hide();
            }

            windowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);

            if (windowEntry.Screen.IsPopup)
            {
                priorityParaLayer.DarkenBG();
            }

            windowEntry.Show();

            CurrentWindow = windowEntry.Screen;
        }
        /// <summary>
        /// In 动画完成
        /// </summary>
        /// <param name="screen"></param>
        private void OnInAnimationFinished(IUIScreenController screen)
        {
            RemoveTransition(screen);
        }
        /// <summary>
        /// Out 动画完成
        /// </summary>
        /// <param name="screen"></param>
        private void OnOutAnimationFinished(IUIScreenController screen)
        {
            RemoveTransition(screen);
            var window = screen as IWindowController;
            if (window.IsPopup)
            {
                priorityParaLayer.RefreshDarken();
            }
        }
        /// <summary>
        /// 窗口关闭请求
        /// </summary>
        /// <param name="screen"></param>
        private void OnCloseRequestedByWindow(IUIScreenController screen)
        {
            HideScreen(screen as IWindowController);
        }
        /// <summary>
        /// 添加过度
        /// </summary>
        /// <param name="screen"></param>
        private void AddTransition(IUIScreenController screen)
        {
            screensTransitioning.Add(screen);
            if (RequestScreenBlock != null)
            {
                RequestScreenBlock();
            }
        }
        /// <summary>
        /// 删除过渡
        /// </summary>
        /// <param name="screen"></param>
        private void RemoveTransition(IUIScreenController screen)
        {
            screensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                if (RequestScreenUnblock != null)
                {
                    RequestScreenUnblock();
                }
            }
        }
    }
}
