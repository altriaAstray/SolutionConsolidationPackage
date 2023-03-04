using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 语言模块
    /// </summary>
    public class LanguagesModule : CustommModuleInitialize
    {
        //------------------------------
        //实例
        public static LanguagesModule Instance = new LanguagesModule();
        //------------------------------
        LanguageSubject subject = new LanguageSubject();
        Multilingual multilingual;
        // 语言配置表
        Dictionary<int, Languages> languages = new Dictionary<int, Languages>();
        //------------------------------
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }

        string filePath = AppConfig.DatabasePath;
        string fileName = "Languages.json";
        //------------------------------
        public IEnumerator Init()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("LanguagesModule 初始化");
            }

            List<Languages> languagesConfigs = new List<Languages>();
            //获取完整路径
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + fileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, fileName);
                languagesConfigs = JsonMapper.ToObject<List<Languages>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.LoadAsset<TextAsset>("AutoDatabase/Languages");
                PathUtils.WriteFile(json.text, filePath, fileName);
                languagesConfigs = JsonMapper.ToObject<List<Languages>>(json.text);
            }

            foreach (Languages tempData in languagesConfigs)
            {
                languages.Add(tempData.Id, tempData);
            }

            if (DatabaseModule.Instance != null)
            {
                SetMultilingual(int.Parse(DatabaseModule.Instance.GetConfig()[100004].Value));
            }

            //if (DataBaseModule.Instance != null)
            //{
            //    List<Languages> tempConfigs = DataBaseModule.Instance.GetLanguages();
            //    foreach (Languages tempData in tempConfigs)
            //    {
            //        languages.Add(tempData.Index, tempData);
            //        //Debug.Log(tempData.Index + "|" + tempData.EN + "|" + tempData.ZH);
            //    }
            //    SetMultilingual(int.Parse(DataBaseModule.Instance.GetConfig()[100004].Value));
            //}
            yield return null;
            IsInited = true;
        }

        public void ClearData()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("LanguagesModule 清除数据");
            }
            //languages.Clear();
        }

        /// <summary>
        /// 获取多语言类型
        /// </summary>
        /// <returns></returns>
        public Multilingual GetMultilingual()
        {
            return multilingual;
        }

        /// <summary>
        /// 选择语言
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isInit"></param>
        public void SetMultilingual(int value)
        {
            switch (value)
            {
                case 0:
                    multilingual = Multilingual.ZH;
                    break;
                case 1:
                    multilingual = Multilingual.EN;
                    break;
            }

            subject.SetState();
        }

        /// <summary>
        /// 根据语言和索引获得文本内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetTextByIndex(int index)
        {
            string text = "";

            if (languages.ContainsKey(index))
            {
                switch (multilingual)
                {
                    case Multilingual.ZH:
                        text = languages[index].ZH;
                        break;

                    case Multilingual.EN:
                        text = languages[index].EN;
                        break;
                }
            }
            return text;
        }

        /// <summary>
        /// 获得被观察者
        /// </summary>
        /// <returns></returns>
        public LanguageSubject GetLanguageSubject()
        {
            return subject;
        }
    }
}