using Wss.FoundationCore.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Wss.FoundationCore.Controls;

namespace Common.Package.PublicControl
{
    public class Light : DesignerItem
    {
        [Designable]
        [DisplayName("线条颜色")]
        public Brush LineColor
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(Light), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(StrokeChanged)));

        private static void StrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Light line = sender as Light;
            (line.Template.FindName("lightPath", line) as Path).Fill = (Brush)new BrushConverter().ConvertFromString(e.NewValue.ToString());
        }

        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(Light),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        Light obj = (Light)sender;
                        Path path1 = obj.Template.FindName("lightPath", obj) as Path;

                        if ((int)e.NewValue == 1)//运行
                        {
                            path1.Fill = new SolidColorBrush(Color.FromRgb(255, 1, 1));
                        }
                        else if ((int)e.NewValue == 2) path1.Fill = new SolidColorBrush(Color.FromRgb(33, 255, 33));
                        else path1.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    }));

        [DataModelField]
        [DisplayName("颜色（红1绿2蓝3）")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        #region 构造

        static Light()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Light), new FrameworkPropertyMetadata(typeof(Light)));
        }
        public Light()
        {
            base._groupName = "常用控件";
            base._displayName = "指示灯";
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
                var style = FindResource("StyleKeyLight") as Style;
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
