using System;
using System.Collections;
using UnityEngine;

namespace GameLogic.UIFramework.Examples
{
    /// <summary>
    /// 多年来，我一直避免使用Legacy Animation系统，但由于我知道人们希望在其UI上使用手写动画，
    /// 因此出于工作流程和性能原因，
    /// 我强烈建议不要使用Animator（参考：https://www.youtube.com/watch?v=_wxitgdx-UI&t=2883s），
    /// 我决定使用Legacy系统添加此示例。您可以研究的另一种选择是SimpleAnimationComponent
    /// （参考：https://blogs.unity3d.com/2017/11/28/introducing-the-simple-animation-component/）尽管它仍然运行在Animator之上，
    /// 但至少它可能有一个更简单的工作流。
    ///警告：这似乎有效，但几乎没有经过测试。生产时要小心：D
    /// </summary>
    public class LegacyAnimationScreenTransition : ATransitionComponent
    {
        [SerializeField] private AnimationClip clip = null;
        [SerializeField] private bool playReverse = false;

        private Action previousCallbackWhenFinished;
        /// <summary>
        /// 动画
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callWhenFinished"></param>
        public override void Animate(Transform target, Action callWhenFinished)
        {
            FinishPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null)
            {
                Debug.LogError("[LegacyAnimationScreenTransition] No Animation component in " + target);
                if (callWhenFinished != null)
                {
                    callWhenFinished();
                }

                return;
            }

            targetAnimation.clip = clip;
            StartCoroutine(PlayAnimationRoutine(targetAnimation, callWhenFinished));
        }

        /// <summary>
        /// 播放动画事务
        /// </summary>
        /// <param name="targetAnimation"></param>
        /// <param name="callWhenFinished"></param>
        /// <returns></returns>
        private IEnumerator PlayAnimationRoutine(Animation targetAnimation, Action callWhenFinished)
        {
            previousCallbackWhenFinished = callWhenFinished;
            foreach (AnimationState state in targetAnimation)
            {
                state.time = playReverse ? state.clip.length : 0f;
                state.speed = playReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            FinishPrevious();
        }

        /// <summary>
        /// 完成上一个
        /// </summary>
        private void FinishPrevious()
        {
            if (previousCallbackWhenFinished != null)
            {
                previousCallbackWhenFinished();
                previousCallbackWhenFinished = null;
            }

            StopAllCoroutines();
        }
    }
}
