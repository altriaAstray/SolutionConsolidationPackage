using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 该层控制面板。面板是没有历史或排队的屏幕，它们只显示和隐藏在框架中，例如：HUD、能量条、迷你地图等。
    /// </summary>
    public class PanelUILayer : AUILayer<IPanelController>
    {
        [SerializeField]
        [Tooltip("Settings for the priority para-layers. A Panel registered to this layer will be reparented to a different para-layer object depending on its Priority.")]
        private PanelPriorityLayerList priorityLayers = null;

        /// <summary>
        /// 重定屏幕父级
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="screenTransform"></param>
        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform)
        {
            var ctl = controller as IPanelController;
            if (ctl != null)
            {
                ReparentToParaLayer(ctl.Priority, screenTransform);
            }
            else
            {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        /// <summary>
        /// 显示屏幕
        /// </summary>
        /// <param name="screen"></param>
        public override void ShowScreen(IPanelController screen)
        {
            screen.Show();
        }

        /// <summary>
        /// 显示屏幕
        /// </summary>
        /// <typeparam name="TProps"></typeparam>
        /// <param name="screen"></param>
        /// <param name="properties"></param>
        public override void ShowScreen<TProps>(IPanelController screen, TProps properties)
        {
            screen.Show(properties);
        }

        /// <summary>
        /// 隐藏屏幕
        /// </summary>
        /// <param name="screen"></param>
        public override void HideScreen(IPanelController screen)
        {
            screen.Hide();
        }

        /// <summary>
        /// 面板可见吗
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public bool IsPanelVisible(string panelId)
        {
            IPanelController panel;
            if (registeredScreens.TryGetValue(panelId, out panel))
            {
                return panel.IsVisible;
            }

            return false;
        }
        /// <summary>
        /// 重新绘制到副层
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="screenTransform"></param>
        private void ReparentToParaLayer(PanelPriority priority, Transform screenTransform)
        {
            Transform trans;
            if (!priorityLayers.ParaLayerLookup.TryGetValue(priority, out trans))
            {
                trans = transform;
            }

            screenTransform.SetParent(trans, false);
        }
    }
}
