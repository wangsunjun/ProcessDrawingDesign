using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using Wss.FoundationCore.Attributes;
using Wss.FoundationCore.Controls;

namespace Common.Package
{
    [DataModel(ModelName = "除污罐")]
    public class DecontaminationTank : DesignerItem
    {
        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(DecontaminationTank),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (DecontaminationTank)sender;
                        if ((int)e.NewValue == 1)//运行
                        {

                            obj.Style = obj.FindResource("StyleKeyDecontaminationTankRunningTag") as Style;
                        }
                        else if ((int)e.NewValue == -1)//故障
                        {
                            obj.Style = obj.FindResource("StyleKeyDecontaminationTankWarnningTag") as Style;
                        }
                        else//停止
                        {
                            obj.Style = obj.FindResource("StyleKeyDecontaminationTankTag") as Style;
                        }
                    }));


        /// <summary>
        ///     真：运行，假：停止
        /// </summary>
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("运行状态")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        #region 构造

        static DecontaminationTank()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (DecontaminationTank),
                new FrameworkPropertyMetadata(typeof (DecontaminationTank)));
        }
        public DecontaminationTank()
        {
            base._groupName = "工艺图控件";
            base._displayName = "除污罐";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/DecontaminationTankStyle.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyDecontaminationTankTag") as Style;
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