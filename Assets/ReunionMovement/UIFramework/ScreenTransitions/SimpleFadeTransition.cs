using System;
using UnityEngine;

namespace GameLogic.UIFramework
{
    /// <summary>
    /// 这是一个作为内置示例实现的简单渐变。
    /// 我建议使用像DOTween这样的免费推特库(http://dotween.demigiant.com/)或推出自己的产品。
    /// 查看Examples项目以获得更强大和久经考验的选项：https://github.com/yankooliveira/uiframework_examples
    /// </summary>
    public class SimpleFadeTransition : ATransitionComponent
    {
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private bool fadeOut = false;

        private CanvasGroup canvasGroup;
        private float timer;
        private Action currentAction;
        private Transform currentTarget;

        private float startValue;
        private float endValue;

        private bool shouldAnimate;

        public override void Animate(Transform target, Action callWhenFinished)
        {
            if (currentAction != null)
            {
                canvasGroup.alpha = endValue;
                currentAction();
            }

            canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            if (fadeOut)
            {
                startValue = 1f;
                endValue = 0f;
            }
            else
            {
                startValue = 0f;
                endValue = 1f;
            }

            currentAction = callWhenFinished;
            timer = fadeDuration;

            canvasGroup.alpha = startValue;
            shouldAnimate = true;
        }

        private void Update()
        {
            if (!shouldAnimate)
            {
                return;
            }

            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(endValue, startValue, timer / fadeDuration);
            }
            else
            {
                canvasGroup.alpha = 1f;
                if (currentAction != null)
                {
                    currentAction();
                }

                currentAction = null;
                shouldAnimate = false;
            }
        }
    }
}