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
    [DataModel(ModelName = "切换开关")]
    public class Switch : DesignerItem
    {
        [Designable]
        [DisplayName("线条颜色")]
        public Brush LineColor
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(Switch), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(StrokeChanged)));

        private static void StrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Switch line = sender as Switch;
            (line.Template.FindName("switchPath", line) as Path).Fill = (Brush)new BrushConverter().ConvertFromString(e.NewValue.ToString());
        }

        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(Switch),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        Switch obj = (Switch)sender;
                        Path path1 = obj.Template.FindName("switchPath", obj) as Path;

                        if ((int)e.NewValue == 1)//运行
                        {
                            obj.Style = obj.FindResource("StyleKeySwitch") as Style;
                        }
                        else
                        {
                            obj.Style = obj.FindResource("StyleKeySwitchLeft") as Style;
                        }
                        
                    }));

        [DataModelField]
        [DisplayName("颜色")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        #region 构造
        private bool _isLeftSwitch=false;
        static Switch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch), new FrameworkPropertyMetadata(typeof(Switch)));
        }
        public Switch()
        {
            base._groupName = "常用控件";
            base._displayName = "切换开关";
            this.MouseDown += Switch_MouseDown;
        }

        void Switch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Switch obj = (Switch)sender;
            Path path1 = obj.Template.FindName("switchPath", obj) as Path;
            if(!_isLeftSwitch)
            {
                obj.Style = obj.FindResource("StyleKeySwitch") as Style;
                _isLeftSwitch = true;
            }
            else
            {
                obj.Style = obj.FindResource("StyleKeySwitchLeft") as Style;
                _isLeftSwitch =false;
            }
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
                var style = FindResource("StyleKeySwitch") as Style;
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
