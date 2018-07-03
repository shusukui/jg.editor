using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.IO;

namespace jg.Editor.Library.Control
{
    public static class ShareClass
    {
        /// <summary>
        /// 判断是否double类型,且大于等于0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool UnDoubleValueCheck(object value)
        {
            if (value.GetType() == typeof(double))
            {
                double radius = (double)value;
                if (radius < 0) return false;
                return true;
            }
            else
                return false;
        }

        //创建移动动画X轴
        internal static Storyboard CeaterAnimation_Xmove(DependencyObject element, double formX, double toX, double animationTime, double waitTime)
        {
            Storyboard storyboard = new Storyboard();
            {
                if (waitTime > 0)
                {
                    DoubleAnimationUsingKeyFrames animation = moveAnimationX(element, formX, formX, waitTime);
                    storyboard.Children.Add(animation);
                }
                if (animationTime >= 0)
                {
                    DoubleAnimationUsingKeyFrames animation = moveAnimationX(element, formX, toX, animationTime);
                    storyboard.Children.Add(animation);
                }
            }
            return storyboard;
        }

        //创建移动动画X轴
        private static DoubleAnimationUsingKeyFrames moveAnimationX(DependencyObject element, double formX, double toX, double moveAnimationTime)
        {
            //x方向动画
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, element);
            DependencyProperty[] propertyChain = new DependencyProperty[]
                    {
                        System.Windows.Controls.Control.RenderTransformProperty,
                        TransformGroup.ChildrenProperty,
                        TranslateTransform.XProperty,
                    };
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0).(1)[3].(2)", propertyChain));
            //添加时间轴
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(formX, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0))));
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(toX, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(moveAnimationTime))));
            return animation;
        }
    }
}
