using Wss.FoundationCore.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Wss.FoundationCore.Controls;

namespace Common.Package.PublicControl
{
    [DataModel(ModelName = "按钮")]
    public class RunaButton : DesignerItem
    {
        private string _groupName = "常用控件";
        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }

        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(RunaButton),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (RunaButton)sender;
                        //if ((int)e.NewValue == 1)//运行
                        //{

                        //    obj.Style = obj.FindResource("StyleKeyBackwaterValveRunningTag2") as Style;
                        //}
                        //else if ((int)e.NewValue == -1)//故障
                        //{
                        //    obj.Style = obj.FindResource("StyleKeyBackwaterValveWarningTag2") as Style;
                        //}
                        //else//停止
                        //{
                        //    obj.Style = obj.FindResource("StyleKeyBackwaterValveStopTag2") as Style;
                        //}
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

        static RunaButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RunaButton),
                new FrameworkPropertyMetadata(typeof(RunaButton)));
        }
         public RunaButton()
        {
            base._groupName = "常用控件";
            base._displayName = "按钮";
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
                var style = FindResource("StyleKeyRunaButtonTag") as Style;
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
