using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    /// <summary>
    /// 摄像机围绕物体旋转
    /// </summary>
    public class CameraRotatesAroundObject : MonoBehaviour
    {
        //目标对象
        public Transform targetPos;
        public GameObject root;
        //移动摄像机
        protected const string MOUSEX = "Mouse X";
        protected const string MOUSEY = "Mouse Y";

        //初始角度
        public float m_rotX = 0; // Camera's current rotation around the X axis (up/down)
        public float m_rotY = 0; // Camera's current rotation around the Y axis (left/right)
        
        Quaternion m_destRot = Quaternion.identity;
        public float OffsetHeight = 0f;
        public float LateralOffset = 0f;
        public float MaxLateralOffset = 5f;
        public float OffsetDistance = 30f;
        public float MaxDistance = 30f;
        public float MinDistance = 10f;
        public float ZoomSpeed = 10f;
        public float ZoomValue = 50f;
        public float ResetSpeed = 5f;
        public float RotateSpeed = 15f;
        public float MaxRotX = 90f;
        public float MinRotX = -90f;
        public float m_defaualtDistance = 30f;
        private Vector3 m_relativePosition = Vector3.zero;

        public bool isEnable;

        public void Start()
        {
            OrbitCamera(0, -0);
            UpdatePosition();
            isEnable = true;
        }

        public void SetIsEnableTrue()
        {
            isEnable = true;
            root.SetActive(false);
        }

        public void Update()
        {
            if(isEnable == false)
            {
                if (Input.GetMouseButton(1))
                {
                    float valueX = Input.GetAxis("Mouse X") * 10;
                    root.transform.Rotate(Vector3.down, valueX);
                }
            }
            else
            {
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WebGLPlayer)
                {
#if UNITY_EDITOR
                    if (EventSystem.current.IsPointerOverGameObject())
#elif UNITY_ANDROID || UNITY_IPHONE
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
                    if (EventSystem.current.IsPointerOverGameObject())
#endif
                    {
                        return;
                    }

                    //左键点击发送射线
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 pos = Input.mousePosition;
                        Ray ray = Camera.main.ScreenPointToRay(pos);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 2000))
                        {
                            if (hit.collider.gameObject != null)
                            {

                            }
                        }
                    }

                    //鼠标左键拖拽
                    if (Input.GetMouseButton(0))
                    {
                        float horz = Input.GetAxis(MOUSEX);
                        float vert = Input.GetAxis(MOUSEY);

                        OrbitCamera(horz, -vert);
                    }

                    //滚轮
                    SetZoom();
                }

                if (Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    //没有触摸
                    if (Input.touchCount <= 0)
                    {
                        return;
                    }

                    //触摸为1 开始触摸
                    if (1 == Input.touchCount && Input.GetTouch(0).phase == TouchPhase.Began)
                    {

                    }
                    //触摸为1 滑动
                    else if (1 == Input.touchCount)
                    {
#if UNITY_EDITOR
                        if (EventSystem.current.IsPointerOverGameObject())
#elif UNITY_ANDROID || UNITY_IPHONE
                        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
                        if (EventSystem.current.IsPointerOverGameObject())
#endif
                        {
                            return;
                        }

                        Touch touch = Input.GetTouch(0);

                        float horz = touch.deltaPosition.x;
                        float vert = touch.deltaPosition.y;

                        OrbitCamera(horz, -vert);
                    }
                }
            }
        }

        /// <summary>
        /// 旋转摄像机
        /// </summary>
        /// <param name="horz">水平调整</param>
        /// <param name="vert">垂直调整</param>
        private void OrbitCamera(float horz, float vert)
        {
            float step = Time.deltaTime * RotateSpeed;
            m_rotX += horz * step;
            m_rotY += vert * step;
            m_rotY = Mathf.Clamp(m_rotY, MinRotX, MaxRotX);
            Quaternion addRot = Quaternion.Euler(0f, m_rotX, 0f);
            m_destRot = addRot * Quaternion.Euler(m_rotY, 0f, 0f);
            transform.rotation = m_destRot;
            UpdatePosition();
        }

        /// <summary>
        /// 更新摄像机位置
        /// </summary>
        public void UpdatePosition()
        {
            OffsetDistance = Mathf.MoveTowards(OffsetDistance, m_defaualtDistance, Time.deltaTime * ZoomSpeed);
            m_relativePosition = (targetPos.position + (Vector3.up * OffsetHeight)) + (transform.rotation * (Vector3.forward * -OffsetDistance)) + (transform.right * LateralOffset);
            transform.position = m_relativePosition;
        }

        /// <summary>
        /// 滚轮
        /// </summary>
        public void SetZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                m_defaualtDistance -= ZoomValue;
                if (m_defaualtDistance < MinDistance)
                {
                    m_defaualtDistance = MinDistance;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                m_defaualtDistance += ZoomValue;
                if (m_defaualtDistance > MaxDistance)
                {
                    m_defaualtDistance = MaxDistance;
                }
            }

            UpdatePosition();
        }
    }
}