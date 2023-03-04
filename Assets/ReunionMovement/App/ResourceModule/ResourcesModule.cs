using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLogic
{
    /// <summary>
    /// 资源模块
    /// </summary>
    public class ResourcesModule : CustommModuleInitialize
    {
        //------------------------------
        //实例
        public static ResourcesModule Instance = new ResourcesModule();
        //------------------------------
        private string _streamPath;
        //缓存从Resource中加载的资源
        private Hashtable _resourceTable; 
        //------------------------------
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        //------------------------------
        public IEnumerator Init()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("ResourcesModule 初始化");
            }

            _resourceTable = new Hashtable();
            _streamPath = Application.streamingAssetsPath;

            yield return null;
            IsInited = true;
        }

        public void ClearData()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("ResourcesModule 清除数据");
            }
        }

        /// <summary>
        /// 加载Resources下资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public T LoadAsset<T>(string path, bool isCache = true) where T : Object
        {
            if (_resourceTable.Contains(path))
            {
                return _resourceTable[path] as T;
            }
            var assets = Resources.Load<T>(path);
            if (assets == null)
            {
                Debug.LogErrorFormat("资源没有找到路径为:{0}", path);
                return null;
            }
            if (isCache)
            {
                _resourceTable.Add(path, assets);
            }
            return assets;
        }

        /// <summary>
        /// 加载Stream下资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public WWW LoadAssetAsStream(string name, bool isCache = true)
        {
            if (_resourceTable.Contains(name))
                return _resourceTable[name] as WWW;
            var www = new WWW("file:///" + Path.Combine(_streamPath, name));
            Debug.Log(Path.Combine(_streamPath, name));
            while (!www.isDone) { }
            return www;
        }

        /// <summary>
        /// 实例化资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T InstantiateAsset<T>(string path) where T : Object
        {
            var obj = LoadAsset<T>(path);
            var go = GameObject.Instantiate<T>(obj);
            if (go == null)
                Debug.LogError("Instantiate {0} failed!", obj);
            return go;
        }

        /// <summary>
        /// 移除单个数据缓存
        /// </summary>
        /// <param name="path"></param>
        public void DeleteAssetCache(string path)
        {
            if(_resourceTable.ContainsKey(path))
            {
                _resourceTable.Remove(path);
            }            
        }

        /// <summary>
        /// 清除资源缓存
        /// </summary>
        public void ClearAssetsCache()
        {
            foreach (Object asset in _resourceTable)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(asset, true);
#else
                GameObject.DestroyObject(asset);
#endif
            }
            _resourceTable.Clear();
        }
    }
}