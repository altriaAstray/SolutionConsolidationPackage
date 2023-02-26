using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Events
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public static class EventMgr
    {
        /// <summary>
        /// 事件监听池
        /// </summary>
        private static Dictionary<EventType, DelegateEvent> eventTypeListeners = new Dictionary<EventType, DelegateEvent>();

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="listenerFunc">监听函数</param>
        public static void addEventListener(EventType type, DelegateEvent.EventHandler listenerFunc)
        {
            DelegateEvent delegateEvent;
            if (eventTypeListeners.ContainsKey(type))
            {
                delegateEvent = eventTypeListeners[type];
            }
            else
            {
                delegateEvent = new DelegateEvent();
                eventTypeListeners[type] = delegateEvent;
            }
            delegateEvent.addListener(listenerFunc);
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="listenerFunc">监听函数</param>
        public static void removeEventListener(EventType type, DelegateEvent.EventHandler listenerFunc)
        {
            if (listenerFunc == null)
            {
                return;
            }
            if (!eventTypeListeners.ContainsKey(type))
            {
                return;
            }
            DelegateEvent delegateEvent = eventTypeListeners[type];
            delegateEvent.removeListener(listenerFunc);
        }

        /// <summary>
        /// 触发某一类型的事件  并传递数据
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="data">事件的数据(可为null)</param>
        public static void dispatchEvent(EventType type, object data)
        {
            if (!eventTypeListeners.ContainsKey(type))
            {
                return;
            }
            //创建事件数据
            EventData eventData = new EventData();
            eventData.type = type;
            eventData.data = data;

            DelegateEvent delegateEvent = eventTypeListeners[type];
            delegateEvent.Handle(eventData);
        }
    }

}

