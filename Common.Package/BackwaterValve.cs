using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using Wss.FoundationCore.Controls;

namespace Common.Package
{
    [DataModel(ModelName = "电磁阀")]
    //此处字段为测试用，用后删除
    [DataModelField(FieldName="上限",TargetGroup=TargetGroup.Config)]
    [DataModelField(FieldName = "开关", TargetGroup = TargetGroup.Config)]
    public class BackwaterValve : DesignerItem
    {
        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(BackwaterValve),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (BackwaterValve)sender;
                        if ((int)e.NewValue == 1)//运行
                        {

                            obj.Style = obj.FindResource("StyleKeyBackwaterValveRunningTag") as Style;
                            obj.ToolTip = "电磁阀";
                        }
                        else if ((int)e.NewValue == -1)//故障
                        {
                            obj.Style = obj.FindResource("StyleKeyBackwaterValveWarningTag") as Style;
                        }
                        else//停止
                        {
                            obj.Style = obj.FindResource("StyleKeyBackwaterValveStopTag") as Style;
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
        private string _rPointName = "PLCLeft.报警指示灯";
        #region 构造

        static BackwaterValve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BackwaterValve),
                new FrameworkPropertyMetadata(typeof (BackwaterValve)));
            
        }
        public BackwaterValve()
        {
            base._groupName = "工艺图控件";
            base._displayName = "电磁阀";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/BackwaterValveStyle.xaml", UriKind.Relative));
        }
        protected override void OnShowWindow()
        {
            base.OnShowWindow();
            new frmBackwaterValve("阀自定义").ShowDialog();
        }
        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyBackwaterValveTag") as Style;
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