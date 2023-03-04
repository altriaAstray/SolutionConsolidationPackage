using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 界面类型
    /// </summary>
    public enum PanelType
    {
        //主界面类型，比如：摇杆，任务，聊天，活动/穿戴提示，主界面入口图标等界面
        MainUI,
        //最常用的类型，绝大多数界面都属于此层级
        NormalUI,
        //仅仅给头顶文字使用
        HeadInfoUI,
        //Tips层级，在最顶层显示，比如：系统飘字提示，系统公告/广播，停服维护等
        TipsUI,
    }

    public enum UnityLayerDef
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,

        Water = 4,
        UI = 5,

        //以下为自定义
        //Hidden = 8,
    }
}