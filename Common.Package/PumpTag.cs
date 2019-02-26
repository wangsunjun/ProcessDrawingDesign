using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Wss.FoundationCore.Attributes;
using Wss.FoundationCore.Controls;

namespace Common.Package
{
    [DataModel(ModelName = "泵")]
    [DataModelField(FieldName = "转速", TargetGroup = TargetGroup.Data)]
    [DataModelField(FieldName = "运行模式", TargetGroup = TargetGroup.Mode)]
    [DataModelField(FieldName = "输出功率", TargetGroup = TargetGroup.Info)]
    [DataModelField(FieldName = "启停", TargetGroup = TargetGroup.Config)]
    [DataModelField(FieldName = "转速上限", TargetGroup = TargetGroup.Config)]
    public class PumpTag : DesignerItem  ,IPropertyValueChanged
    {
        frmPump _frm;

        public static DependencyProperty RunningStateProperty = DependencyProperty.Register("RunningState", typeof (int), typeof (PumpTag),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (PumpTag) sender;
                        if ((int) e.NewValue == 1)obj.Style = obj.FindResource("StyleKeyPumpRunningTag") as Style; //运行图标
                        else if ((int) e.NewValue == 0) obj.Style = obj.FindResource("StyleKeyPumpWarningTag") as Style;
                        else obj.Style = obj.FindResource("StyleKeyPumpStopTag") as Style;
                    }));
        /// <summary>
        /// 真：运行，假：停止
        /// </summary>
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("运行状态")]
        [PropertyGuid("111ecdf9-3f2e-4049-a828-8a667f720b37")]
        public int RunningState
        {
            get { return (int) GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        private string _runningStateString = Guid.NewGuid().ToString();
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("asdf")]
        [PropertyGuid("111ecdf9-3f2e-4049-a828-8a667f720b37")]
        public string RunningStateString
        {
            get
            {
                return _runningStateString;
            }
            set
            {
                _runningStateString = value;
            }
        }
        #region 暂时不用
        public static DependencyProperty isrunningProperty = DependencyProperty.Register("IsRunning", typeof(bool),
            typeof(PumpTag), new PropertyMetadata(true));

        public static DependencyProperty isfaultProperty = DependencyProperty.Register("IsFault", typeof(bool),
            typeof(PumpTag), new PropertyMetadata(true));

        public static DependencyProperty isremoteProperty = DependencyProperty.Register("IsRemote", typeof(bool),
            typeof(PumpTag), new PropertyMetadata(true));

        public static DependencyProperty isautoProperty = DependencyProperty.Register("IsAuto", typeof(bool),
            typeof(PumpTag), new PropertyMetadata(true));

        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("是否运行")]
        [PropertyGuid("222ecdf9-3f2e-4049-a828-8a667f720b37")]
        public bool IsRunning
        {
            get { return (bool)GetValue(isrunningProperty); }
            set
            {
                SetValue(isrunningProperty, value);
                if (value)
                {
                    //运行图标
                    Style = FindResource("StyleKeyPumpRunningTag") as Style;
                }
                else
                {
                    Style = FindResource("StyleKeyPumpWarningTag") as Style;
                }
            }
        }

        /// <summary>
        ///     真：故障，假：正常
        /// </summary>
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("是否故障")]
        [PropertyGuid("333ecdf9-3f2e-4049-a828-8a667f720b37")]
        public bool IsFault
        {
            get { return (bool)GetValue(isfaultProperty); }
            set
            {
                SetValue(isfaultProperty, value);
                if (!value)
                {
                    if (IsRunning)
                    {
                        //运行图标
                        Style = FindResource("StyleKeyPumpRunningTag") as Style;
                    }
                    else
                    {
                        Style = FindResource("StyleKeyPumpWarningTag") as Style;
                    }
                }
            }
        }

        /// <summary>
        ///     真：自动，假：手动
        /// </summary>
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("是否远程")]
        [PropertyGuid("444ecdf9-3f2e-4049-a828-8a667f720b37")]
        public bool IsRemote
        {
            get { return (bool)GetValue(isremoteProperty); }
            set
            {
                SetValue(isremoteProperty, value);
                //控制模式信息=””
                if (value)
                {
                    //控制模式信息+=”远程”
                }
            }
        }

        /// <summary>
        ///     真：远程，假：本地
        /// </summary>
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("是否自动")]
        public bool IsAuto
        {
            get { return (bool)GetValue(isautoProperty); }
            set
            {
                SetValue(isautoProperty, value);
                //控制模式信息=””
                if (value)
                {
                    //控制模式信息+=”自动”
                }
            }
        }
        #endregion
        #region 构造

        static PumpTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (PumpTag), new FrameworkPropertyMetadata(typeof (PumpTag)));
        }
        public PumpTag()
        {
            base._groupName = "工艺图控件";
            base._displayName = "泵";
        }

        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/PumpTagStyle.xaml", UriKind.Relative));
        }
        protected override void OnShowWindow()
        {
            base.OnShowWindow();
            string strr = base._controlID.ToString();
            List<ISerializeProperty> list = base._listProperty;

            if (_frm == null)
            {
                _frm = new frmPump("泵自定义");
                _frm.ShowDialog();
            }
            _frm.ShowPointValue(list);
        }
        protected override Style OnGetToolBoxStyle()
        {
            return FindResource("StyleKeyPumpTag") as Style;
        }

        #endregion

        public override void PropertyChange(string propertyName, object changePValue)
        {
            base.PropertyChange(propertyName, changePValue);
            if (propertyName == "RunningState")
            {
                RunningState = (int)changePValue;
            }
        }

        public bool PropertyChanged(object value, string propertyName)
        {
            if(propertyName== "RunningState")
            {
                RunningState = (int)(value);
            }
            return true;
        }
    }
}