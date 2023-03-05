using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class SceneModule : CustommModuleInitialize
    {
        #region 实例与初始化
        //实例
        public static SceneModule Instance = new SceneModule();
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
                Debug.Log("SceneModule 初始化");
            }
            _initProgress = 0;
            Instance = this;

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
                Debug.Log("SceneModule 清除数据");
            }
        }
    }
}