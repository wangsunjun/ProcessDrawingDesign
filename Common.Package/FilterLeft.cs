using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using Wss.FoundationCore.Controls;

namespace Common.Package
{
    [DataModel(ModelName = "过滤器左")]
    public class FilterLeft : DesignerItem
    {
        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(FilterLeft),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (FilterLeft)sender;
                        if ((int)e.NewValue == 1)//运行
                        {

                            obj.Style = obj.FindResource("StyleKeyFilterLeftTag") as Style;
                        }
                        else if ((int)e.NewValue == -1)//故障
                        {
                            obj.Style = obj.FindResource("StyleKeyFilterLeftWarnningTag") as Style;
                        }
                        else//停止
                        {
                            //obj.Style = obj.FindResource("StyleKeyBackwaterValveStopTag") as Style;
                        }
                    }));


        /// <summary>
        ///     真：运行，假：停止
        /// </summary>
        [DataModelField]
        [DisplayName("运行状态")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        #region 构造

        static FilterLeft()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (FilterLeft),
                new FrameworkPropertyMetadata(typeof (FilterLeft)));
        }
        public FilterLeft()
        {
            base._groupName = "工艺图控件";
            base._displayName = "过滤器左";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/FilterLeftStyle.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyFilterLeftTag") as Style;
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