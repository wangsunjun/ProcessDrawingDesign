using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Wss.Foundation.Controls
{
    public class AyRadioList : Panel
    {
        private AyRadioCheckedConverter converter = new AyRadioCheckedConverter();

        public AyRadioList(){}

        #region WrapPanel拓展依赖属性
        /// <summary>
        /// radio参考属性的 属性值
        /// </summary>
        public string CheckedRadioValue
        {
            get { return (string)GetValue(CheckedRadioValueProperty); }
            set { SetValue(CheckedRadioValueProperty, value); }
        }
        public static readonly DependencyProperty CheckedRadioValueProperty =  DependencyProperty.Register("CheckedRadioValue", typeof(string), typeof(AyRadioList), new PropertyMetadata(""));

        /// <summary>
        /// radio参考属性
        /// </summary>
        public string CheckedRadioPath
        {
            get { return (string)GetValue(CheckedRadioPathProperty); }
            set { SetValue(CheckedRadioPathProperty, value); }
        }

        public static readonly DependencyProperty CheckedRadioPathProperty = DependencyProperty.Register("CheckedRadioPath", typeof(string), typeof(AyRadioList), new PropertyMetadata(""));
        #endregion

        /// <summary>
        /// 决定是否立即换行
        /// </summary>
        public static DependencyProperty LineBreakBeforeProperty;

        static AyRadioList()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.AffectsArrange = true;
            metadata.AffectsMeasure = true;
            LineBreakBeforeProperty = DependencyProperty.RegisterAttached("LineBreakBefore", typeof(bool), typeof(AyRadioList), metadata);
        }
        public static void SetLineBreakBefore(UIElement element, Boolean value)
        {
            element.SetValue(LineBreakBeforeProperty, value);
        }
        public static Boolean GetLineBreakBefore(UIElement element)
        {
            return (bool)element.GetValue(LineBreakBeforeProperty);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size currentLineSize = new Size();
            Size panelSize = new Size();

            foreach (UIElement element in base.InternalChildren)
            {
                element.Measure(constraint);
                Size desiredSize = element.DesiredSize;

                if (GetLineBreakBefore(element) ||
                    currentLineSize.Width + desiredSize.Width > constraint.Width)
                {
                    panelSize.Width = Math.Max(currentLineSize.Width, panelSize.Width);
                    panelSize.Height += currentLineSize.Height;
                    currentLineSize = desiredSize;
                    
                    if (desiredSize.Width > constraint.Width)
                    {
                        panelSize.Width = Math.Max(desiredSize.Width, panelSize.Width);
                        panelSize.Height += desiredSize.Height;
                        currentLineSize = new Size();
                    }
                }
                else
                {
                    currentLineSize.Width += desiredSize.Width;
                    // Make sure the line is as tall as its tallest element.
                    currentLineSize.Height = Math.Max(desiredSize.Height, currentLineSize.Height);
                }
            }

            // Return the size required to fit all elements.
            // Ordinarily, this is the width of the constraint, and the height
            // is based on the size of the elements.
            // However, if an element is wider than the width given to the panel,
            // the desired width will be the width of that line.
            panelSize.Width = Math.Max(currentLineSize.Width, panelSize.Width);
            panelSize.Height += currentLineSize.Height;
            return panelSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            int firstInLine = 0;
            Size currentLineSize = new Size();
            double accumulatedHeight = 0;

            UIElementCollection elements = base.InternalChildren;
            for (int i = 0; i < elements.Count; i++)
            {

                Size desiredSize = elements[i].DesiredSize;
                if (GetLineBreakBefore(elements[i]) || currentLineSize.Width + desiredSize.Width > arrangeBounds.Width) //need to switch to another line
                {
                    arrangeLine(accumulatedHeight, currentLineSize.Height, firstInLine, i);
                    accumulatedHeight += currentLineSize.Height;
                    currentLineSize = desiredSize;

                    if (desiredSize.Width > arrangeBounds.Width) //the element is wider then the constraint - give it a separate line                    
                    {
                        arrangeLine(accumulatedHeight, desiredSize.Height, i, ++i);
                        accumulatedHeight += desiredSize.Height;
                        currentLineSize = new Size();
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    currentLineSize.Width += desiredSize.Width;
                    currentLineSize.Height = Math.Max(desiredSize.Height, currentLineSize.Height);
                }
            }
            if (firstInLine < elements.Count) arrangeLine(accumulatedHeight, currentLineSize.Height, firstInLine, elements.Count);

            return arrangeBounds;
        }
        
        private void arrangeLine(double y, double lineHeight, int start, int end)
        {
            double x = 0;
            UIElementCollection children = InternalChildren;

            for (int i = start; i < end; i++)
            {
                UIElement child = children[i];
                RadioButton item = child as RadioButton;
                if (item != null)
                {
                    if (BindingOperations.GetBinding(item, RadioButton.IsCheckedProperty) == null)
                    {
                        if (CheckedRadioPath != "")
                        {
                            var checkPath = item.GetType().GetProperty(CheckedRadioPath);

                            //Binding binding = new Binding { Path = new PropertyPath("CheckedRadioValue"), Source = this, Mode = BindingMode.TwoWay, Converter = converter, ConverterParameter = new Func<string>(() => checkPath.GetValue(item) as string) };
                            Binding binding = new Binding { Path = new PropertyPath("CheckedRadioValue"), Source = this, Mode = BindingMode.TwoWay, Converter = converter, ConverterParameter = checkPath.GetValue(item,null) };
                            item.SetBinding(RadioButton.IsCheckedProperty, binding);

                        }
                    }
                }
                #region MyRegion
       
                //            //创建双向绑定
                //            Binding binding = new Binding { Path = new PropertyPath(RadioCheckedPath), Mode = BindingMode.TwoWay, Converter = converter, ConverterParameter = item.Tag };
                //            BindingOperations.SetBinding(item, RadioButton.IsCheckedProperty, binding);

                //            Binding binding = new Binding { Path = new PropertyPath("RadioCheckedValue"), Source = this, Mode = BindingMode.TwoWay, Converter = converter, ConverterParameter = item.Tag };
                //            item.SetBinding(RadioButton.IsCheckedProperty, binding);

                #endregion

                child.Arrange(new Rect(x, y, child.DesiredSize.Width, lineHeight));
                x += child.DesiredSize.Width;
            }
        }
    }

    [ValueConversion(typeof(string), typeof(bool))]
    public class AyRadioCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = parameter as string ?? "";
            string v = value as string ?? "";
            if (p != "" && v.Trim() == p.Trim())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (parameter is Func<string>) return ((Func<string>)parameter)() ?? "";
            return parameter;
        }
    }
}

