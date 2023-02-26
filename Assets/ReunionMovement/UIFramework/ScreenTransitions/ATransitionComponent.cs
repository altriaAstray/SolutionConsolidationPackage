using UnityEngine;
using System;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 屏幕使用ATransitionComponent为其输入和输出转换设置动画。这可以扩展到使用Lerps、动画等。
    /// </summary>
    public abstract class ATransitionComponent : MonoBehaviour
    {
        /// <summary>
        /// 设置指定目标变换的动画，并在动画完成后执行CallWhenFinished。
        /// </summary>
        /// <param name="target">Target transform.</param>
        /// <param name="callWhenFinished">Delegate to be called when animation is finished.</param>
        public abstract void Animate(Transform target, Action callWhenFinished);
    }
}
