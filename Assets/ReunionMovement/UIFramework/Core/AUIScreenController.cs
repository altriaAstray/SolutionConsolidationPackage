using UnityEngine;
using System;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// Base implementation for UI Screens. You'll probably want to inherit
    /// from one of its child classes: AWindowController or APanelController, not this.
    /// <seealso cref="AWindowController"/>
    /// <seealso cref="APanelController"/>
    /// </summary>
    public abstract class AUIScreenController<TProps> : MonoBehaviour, IUIScreenController
        where TProps : IScreenProperties
    {
        [Header("屏幕动画")]
        [Tooltip("显示屏幕的动画")]
        [SerializeField]
        private ATransitionComponent animIn;

        [Tooltip("隐藏屏幕的动画")]
        [SerializeField]
        private ATransitionComponent animOut;

        [Header("屏幕属性")]
        [Tooltip("这是此屏幕的数据有效负载和设置。您可以直接在预制板中装配和/或在显示此屏幕时传递它")]
        [SerializeField]
        private TProps properties;

        /// <summary>
        /// 此ID的唯一标识符。如果使用默认系统，则其名称应与屏幕的前言相同。
        /// </summary>
        public string ScreenId { get; set; }

        /// <summary>
        /// 显示动画的过渡组件
        /// </summary>
        public ATransitionComponent AnimIn
        {
            get { return animIn; }
            set { animIn = value; }
        }

        /// <summary>
        /// 隐藏动画的过渡组件
        /// </summary>
        public ATransitionComponent AnimOut
        {
            get { return animOut; }
            set { animOut = value; }
        }

        /// <summary>
        /// 在“in”转换完成时发生。
        /// </summary>
        public Action<IUIScreenController> InTransitionFinished { get; set; }

        /// <summary>
        /// 在“out”转换完成时发生。
        /// </summary>
        public Action<IUIScreenController> OutTransitionFinished { get; set; }

        /// <summary>
        /// 屏幕可以触发此事件以请求其负责层关闭它
        /// </summary>
        /// <value>The close request.</value>
        public Action<IUIScreenController> CloseRequest { get; set; }

        /// <summary>
        /// 如果此屏幕由于某种原因被破坏，则必须警告其层
        /// </summary>
        /// <value>The destruction action.</value>
        public Action<IUIScreenController> ScreenDestroyed { get; set; }

        /// <summary>
        /// 此屏幕当前可见吗？
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// 此屏幕的属性。可以包含序列化值，也可以传入私有值。
        /// </summary>
        /// <value>The properties.</value>
        protected TProps Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            AddListeners();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (ScreenDestroyed != null)
            {
                ScreenDestroyed(this);
            }

            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequest = null;
            ScreenDestroyed = null;
            RemoveListeners();
        }

        /// <summary>
        /// 用于设置事件/消息的所有侦听器。默认情况下，调用Awake（）
        /// </summary>
        protected virtual void AddListeners()
        {
        }

        /// <summary>
        /// 用于删除事件/消息的所有侦听器。默认情况下，调用OnDestroy（）
        /// </summary>
        protected virtual void RemoveListeners()
        {
        }

        /// <summary>
        /// 为此屏幕设置属性时，将调用此方法。此时，您可以安全地访问属性
        /// </summary>
        protected virtual void OnPropertiesSet()
        {
        }

        /// <summary>
        /// 当屏幕显示动画时，立即调用
        /// </summary>
        protected virtual void WhileHiding()
        {
        }

        /// <summary>
        /// 设置属性时，将调用此方法。这样，您可以在特定条件下扩展财产的使用。
        /// </summary>
        /// <param name="props">Properties.</param>
        protected virtual void SetProperties(TProps props)
        {
            properties = props;
        }

        /// <summary>
        /// 如果调整层次结构时，屏幕有任何特殊行为需要调用
        /// </summary>
        protected virtual void HierarchyFixOnShow()
        {
        }

        /// <summary>
        /// 隐藏此屏幕
        /// </summary>
        /// <param name="animate">Should animation be played? (defaults to true)</param>
        public void Hide(bool animate = true)
        {
            DoAnimation(animate ? animOut : null, OnTransitionOutFinished, false);
            WhileHiding();
        }

        /// <summary>
        /// 使用指定的属性显示此屏幕。
        /// </summary>
        /// <param name="props">屏幕数据</param>
        public void Show(IScreenProperties props = null)
        {
            if (props != null)
            {
                if (props is TProps)
                {
                    SetProperties((TProps)props);
                }
                else
                {
                    Debug.LogError("传递的属性类型错误! (" + props.GetType() + " 而不是 " +
                                   typeof(TProps) + ")");
                    return;
                }
            }

            HierarchyFixOnShow();
            OnPropertiesSet();

            if (!gameObject.activeSelf)
            {
                DoAnimation(animIn, OnTransitionInFinished, true);
            }
            else
            {
                if (InTransitionFinished != null)
                {
                    InTransitionFinished(this);
                }
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="callWhenFinished"></param>
        /// <param name="isVisible"></param>
        private void DoAnimation(ATransitionComponent caller, Action callWhenFinished, bool isVisible)
        {
            if (caller == null)
            {
                gameObject.SetActive(isVisible);
                if (callWhenFinished != null)
                {
                    callWhenFinished();
                }
            }
            else
            {
                if (isVisible && !gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                caller.Animate(transform, callWhenFinished);
            }
        }

        /// <summary>
        /// 播放过度动画[进入]完成
        /// </summary>
        private void OnTransitionInFinished()
        {
            IsVisible = true;

            if (InTransitionFinished != null)
            {
                InTransitionFinished(this);
            }
        }

        /// <summary>
        /// 播放过度动画[退出]完成
        /// </summary>
        private void OnTransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            if (OutTransitionFinished != null)
            {
                OutTransitionFinished(this);
            }
        }
    }
}
