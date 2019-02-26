using Wss.FoundationCore.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wss.FoundationCore.Controls;

namespace Common.Package.PublicControl
{
    [DataModel(ModelName = "箭头")]
    public class Arrow : DesignerItem
    {
        [Designable]
        [DisplayName("线条颜色")]
        public Brush LineColor
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(Arrow), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(StrokeChanged)));

        private static void StrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Arrow line = sender as Arrow;
            (line.Template.FindName("arrowPath", line) as Path).Fill = (Brush)new BrushConverter().ConvertFromString(e.NewValue.ToString());
        }

        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(Arrow),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        #region 根据旋转方向设定动画
                        Arrow obj = (Arrow)sender;
                        int direction = 0;//右：0 下：1 左：2 上：3
                        if ((int)e.NewValue == 1)//1代表动画开启
                        {
                            int tempAngle=(int)obj.Container.Angle % 360;;
                            switch (tempAngle)//顺时针角度,有负数
                            {
                                case 0:
                                    direction = 0;
                                    break;
                                case 90:
                                    direction = 1;
                                    break;
                                case 180:
                                    direction = 2;
                                    break;
                                case 270:
                                    direction = 3;
                                    break;
                                case -90:
                                    direction = 3;
                                    break;
                                case -180:
                                    direction = 2;
                                    break;
                                case -270:
                                    direction = 1;
                                    break;
                                default:
                                    break;
                            }
                            if ((int)obj.Container.ScaleX == -1)//-1有水平翻转
                            {
                                if (direction == 0) direction = 2;
                                else if (direction == 2) direction = 0;
                            }
                            if ((int)obj.Container.ScaleY == -1)
                            {
                                if (direction == 1) direction = 3;
                                else if (direction == 3) direction = 1;
                            }
                            Storyboard story=null;
                            Style style=null;
                            Arrow arrowwwww = new Arrow();
                            switch (direction)//根据箭头方向给方向的Style
                            {
                                case 0:
                                    obj.Style = obj.FindResource("StyleKeyArrowRight") as Style;
                                    obj.ApplyTemplate();
                                    story = (Storyboard)obj.FindResource("Storyboard1");
                                    break;
                                case 1:
                                    obj.Style = obj.FindResource("StyleKeyArrowBottom") as Style;
                                    obj.ApplyTemplate();
                                    story = (Storyboard)obj.FindResource("Storyboard2");
                                    break;
                                case 2:
                                    obj.Style = obj.FindResource("StyleKeyArrowLeft") as Style;
                                    obj.ApplyTemplate();
                                    story = (Storyboard)obj.FindResource("Storyboard3");
                                    break;
                                case 3:
                                    obj.Style = obj.FindResource("StyleKeyArrowTop") as Style;
                                    obj.ApplyTemplate();
                                    //obj.ApplyAnimationClock(obj.Style, new AnimationClock());
                                    //<RotateTransform Angle="90"/>
                                    //obj.RenderTransform = 270;
                                    story = (Storyboard)obj.FindResource("Storyboard4");
                                    break;
                            }
                            
                            //story.BeginAnimation(obj,Timeline.FillBehaviorProperty);
                            obj.BeginStoryboard(story);
                            //obj.BeginInit();
                        }
                        else //动画关闭
                        {
                            obj.Style = obj.FindResource("StyleKeyArrow") as Style;
                        }
                        #endregion
                    }));

        [DataModelField]
        [DisplayName("动画开启（1开启，其它关闭）")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        #region 构造

        static Arrow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Arrow), new FrameworkPropertyMetadata(typeof(Arrow)));
        }
        public Arrow()
        {
            base._groupName = "常用控件";
            base._displayName = "箭头";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/PublicControl/Style.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyArrow") as Style;
                return style;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OnGetToolBoxStyle方法");
                return null;
            }
        }

        #endregion
    }
}
