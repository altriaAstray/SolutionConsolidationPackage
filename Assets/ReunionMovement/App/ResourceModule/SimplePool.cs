using GameLogic.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 简单对象池
    /// </summary>

    public class SimplePool : SingleToneMgr<SimplePool>
    {
        //对象池
        public List<ObjectsPool> ObjectsList = new List<ObjectsPool>();

        Transform thisTransform;
        int[] numberObjects;
        GameObject[][] stObjects;
        public int numObjectsList;

        public void Awake()
        {
            Instance = this;
            Init();
        }

        public void Start()
        {
            
        }

        void Init()
        {
            thisTransform = transform;
            numObjectsList = ObjectsList.Count;
            AddObjectsToPool();
        }

        //将对象添加到池
        void AddObjectsToPool()
        {
            numberObjects = new int[numObjectsList];
            stObjects = new GameObject[numObjectsList][];

            for (int num = 0; num < numObjectsList; num++)
            {
                numberObjects[num] = ObjectsList[num].numberObjects;
                stObjects[num] = new GameObject[numberObjects[num]];
                InstanInPool(ObjectsList[num].obj, stObjects[num]);
            }
        }

        /// <summary>
        /// 池中的实例
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objs"></param>
        void InstanInPool(GameObject obj, GameObject[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = Instantiate(obj);
                objs[i].SetActive(false);
                objs[i].transform.parent = thisTransform;
            }
        }

        /// <summary>
        /// 给出对象(用于提供对象)
        /// </summary>
        /// <param name="numElement">列表索引</param>
        /// <returns></returns>
        public GameObject GiveObj(int numElement)
        {
            for (int i = 0; i < numberObjects[numElement]; i++)
            {
                if (!stObjects[numElement][i].activeSelf)
                    return stObjects[numElement][i];
            }

            if(Tools.IsDebug())
                Debug.Log("池中的对象已结束!");
            return null;
        }

        //接收对象
        public void Takeobj(GameObject obj)
        {
            if (obj.activeSelf) obj.SetActive(false);
            if (obj.transform.parent != thisTransform) obj.transform.parent = thisTransform;
        }

        //--------------------------------
        //生成声音对象
        public void ObjectProcessing(AudioClip clip, Transform emitter, float volume, bool loop)
        {
            StartCoroutine(ObjectProcessing2(clip, emitter, volume, loop));
        }
        IEnumerator ObjectProcessing2(AudioClip clip, Transform emitter, float volume, bool loop)
        {
            GameObject go = SimplePool.Instance.GiveObj(0);
            if (go != null)
            {
                if (emitter != null)
                {
                    go.transform.parent = emitter;
                }
                else
                {
                    go.transform.parent = transform;
                }

                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);

                AudioSource t_source = go.GetComponent<AudioSource>();
                t_source.clip = clip;
                t_source.volume = volume;//AudioSize / 100f;
                t_source.loop = loop;
                t_source.Play();

                yield return new WaitForSeconds(clip.length);

                Takeobj(go);
            }
        }
        //--------------------------------
    }

    [System.Serializable]

    public class ObjectsPool
    {
        public GameObject obj;
        public int numberObjects = 100;
    }
}
