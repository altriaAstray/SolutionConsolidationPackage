using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// UI控制器
    /// </summary>
    public class UIController : MonoBehaviour
    {
        /// <summary>
        /// Set from KUIModule, InstanceName
        /// </summary>
        public string UIName = "";

        public virtual void OnInit()
        {

        }

        public virtual void BeforeOpen(object[] onOpenArgs, Action doOpen)
        {
            doOpen();
        }

        public virtual void OnOpen(params object[] args)
        {
        }
        public virtual void OnSet(params object[] args)
        {
        }
        public virtual void OnClose()
        {
        }

        /// <summary>
        /// 输入uri搜寻控件
        /// findTrans默认参数null时使用this.transform
        /// </summary>
        public T GetControl<T>(string uri, Transform findTrans = null, bool isLog = true) where T : UnityEngine.Object
        {
            return (T)GetControl(typeof(T), uri, findTrans, isLog);
        }

        public object GetControl(Type type, string uri, Transform findTrans = null, bool isLog = true)
        {
            if (findTrans == null)
                findTrans = transform;

            Transform trans = findTrans.Find(uri);
            if (trans == null)
            {
                if (isLog)
                    Debug.LogError(string.Format("Get UI<{0}> Control Error: " + uri, this));
                return null;
            }

            if (type == typeof(GameObject))
                return trans.gameObject;

            return trans.GetComponent(type);
        }

        /// <summary>
        /// 默认在当前transfrom下根据Name查找子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T FindControl<T>(string name) where T : Component
        {
            GameObject obj = DFSFindObject(transform, name);
            if (obj == null)
            {
                Debug.LogError("Find UI Control Error: " + name);
                return null;
            }

            return obj.GetComponent<T>();
        }

        public GameObject FindGameObject(string name)
        {
            GameObject obj = DFSFindObject(transform, name);
            if (obj == null)
            {
                Debug.LogError("Find GemeObject Error: " + name);
                return null;
            }

            return obj;
        }

        /// <summary>
        /// 从parent下根据Name查找
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject DFSFindObject(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform node = parent.GetChild(i);
                if (node.name == name)
                    return node.gameObject;

                GameObject target = DFSFindObject(node, name);
                if (target != null)
                    return target;
            }

            return null;
        }

        /// <summary>
        /// 清除一个GameObject下面所有的孩子
        /// </summary>
        /// <param name="go"></param>
        public void DestroyGameObjectChildren(GameObject go)
        {
            Tools.ClearChild(go);
        }

        /// <summary>
        /// Shortcuts for UIModule's Open Window
        /// </summary>
        protected void OpenWindow(string uiName, params object[] args)
        {
            UIModule.Instance.OpenWindow(uiName, args);
        }

        /// <summary>
        /// Shortcuts for UIModule's Close Window
        /// </summary>
        /// <param name="uiName"></param>
        protected void CloseWindow(string uiName = null)
        {
            UIModule.Instance.CloseWindow(uiName ?? UIName);
        }

        /// <summary>
        /// 从数组获取参数，并且不报错，返回null, 一般用于OnOpen, OnClose的可变参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openArgs"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected T GetFromArgs<T>(object[] openArgs, int offset, bool isLog = true)
        {
            return openArgs.Get<T>(offset, isLog);
        }

        [Obsolete("Use CallUI(stringName) insted!")]
        public static void CallUI<T>(Action<T> callback) where T : UIController
        {
            UIModule.Instance.CallUI<T>(callback);
        }
    }
}
