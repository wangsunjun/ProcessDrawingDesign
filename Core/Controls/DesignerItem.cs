using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Wss.FoundationCore.Attributes;

namespace Wss.FoundationCore.Controls
{
    /// <summary>
    /// 控件属性值改变委托
    /// </summary>
    /// <param name="di">控件</param>
    /// <param name="id">属性名称</param>
    /// <param name="value">改变的值</param>
    public delegate void DWriteRegister(DesignerItem di,string id, object value);
    public delegate void DShowWindow(DesignerItem di);
   

    [SerializeProperty("Tag")]
    public class DesignerItem : Control
    {
        public string _groupName = "";    //组态控件分组名称
        public string _displayName = "";  //组态控件显示名称
        //public string _pointName = "";      //要关联的后台点位数据
        //public string _id="";
        public Style ToolBoxStyle { get; private set; }
        public Style DesignStyle { get; private set; }
        public bool IsInToolBox { get; internal set; }
        public List<ISerializeProperty> _listProperty;
        
        public DWriteRegister _DWriteRegister;
        public DShowWindow _DShowWindow;
        
        public Guid _controlID;
        public bool _isReturn;
        public string buildingID = "";

        public DesignerItem()
        {
            //var themeResources = @"/" + this.GetType().Assembly.GetName() + ";component/themes/" +
            //                     this.GetType()._name + ".xaml";
            //var themeUri = new Uri(themeResources, UriKind.Relative);
            var uris = new List<Uri>();
            OnMergeResource(ref uris);
            if (uris == null)
            {
                throw new InvalidOperationException("合并的Uri列表不能为null");
            }
            foreach (var uri in uris)
            {
                ResourceDictionary dictionary;
                if (uri == null)
                {
                    throw new InvalidOperationException("合并的Uri不能为null");
                }
                if (uri.IsAbsoluteUri)
                {
                    throw new InvalidOperationException("不能合并绝对Uri");
                }
                try
                {
                    var x = new Uri(Application.Current.StartupUri, uri);
                    var z = File.Exists(x.AbsolutePath);
                    dictionary = Application.LoadComponent(uri) as ResourceDictionary;
                }
                catch (Exception)
                {
                    continue;
                }
                if (dictionary != null)
                {
                    Resources.MergedDictionaries.Add(dictionary);
                }
            }

            //TextBlock tb = new TextBlock();
            //tb.Text = "测试提示框";
            //ToolTipContent = tb;
            //OtherTask();
            MinHeight = 1;
            MinWidth = 1;
        }

        protected virtual void OnMergeResource(ref List<Uri> resources)
        {
        }
        protected virtual void OnShowWindow()
        {

        }
        public void ShowWindow()
        {
            OnShowWindow();
        }
        public void ShowToolBoxStyle()
        {
            IsInToolBox = true;
            if (Style != null && DesignStyle==null)
            {
                DesignStyle = Style;
            }
            if (ToolBoxStyle == null)
            {
               ToolBoxStyle= OnGetToolBoxStyle();
            }
            Style = ToolBoxStyle;
        }


        //取得图元的容器
        public BaseContainer Container
        {
            get { return Parent as BaseContainer; }
        }

        public void ShowDesignStyle()
        {
            IsInToolBox = false;
            if (DesignStyle != null)
            {
                Style = DesignStyle;
            }
        }

        protected virtual Style OnGetToolBoxStyle()
        {
            return this.Style;
        }

        #region
        /// <summary>
        ///     取得图元所在画布
        /// </summary>
        //public DesignerCanvas ParentCanvas
        //{
        //    get
        //    {
        //        if (Container == null) return null;
        //        return Container.ParentCanvas;
        //    }
        //}
        #endregion

        public virtual void PropertyChange(string propertyName, object changePValue)
        {

        }
    }
}