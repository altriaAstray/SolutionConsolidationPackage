using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 功能：音频管理
/// 创建者：长生
/// 日期：2021年11月23日11:22:09
/// </summary>

namespace GameLogic
{

    public class AudioModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static AudioModule Instance = new AudioModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        string filePath = AppConfig.DatabasePath;
        string fileName = "AudioConfig.json";
        //声音对象
        AudioSource sourceObj;
        Transform transformObj;

        //背景音乐
        bool playMusic = false;
        float musicVolume = 0.5f;
        Dictionary<int, AudioConfig> musics = new Dictionary<int, AudioConfig>();   //全部bgm
        List<int> randomBgm = new List<int>();                                      //bgm随机播放
        //特效音
        bool playSoundEffect = false;
        float soundEffectVolume = 0.5f;
        Dictionary<int, AudioConfig> sounds = new Dictionary<int, AudioConfig>();   //全部特效音

        public IEnumerator Init()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("AudioModule 初始化");
            }

            List<AudioConfig> audioConfigs = new List<AudioConfig>();
            //获取完整路径
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + fileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, fileName);
                audioConfigs = JsonMapper.ToObject<List<AudioConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.LoadAsset<TextAsset>("AutoDatabase/AudioConfig");
                PathUtils.WriteFile(json.text, filePath, fileName);
                audioConfigs = JsonMapper.ToObject<List<AudioConfig>>(json.text);
            }
            
            foreach (AudioConfig tempData in audioConfigs)
            {
                if (tempData.Type == 1 || tempData.Type == 2)
                {
                    musics.Add(tempData.Id, tempData);
                }
                else if (tempData.Type == 3)
                {
                    sounds.Add(tempData.Id, tempData);
                }
            }

            transformObj = AppEngine.Instance.transform;
            sourceObj = transformObj.GetComponent<AudioSource>();
            if (sourceObj == null)
            {
                sourceObj = AppEngine.Instance.gameObject.AddComponent<AudioSource>();
            }

            ReadConfig();

            yield return null;
            IsInited = true;
        }

        public void ClearData()
        {
            if (Tools.IsDebug())
            {
                Debug.Log("AudioModule 清除数据");
            }
        }
        //------------------------------

        /// <summary>
        /// 获取全部音乐键
        /// </summary>
        /// <returns>全部音乐键</returns>
        public List<int> GetMusicKey()
        {
            return musics.Keys.ToList();
        }

        ///// <summary>
        ///// 根据类型获取全部音乐键
        ///// </summary>
        ///// <param name="type">类型</param>
        ///// <returns>全部音乐键</returns>
        //public List<int> GetMusicKeyByType(int type)
        //{
        //    var query = from r in musics where r.Value.Type == type select r;
        //    List<int> list = new List<int>();

        //    foreach (KeyValuePair<int ,AudioConfig> kv in query)
        //    {
        //        list.Add(kv.Key); 
        //    }
        //    return list;
        //}

        /// <summary>
        /// 获取全部音效键
        /// </summary>
        /// <returns>全部音效键</returns>
        public List<int> GetSoundKey()
        {
            return sounds.Keys.ToList();
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        public void ReadConfig()
        {
            if (DatabaseModule.Instance.GetConfig()[100005].Value == "TRUE")
            {
                playMusic = true;
            }
            else
            {
                playMusic = false;
            }
            musicVolume = float.Parse(DatabaseModule.Instance.GetConfig()[100006].Value);


            if (DatabaseModule.Instance.GetConfig()[100007].Value == "TRUE")
            {
                playSoundEffect = true;
            }
            else
            {
                playSoundEffect = false;
            }
            soundEffectVolume = float.Parse(DatabaseModule.Instance.GetConfig()[100008].Value);
        }

        /// <summary>
        /// 是否关闭BGM
        /// </summary>
        /// <returns>是否</returns>
        public bool IsPlayingMusic()
        {
            return playMusic;
        }
        /// <summary>
        /// 是否关闭特效音
        /// </summary>
        /// <returns>是否</returns>
        public bool IsPlayingSoundEffect()
        {
            return playSoundEffect;
        }
        /// <summary>
        /// 播放BGM声音大小
        /// </summary>
        /// <returns>值</returns>
        public float MusicVolume()
        {
            return musicVolume;
        }
        /// <summary>
        /// 播放特效音声音大小
        /// </summary>
        /// <returns>值</returns>
        public float SoundEffectVolume()
        {
            return soundEffectVolume;
        }
        /// <summary>
        /// 设置背景音是否关闭
        /// </summary>
        /// <param name="value"></param>
        public void SetPlayMusic(bool value)
        {
            playMusic = value;

            if (playMusic == true)
            {
                sourceObj.Pause();
            }
            else
            {
                sourceObj.Play();
            }

            if (playMusic == true)
            {
                DatabaseModule.Instance.GetConfig()[100005].Value = "TRUE";
            }
            else
            {
                DatabaseModule.Instance.GetConfig()[100005].Value = "FALSE";
            }
            DatabaseModule.Instance.SaveConfig();
        }
        /// <summary>
        /// 设置背景音大小
        /// </summary>
        /// <param name="value"></param>
        public void SetMusicVolume(float value)
        {
            musicVolume = Mathf.Clamp(value, 0, 1);
            float tempValue = (float)Math.Round(musicVolume, 2);
            DatabaseModule.Instance.GetConfig()[100006].Value = tempValue.ToString();
            DatabaseModule.Instance.SaveConfig();
            sourceObj.volume = musicVolume;
        }
        /// <summary>
        /// 设置特效音是否关闭
        /// </summary>
        /// <param name="value"></param>
        public void SetPlaySoundEffect(bool value)
        {
            playSoundEffect = value;

            if (playSoundEffect == true)
            {
                DatabaseModule.Instance.GetConfig()[100007].Value = "TRUE";
            }
            else
            {
                DatabaseModule.Instance.GetConfig()[100007].Value = "FALSE";
            }

            DatabaseModule.Instance.SaveConfig();
        }
        /// <summary>
        /// 设置特效音大小
        /// </summary>
        /// <param name="value"></param>
        public void SetSoundEffectVolume(float value)
        {
            soundEffectVolume = Mathf.Clamp(value, 0, 1);
            float tempValue = (float)Math.Round(soundEffectVolume, 2);
            DatabaseModule.Instance.GetConfig()[100008].Value = tempValue.ToString();
            DatabaseModule.Instance.SaveConfig();
        }


        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="p_fileName"></param>
        public void PlayBGM(int index)
        {
            if (sourceObj.clip == null || !sourceObj.isPlaying)
            {
                if (musics != null && musics.ContainsKey(index))
                {
                    AudioClip audioClip = ResourcesModule.Instance.LoadAsset<AudioClip>(musics[index].Path + musics[index].Name);
                    sourceObj.clip = audioClip;
                    sourceObj.volume = musicVolume;
                    sourceObj.loop = true;
                    if (!playMusic)
                        sourceObj.Play();
                }
            }
        }

        /// <summary>
        /// 随机播放
        /// </summary>
        /// <param name="p_fileName"></param>
        public void RandomPlayBGM(List<int> Ids)
        {
            randomBgm = Ids;

            if (sourceObj.clip == null || !sourceObj.isPlaying)
            {
                int index = UnityEngine.Random.Range(0, 5);
                if (musics != null && musics.ContainsKey(randomBgm[index]))
                {
                    AudioClip audioClip = ResourcesModule.Instance.LoadAsset<AudioClip>(musics[randomBgm[index]].Path + musics[randomBgm[index]].Name);
                    sourceObj.clip = audioClip;
                    sourceObj.volume = musicVolume;
                    sourceObj.loop = false;
                    if (!playMusic)
                        sourceObj.Play();
                }
            }
        }

        public void Update()
        {
            if(randomBgm != null && randomBgm.Count > 0 && !playMusic)
            {
                if(!sourceObj.isPlaying)
                {
                    int index = UnityEngine.Random.Range(0, randomBgm.Count);
                    if (musics != null && musics.ContainsKey(randomBgm[index]))
                    {
                        AudioClip audioClip = ResourcesModule.Instance.LoadAsset<AudioClip>(musics[randomBgm[index]].Path + musics[randomBgm[index]].Name);
                        sourceObj.clip = audioClip;
                        sourceObj.volume = musicVolume;
                        sourceObj.loop = false;
                        sourceObj.Play();
                    }
                }
            }
        }


        /// <summary>
        /// 结束背景音乐
        /// </summary>
        public void StopBGM()
        {
            if(randomBgm != null)
            {
                randomBgm.Clear();
            }

            if (sourceObj != null)
            {
                sourceObj.Stop();
            }
        }

        //特效音 - 播放与停止
        public void PlaySound(int index)
        {
            PlaySound(index, null, /*0.8f,*/ false);
        }
        //特效音 - 播放与停止
        public void PlaySound(int index, Transform pos)
        {
            PlaySound(index, pos,/* 0.8f,*/ false);
        }
        //特效音 - 播放与停止
        public void PlaySound(int index, Transform emitter, bool loop)
        {
            if (playSoundEffect == false && soundEffectVolume > 0)
            {
                if (sounds != null && sounds.ContainsKey(index))
                {
                    AudioClip clip = ResourcesModule.Instance.LoadAsset<AudioClip>(sounds[index].Path + sounds[index].Name);
                    if (clip != null)
                    {
                        SimplePool.Instance.ObjectProcessing(clip, emitter, soundEffectVolume, loop);
                    }
                }
            }
        }
    }
}