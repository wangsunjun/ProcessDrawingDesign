//using Wss.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wss.Foundation.Controls
{
    public class ConfigViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextPropertyTemplate { get; set; }
        public DataTemplate TextBoxPropertyTemplate { get; set; }
        public DataTemplate ComboBoxPropertyTemplate { get; set; }
        public DataTemplate BoolPropertyTemplate { get; set; }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        //public override DataTemplate SelectTemplate(object item, DependencyObject container)
        //{
        //    var itemvalue=item as IGeneralValueable;
        //    if(itemvalue!=null){
        //    if (itemvalue.Value.GetTypeCode() == TypeCode.Boolean)
        //    {
        //        //ContentPresenter myContentPresenter = container as ContentPresenter;
        //        ////var btn = BoolPropertyTemplate.FindName("btnSendValue", myContentPresenter) as Button;
        //        //DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
        //        //Button btn = myDataTemplate.FindName("btnSendValue", myContentPresenter) as Button;
        //        //if (btn != null) {
        //        //    btn.Click += btn_Click;
        //        //}
        //        return BoolPropertyTemplate;
        //    }
        //        else if (itemvalue.Value.GetTypeCode() == TypeCode.String)
        //    {
        //        return TextBoxPropertyTemplate;
        //    }
        //    }
        //    return TextBoxPropertyTemplate;
        //}

        void btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("测试");
        }
    }
}
