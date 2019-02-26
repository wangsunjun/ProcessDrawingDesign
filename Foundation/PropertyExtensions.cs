using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Wss.FoundationCore.Attributes;
using Wss.Foundation.Controls;
using OmniLib.Reflection;

namespace Wss.Foundation
{
    public class PropertyEditer : Window        //继承自window
    {
        public object Property { get; set; }
    }

    public class PropertyEditerButton : Button  //继承自button
    {
        public static readonly DependencyProperty _property = DependencyProperty.Register("Property", typeof (object), typeof (PropertyEditerButton));

        public PropertyEditerButton(Type editer)
        {
            Editer = editer;
            Click += PropertyEditerButton_Click;
        }

        public object Property
        {
            get { return GetValue(_property); }
            set { SetValue(_property, value); }
        }

        public Type Editer { get; private set; }

        private void PropertyEditerButton_Click(object sender, RoutedEventArgs e)
        {
            if (Content != null)
            {
                PropertyEditer pe = null;
                var fi = Editer.FastGetConstructor();
                if (fi != null)
                {
                    pe = fi.Invoke() as PropertyEditer;
                }
                if (pe != null)
                {
                    pe.Property = Property;
                    pe.Owner = Application.Current.MainWindow;
                    pe.ShowDialog();
                    Property = pe.Property;
                }
            }
        }
    }

    public static class PropertyExtensions
    {
        public static double itemO = 0.0;

        //将XElement节点转换为DesignerItem实例,对于结构不符合的节点，将返回null
        public static DesignerContainer ToDesignerContainer(this XElement xElement)
        {
            return DesignAttributeService.DeSerializFromXElement(xElement) as DesignerContainer;
        }

        //将XElement节点转换为一组DesignerItem
        public static List<DesignerContainer> ToDesignerContainers(this XElement xElement)
        {
            var result = new List<DesignerContainer>();
            if (xElement == null) return result;
            var items = xElement.Elements();
            result.AddRange(items.Select(item =>
                DesignAttributeService.DeSerializFromXElement(item) as DesignerContainer
                ).Where(ditem => ditem != null));
            return result;
        }

        private static Point CreatePointFromString(string s)
        {
            double x = double.Parse(s.Split(',')[0]), y = double.Parse(s.Split(',')[1]);
            double w = 0, h = 0; //AppParams.CanvasWidth, h = AppParams.CanvasHeight;
            var p = new Point(w == 0 ? x : (x/w), h == 0 ? y : (y/h));
            return p;
        }

        //将List<DesignerContainer>转换为XElement节点
        public static XElement ToXElement(this IEnumerable<DesignerContainer> obj)
        {
            var result = new XElement("DesignerContainers");
            if (obj == null) return result;
            foreach (var item in obj)
            {
                result.Add(DesignAttributeService.SerializToXElement(item));
            }
            return result;
        }
    }

    public class NameRule : ValidationRule//控件内容规则化
    {
        //public DesignerContainer di { get; set; }
        ////public List<string> lName { get; set; }
        //public string oldName { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //if (lName.Contains(value) && !oldName.Equals(value))
            //if ((di.ParentCanvas as DesignerCanvas).BaissObject.ContainsName(value.ToString()) && !oldName.Equals(value))
            {
                //MessageBox.Show("此名称已被使用,请重新命名");
                // return new ValidationResult(false, "此名称已被使用,请重新命名");
            }
            //else
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    MessageBox.Show("名称不能为空!");
                    return new ValidationResult(false, "名称不能为空!");
                }
                //  if (!string.IsNullOrEmpty(di.StyleName))
                //  {
                //      di._name = value.ToString();
                //      di.Item._name = value.ToString();
                //  }
                // // di.BasisObject.ChangeName(value.ToString());
                ////  di.Item.BasisObject.ChangeName(value.ToString());
                //  //string prefix = "新窗口.绘图区" + "." + (string.IsNullOrEmpty(AppParams.CurrentZDName) ? "" : "." + string.IsNullOrEmpty(AppParams.CurrentZDName)) + AppParams.CurrentMenuName + ".";
                // //MateManager.ChangeMateName(prefix + oldName, prefix + value.ToString());
                //  //di.CheckLinked(DesignerCanvas.designerCanvas , true);
                //  di._name = value.ToString();
                //  di.Item._name = value.ToString();
                return new ValidationResult(true, null);
            }
        }
    }
}