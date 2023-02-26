using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// Template for an UI. You can rig the prefab for the UI Frame itself and all the screens that should
    /// be instanced and registered upon instantiating a new UI Frame.
    /// UI模板。您可以为UI框架本身以及在实例化新UI框架时应实例化和注册的所有屏幕装配预制板。
    /// </summary>

    [CreateAssetMenu(fileName = "UISettings", menuName = "UI框架/UI设置")]
    public class UISettings : ScriptableObject
    {
        [Tooltip("UI框架结构本身的预制件")]
        [SerializeField] private UIFrame templateUIPrefab = null;
        [Tooltip("实例化 UI 时要实例化和注册的所有屏幕（面板和窗口）的预制件")]
        [SerializeField] private List<GameObject> screensToRegister = null;
        [Tooltip("如果屏幕预制件未停用，系统是否应在实例化时自动停用其游戏对象？ 如果为 false，屏幕将在实例化时处于可见状态。")]
        [SerializeField] private bool deactivateScreenGOs = true;

        /// <summary>
        /// 创建UI框架预置的实例。
        /// 默认情况下，也实例化列出并注册所有屏幕。
        /// 如果deactivateScreenGO标志为true，它将停用所有屏幕游戏对象，以防它们处于活动状态。
        /// </summary>
        /// <param name="instanceAndRegisterScreens">是否应该实例化和注册设置文件中列出的屏幕？</param>
        /// <returns>A new UI Frame</returns>
        public UIFrame CreateUIInstance(bool instanceAndRegisterScreens = true)
        {
            var newUI = Instantiate(templateUIPrefab);

            if (instanceAndRegisterScreens)
            {
                foreach (var screen in screensToRegister)
                {
                    var screenInstance = Instantiate(screen);
                    var screenController = screenInstance.GetComponent<IUIScreenController>();

                    if (screenController != null)
                    {
                        newUI.RegisterScreen(screen.name, screenController, screenInstance.transform);
                        if (deactivateScreenGOs && screenInstance.activeSelf)
                        {
                            screenInstance.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogError("[UIConfig] 屏幕不包含ScreenController！跳过" + screen.name);
                    }
                }
            }

            return newUI;
        }

        //启用验证
        private void OnValidate()
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            for (int i = 0; i < screensToRegister.Count; i++)
            {
                var screenCtl = screensToRegister[i].GetComponent<IUIScreenController>();
                if (screenCtl == null)
                {
                    objectsToRemove.Add(screensToRegister[i]);
                }
            }

            if (objectsToRemove.Count > 0)
            {
                Debug.LogError("[UISettings]一些添加到屏幕预设列表的游戏对象没有附加屏幕控制器! 删除.");
                foreach (var obj in objectsToRemove)
                {
                    Debug.LogError("[UISettings] 删除 " + obj.name + " 从 " + name + " 因为它没有附加屏幕控制器!");
                    screensToRegister.Remove(obj);
                }
            }
        }
    }
}
