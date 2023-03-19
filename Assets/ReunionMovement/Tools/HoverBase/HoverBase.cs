using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameLogic
{
	/// <summary>
    /// 悬停
    /// </summary>
    public class HoverBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
		public bool isHover;

		public delegate void HoverHandler(bool hover);
		public event HoverHandler GetIsHover;

		private void Awake()
		{
			isHover = false;
		}

		/// <summary>
		/// 进入悬停
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			isHover = true;
            GetIsHover(isHover);
		}
		/// <summary>
		/// 离开悬停
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerExit(PointerEventData eventData)
		{
			isHover = false;
			GetIsHover(isHover);
		}
    }
}