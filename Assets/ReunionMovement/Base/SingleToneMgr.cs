using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Base
{
    public class SingleToneMgr<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
            set
            {
                if (instance == null)
                {
                    instance = value;
                }
                else if (instance != value)
                {
                    Destroy(value.gameObject);
                }
            }
        }

        void Awake()
        {
            Instance = this as T;
        }
    }
}