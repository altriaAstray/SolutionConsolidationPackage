using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class BillboardFX : MonoBehaviour
    {
        public Transform camTransform;
        public BillboardFXType billboardFXType = BillboardFXType.Mode1;

        public enum BillboardFXType
        {
            Mode1, //和摄像机保持一个方向和角度
            Mode2, //Z轴朝向摄像机，但角度一直为0
            Mode3, //Z轴朝向摄像机
        }
        
        Quaternion originalRotation;

        void Start()
        {
            originalRotation = transform.rotation;
        }

        void Update()
        {
            switch(billboardFXType)
            {
                case BillboardFXType.Mode1:
                    transform.rotation = camTransform.rotation * originalRotation;
                    break; 
                case BillboardFXType.Mode2:
                    Vector3 v = camTransform.position - transform.position;
                    v.x = v.z = 0.0f;
                    transform.LookAt(camTransform.position - v);
                    break;
                case BillboardFXType.Mode3:
                    transform.LookAt(camTransform.transform.position);
                    break;
            }
        }
    }
}