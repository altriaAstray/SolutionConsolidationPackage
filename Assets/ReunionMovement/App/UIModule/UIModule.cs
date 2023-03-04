using GameLogic.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    public class UIModule : CustommModuleInitialize
    {
        #region 实例与初始化
        //实例
        public static UIModule Instance = new UIModule();
        //是否初始化完成
        public bool IsInited { get; private set; }
        //初始化进度
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public IEnumerator Init()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("UIModule 初始化");
            }
            _initProgress = 0;
            Instance = this;

            CreateRoot();

            yield return null;

            _initProgress = 100;
            IsInited = true;
        }
        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("UIModule 清除数据");
            }
        }

        /// <summary>
        /// 正在加载的UI统计
        /// </summary>
        private int _loadingUICount = 0;

        public int LoadingUICount
        {
            get { return _loadingUICount; }
            set
            {
                _loadingUICount = value;
                if (_loadingUICount < 0)
                {
                    Debug.LogError("Error ---- LoadingUICount < 0");
                }
            }
        }
        //----------------------------------
        public Dictionary<string, UILoadState> UIWindows = new Dictionary<string, UILoadState>();

        public static Action<UIController> OnInitEvent;
        public static Action<UIController> OnOpenEvent;
        public static Action<UIController> OnCloseEvent;

        public EventSystem EventSystem;
        //----------------------------------
        //创建根路径
        public GameObject UIRoot { get; private set; }
        public Camera UICamera { get; private set; }
        public GameObject MainUIRoot { get; private set; }
        public GameObject NormalUIRoot { get; private set; }
        public GameObject HeadInfoUIRoot { get; private set; }
        public GameObject TipsUIRoot { get; private set; }

        private void CreateRoot()
        {
            UIRoot = new GameObject("UIRoot");
            MainUIRoot = new GameObject("MainUIRoot");
            NormalUIRoot = new GameObject("NormalUIRoot");
            HeadInfoUIRoot = new GameObject("HeadInfoUIRoot");
            TipsUIRoot = new GameObject("TipsUIRoot");
            MainUIRoot.transform.SetParent(UIRoot.transform, true);
            NormalUIRoot.transform.SetParent(UIRoot.transform, true);
            HeadInfoUIRoot.transform.SetParent(UIRoot.transform, true);
            TipsUIRoot.transform.SetParent(UIRoot.transform, true);

            //create camera
            UICamera = new GameObject("UICamera").AddComponent<Camera>();
            UICamera.transform.SetParent(UIRoot.transform, true);
            UICamera.cullingMask = (1 << (int)UnityLayerDef.UI);
            UICamera.clearFlags = CameraClearFlags.Depth;
            UICamera.nearClipPlane = UIDefs.Camera_Near;
            UICamera.farClipPlane = UIDefs.Camera_Far;
            UICamera.orthographic = false;
            UICamera.orthographicSize = UIDefs.Camera_Size;
            UICamera.depth = UIDefs.Camera_Depth;
            UICamera.gameObject.AddComponent<AudioListener>();

            GameObject.DontDestroyOnLoad(UIRoot);

            EventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            EventSystem.gameObject.AddComponent<StandaloneInputModule>();

            GameObject.DontDestroyOnLoad(EventSystem);
        }
        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="uiObj"></param>
        private void InitUIAsset(GameObject uiObj)
        {
            if (!uiObj)
            {
                if (Tools.IsDebug())
                {
                    Debug.LogError("uiObj is null !");
                }
                return;
            }
            var windowAsset = uiObj.GetComponent<UIWindowAsset>();
            var canvas = uiObj.GetComponent<Canvas>();
            switch (windowAsset.PanelType)
            {
                case PanelType.MainUI:
                    uiObj.transform.SetParent(MainUIRoot.transform);
                    break;
                case PanelType.NormalUI:
                    uiObj.transform.SetParent(NormalUIRoot.transform);
                    break;
                case PanelType.HeadInfoUI:
                    uiObj.transform.SetParent(HeadInfoUIRoot.transform);
                    break;
                case PanelType.TipsUI:
                    uiObj.transform.SetParent(TipsUIRoot.transform);
                    break;
                default:
                    if (Tools.IsDebug())
                    {
                        Debug.LogError(string.Format("not define PanelType", windowAsset.PanelType));
                    }
                    uiObj.transform.SetParent(UIRoot.transform);
                    break;
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = UICamera;
        }
        //----------------------------------
        //创建UI
        public UILoadState LoadWindow(string name, bool openWhenFinish, params object[] args)
        {
            GameObject uiObj = ResourcesModule.Instance.InstantiateAsset<GameObject>("Prefabs/UIs/" + name);

            if (uiObj != null)
            {
                InitUIAsset(uiObj);
                uiObj.SetActive(false);
                uiObj.name = name;
                uiObj.transform.localRotation = Quaternion.identity;
                uiObj.transform.localScale = Vector3.one;

                var uiBase = CreateUIController(uiObj, name);

                UILoadState uiLoadState = new UILoadState(name);
                uiLoadState.UIWindow = uiBase;
                uiLoadState.UIWindow.UIName = name;
                uiLoadState.IsLoading = false;
                uiLoadState.OpenWhenFinish = openWhenFinish;
                uiLoadState.OpenArgs = args;
                uiLoadState.isOnInit = true;
                InitWindow(uiLoadState, uiLoadState.UIWindow, uiLoadState.OpenWhenFinish, uiLoadState.OpenArgs);

                UIWindows.Add(name, uiLoadState);

                return uiLoadState;
            }
            return null;
        }

        //----------------------------------
        //初始化UI
        private void InitWindow(UILoadState uiState, UIController uiBase, bool open, params object[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            uiBase.OnInit();
            stopwatch.Stop();

            if (Tools.IsDebug())
            {
                Debug.Log(string.Format("OnInit UI {0}, cost {1}", uiBase.gameObject.name, stopwatch.Elapsed.TotalMilliseconds * 0.001f));
            }


            if (OnInitEvent != null)
                OnInitEvent(uiBase);

            if (open)
            {
                OnOpen(uiState, args);
            }

            if (!open)
            {
                if (!uiState.IsStaticUI)
                {
                    CloseWindow(uiBase.UIName); // Destroy
                    return;
                }
                else
                {
                    uiBase.gameObject.SetActive(false);
                }
            }

            uiState.OnUIWindowLoadedCallbacks(uiState);
        }
        //----------------------------------
        //更新UI
        [Obsolete("Use string ui name instead for more flexible!")]
        public void CallUI<T>(Action<T> callback) where T : UIController
        {
            CallUI<T>((_ui, _args) => callback(_ui));
        }

        // 使用泛型方式
        [Obsolete("Use string ui name instead for more flexible!")]
        public void CallUI<T>(Action<T, object[]> callback, params object[] args) where T : UIController
        {
            string uiName = typeof(T).Name.Remove(0, 3); // 去掉 "XUI"

            CallUI(uiName, (_uibase, _args) => { callback(_uibase as T, _args); }, args);
        }
        /// <summary>
        /// 等待并获取UI实例，执行callback
        /// 源起Loadindg UI， 在加载过程中，进度条设置方法会失效
        /// 如果是DynamicWindow,，使用前务必先要Open!
        /// </summary>
        /// <param name="uiTemplateName"></param>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void CallUI(string uiName, Action<UIController, object[]> callback, params object[] args)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, false); // 加载，这样就有UIState了, 但注意因为没参数，不要随意执行OnOpen
            }

            uiState.DoCallback(callback, args);
        }
        //----------------------------------
        //打开UI
        private void OnOpen(UILoadState uiState, params object[] args)
        {
            if (uiState.IsLoading)
            {
                uiState.OpenWhenFinish = true;
                uiState.OpenArgs = args;
                return;
            }

            UIController uiBase = uiState.UIWindow;

            if (uiBase.gameObject.activeSelf)
            {
                uiBase.OnClose();

                if (OnCloseEvent != null)
                    OnCloseEvent(uiBase);
            }

            uiBase.BeforeOpen(args, () =>
            {
                uiBase.gameObject.SetActive(true);
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                uiBase.OnOpen(args);
                stopwatch.Stop();

                if (Tools.IsDebug())
                {
                    Debug.Log(string.Format("OnOpen UI {0}, cost {1}", uiBase.gameObject.name, stopwatch.Elapsed.TotalMilliseconds * 0.001f));
                }

                if (OnOpenEvent != null)
                    OnOpenEvent(uiBase);
            });
        }
        // 打开窗口（非复制）
        public UILoadState OpenWindow(string uiName, params object[] args)
        {
            //TOD需要先创建脚本对象，再根据脚本中的值进行加载资源
            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, true, args);
                return uiState;
            }

            if (!uiState.isOnInit)
            {
                uiState.isOnInit = true;
                if (uiState.UIWindow != null) uiState.UIWindow.OnInit();
            }
            OnOpen(uiState, args);
            return uiState;
        }
        //----------------------------------
        //关闭UI
        public void CloseWindow(Type t)
        {
            CloseWindow(t.Name.Remove(0, 3)); // XUI remove
        }

        public void CloseWindow<T>()
        {
            CloseWindow(typeof(T));
        }

        public void CloseWindow(string name)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(name, out uiState))
            {
                if (Tools.IsDebug())
                {
                    Debug.Log(string.Format("[CloseWindow]没有加载的UIWindow: {0}", name));
                }
                return; // 未开始Load
            }

            if (uiState.IsLoading) // Loading中
            {
                if (Tools.IsDebug())
                {
                    Debug.Log(string.Format("[CloseWindow]IsLoading的{0}", name));
                }
                uiState.OpenWhenFinish = false;
                return;
            }

            uiState.UIWindow.gameObject.SetActive(false);

            uiState.UIWindow.OnClose();

            if (OnCloseEvent != null)
                OnCloseEvent(uiState.UIWindow);

            if (!uiState.IsStaticUI)
            {
                DestroyWindow(name);
            }
        }

        /// <summary>
        /// Destroy all windows that has LoadState.
        /// Be careful to use.
        /// </summary>
        public void DestroyAllWindows()
        {
            List<string> LoadList = new List<string>();

            foreach (KeyValuePair<string, UILoadState> uiWindow in UIWindows)
            {
                if (IsLoad(uiWindow.Key))
                {
                    LoadList.Add(uiWindow.Key);
                }
            }

            foreach (string item in LoadList)
                DestroyWindow(item, true);
        }
        //NOTE 在非生成代码情况下,xlua无法访问Obsolete的方法
        //[Obsolete("Deprecated: Please don't use this")]
        public void CloseAllWindows()
        {
            List<string> toCloses = new List<string>();

            foreach (KeyValuePair<string, UILoadState> uiWindow in UIWindows)
            {
                if (IsOpen(uiWindow.Key))
                {
                    toCloses.Add(uiWindow.Key);
                }
            }

            for (int i = toCloses.Count - 1; i >= 0; i--)
            {
                CloseWindow(toCloses[i]);
            }
        }
        //----------------------------------
        //更新UI
        // 设置窗口（非复制）
        public UILoadState SetWindow(string uiName, params object[] args)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, true, args);
                return uiState;
            }

            if (!uiState.isOnInit)
            {
                uiState.isOnInit = true;
                if (uiState.UIWindow != null) uiState.UIWindow.OnInit();
            }
            OnSet(uiState, args);
            return uiState;
        }

        private void OnSet(UILoadState uiState, params object[] args)
        {
            if (uiState.IsLoading)
            {
                uiState.OpenWhenFinish = true;
                uiState.OpenArgs = args;
                return;
            }

            UIController uiBase = uiState.UIWindow;

            if (uiBase.gameObject.activeSelf)
            {
                uiBase.BeforeOpen(args, () =>
                {
                    uiBase.gameObject.SetActive(true);
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    uiBase.OnSet(args);
                    stopwatch.Stop();
                    if (Tools.IsDebug())
                    {
                        Debug.Log(string.Format("OnOpen UI {0}, cost {1}", uiBase.gameObject.name, stopwatch.Elapsed.TotalMilliseconds * 0.001f));
                    }
                    if (OnOpenEvent != null)
                        OnOpenEvent(uiBase);
                });
            }

        }
        //----------------------------------
        //删除UI  
        public void DestroyWindow(string uiName, bool destroyImmediate = false)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(uiName, out uiState);
            if (uiState == null || uiState.UIWindow == null)
            {
                Debug.Log(string.Format("{0} has been destroyed", uiName));
                return;
            }
            if (destroyImmediate)
            {
                UnityEngine.Object.DestroyImmediate(uiState.UIWindow.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(uiState.UIWindow.gameObject);
            }

            uiState.UIWindow = null;

            UIWindows.Remove(uiName);
        }
        //----------------------------------
        //        
        //----------------------------------
        //        
        //----------------------------------
        // 隐藏时打开，打开时隐藏
        public void ToggleWindow<T>(params object[] args)
        {
            string uiName = typeof(T).Name.Remove(0, 3); // 去掉"CUI"
            ToggleWindow(uiName, args);
        }

        public void ToggleWindow(string name, params object[] args)
        {
            if (IsOpen(name))
            {
                CloseWindow(name);
            }
            else
            {
                OpenWindow(name, args);
            }
        }
        //----------------------------------
        //tool
        public bool IsLoad(string name)
        {
            if (UIWindows.ContainsKey(name))
                return true;
            return false;
        }

        public bool IsOpen(string name)
        {
            UIController uiBase = GetUIBase(name);
            return uiBase == null ? false : uiBase.gameObject.activeSelf;
        }

        private UIController GetUIBase(string name)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(name, out uiState);
            if (uiState != null && uiState.UIWindow != null)
                return uiState.UIWindow;

            return null;
        }


        public virtual UIController CreateUIController(GameObject uiObj, string uiTemplateName)
        {
            UIController uiBase = uiObj.AddComponent(System.Type.GetType("GameLogic.UI." + uiTemplateName + ", Assembly-CSharp")) as UIController;
            //UIController uiBase = uiObj.AddComponent<UIController>();
            return uiBase;
        }
    }

    public class UILoadState
    {
        public string UIName;
        public UIController UIWindow;
        public Type UIType;
        public bool IsLoading;
        //非复制出来的, 静态UI
        public bool IsStaticUI;
        //是否初始化
        public bool isOnInit = false;
        //完成后是否打开
        public bool OpenWhenFinish;
        public object[] OpenArgs;
        //回调
        internal Queue<Action<UIController, object[]>> CallbacksWhenFinish;
        internal Queue<object[]> CallbacksArgsWhenFinish;

        public UILoadState(string uiName, Type uiControllerType = default(Type))
        {
            if (uiControllerType == default(Type)) uiControllerType = typeof(UIController);

            UIName = uiName;
            UIWindow = null;
            UIType = uiControllerType;

            IsLoading = true;
            OpenWhenFinish = false;
            OpenArgs = null;

            CallbacksWhenFinish = new Queue<Action<UIController, object[]>>();
            CallbacksArgsWhenFinish = new Queue<object[]>();
        }

        /// <summary>
        /// 确保加载完成后的回调
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void DoCallback(Action<UIController, object[]> callback, object[] args = null)
        {
            if (args == null)
                args = new object[0];

            if (IsLoading) // Loading
            {
                CallbacksWhenFinish.Enqueue(callback);
                CallbacksArgsWhenFinish.Enqueue(args);
                return;
            }

            // 立即执行即可
            callback(UIWindow, args);
        }

        internal void OnUIWindowLoadedCallbacks(UILoadState uiState)
        {
            //if (openState.OpenWhenFinish)  // 加载完打开 模式下，打开时执行回调
            {
                while (uiState.CallbacksWhenFinish.Count > 0)
                {
                    Action<UIController, object[]> callback = uiState.CallbacksWhenFinish.Dequeue();
                    object[] _args = uiState.CallbacksArgsWhenFinish.Dequeue();
                    DoCallback(callback, _args);
                }
            }
        }
    }

    /// <summary>
    /// UI默认值
    /// </summary>
    public static class UIDefs
    {
        public static int Camera_Size = 1;
        public static int Camera_Near = 1;
        public static int Camera_Far = 1000;
        public static int Camera_Depth = 1;

    }
}