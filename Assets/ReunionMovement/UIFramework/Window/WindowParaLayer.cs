using UnityEngine;
using System.Collections.Generic;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 这是一个副层，因此可以显示具有更高优先级的窗口。
    /// 默认情况下，它包含任何标记为 Popup 的窗口。 它由 WindowUILayer 控制。
    /// </summary>
    public class WindowParaLayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject darkenBgObject = null;

        private List<GameObject> containedScreens = new List<GameObject>();

        /// <summary>
        /// 添加屏幕
        /// </summary>
        /// <param name="screenRectTransform"></param>
        public void AddScreen(Transform screenRectTransform)
        {
            screenRectTransform.SetParent(transform, false);
            containedScreens.Add(screenRectTransform.gameObject);
        }

        /// <summary>
        /// 刷新变暗
        /// </summary>
        public void RefreshDarken()
        {
            for (int i = 0; i < containedScreens.Count; i++)
            {
                if (containedScreens[i] != null)
                {
                    if (containedScreens[i].activeSelf)
                    {
                        darkenBgObject.SetActive(true);
                        return;
                    }
                }
            }

            darkenBgObject.SetActive(false);
        }

        /// <summary>
        /// 背景变暗
        /// </summary>
        public void DarkenBG()
        {
            darkenBgObject.SetActive(true);
            darkenBgObject.transform.SetAsLastSibling();
        }
    }
}
