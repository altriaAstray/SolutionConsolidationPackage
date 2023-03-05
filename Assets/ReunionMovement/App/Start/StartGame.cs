using GameLogic.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace GameLogic
{
    public class StartGame : AppGame
    {
        protected override IList<CustommModuleInitialize> CreateModules()
        {
            var modules = base.CreateModules();

            modules.Add(ResourcesModule.Instance);
            modules.Add(DatabaseModule.Instance);
            modules.Add(EventModule.Instance);
            modules.Add(SceneModule.Instance);
            modules.Add(LanguagesModule.Instance);
            modules.Add(AudioModule.Instance);
            modules.Add(UIModule.Instance);
            return modules;
        }

        /// <summary>
        /// 在初始化模块之前，协同程序
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnBeforeInit()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("StartGame初始化前");
            }
            yield return null;
        }

        /// <summary>
        /// 在初始化模块之后，协同路由
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnGameStart()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("StartGame初始化后");
            }
            yield return null;

            StartCoroutine(StartGameShow());
        }

        public void Play()
        {
            
        }

        public IEnumerator StartGameShow()
        {
            UIModule.Instance.OpenWindow("StartGameUIPlane");

            yield return new WaitForSeconds(0f);
        }

        /// <summary>
        /// 退出处理
        /// </summary>
        void OnApplicationQuit()
        {

        }
        /// <summary>
        /// 焦点处理
        /// </summary>
        /// <param name="focus"></param>
        void OnApplicationFocus(bool focus)
        {

        }
    }
}

