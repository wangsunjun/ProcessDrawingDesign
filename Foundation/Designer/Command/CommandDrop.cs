using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Wss.FoundationCore.Attributes;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Models;
//using Wss.Objects;
//using Wss.Objects.Linkages;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ��קͼԪ
    /// </summary>
    public class CommandDrop : Command
    {
        private DragEventArgs _drgeEarg;
        private List<DesignerContainer> _mDi;
        private List<double> _mNewHeight;
        private List<double> _mNewLeft;
        private List<double> _mNewTop;
        private List<double> _mNewWidth;
        private List<double> _mOldHeight;
        private List<double> _mOldLeft;
        private List<double> _mOldTop;
        private List<double> _mOldWidth;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            _drgeEarg = param[0] as DragEventArgs;
            OnDrop(_drgeEarg, mgr);
            return true;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (_mDi == null) return false;
            foreach (var item1 in _mDi)
            {
                mgr.CurrentCanvas.Remove(item1);
            }

            mgr.CurrentCanvas.SelectionService.ClearSelection();

            var pv = mgr.CurrentCanvas.FindName("pv") as PropertyView;
            if (pv != null) pv.InitProView();
            mgr.CurrentCanvas.Focus();
            return true;

        }

        public override bool ReRun(CommandManager mgr)
        {
            if (_drgeEarg != null)
            {
                OnDrop(_drgeEarg, mgr);
            }
            return true;
        }

        public static void Run(CommandManager cmgr, DragEventArgs items)
        {
            cmgr.Run<CommandDrop>(items);
        }
        protected void OnDrop(DragEventArgs e, CommandManager mgr)
        {
            var dragObject = (DragObject)e.Data.GetData(typeof(DragObject));
            if (dragObject == null || String.IsNullOrEmpty(dragObject.Xaml)) return;
            DesignerContainer newItem = null;
            try
            {
                var xe = XElement.Load(new StringReader(dragObject.Xaml));
                newItem = xe.ToDesignerContainer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (newItem != null)
            {
               
                var position = e.GetPosition(mgr.CurrentCanvas);

                int left;
                int top;
                if (dragObject.DesiredSize.HasValue)
                {
                    var desiredSize = dragObject.DesiredSize.Value;
                    newItem.Width = double.IsNaN(newItem.Item.MinWidth) || newItem.Item.MinWidth == 0 || newItem.Item.MinWidth < desiredSize.Width
                        ? Convert.ToInt32(desiredSize.Width)
                        : Convert.ToInt32(newItem.Item.MinWidth);
                    newItem.Height = double.IsNaN(newItem.Item.MinHeight) || newItem.Item.MinHeight == 0 || newItem.Item.MinHeight < desiredSize.Height
                        ? Convert.ToInt32(desiredSize.Height)
                        : Convert.ToInt32(newItem.Item.MinHeight);
                    if (_mDi == null)
                    {
                        _mDi = new List<DesignerContainer>();
                    }
                    _mDi.Add(newItem);
                    if (_mOldWidth == null)
                    {
                        _mOldWidth = new List<double>();
                    }
                    _mOldWidth.Add(desiredSize.Width);
                    if (_mOldHeight == null)
                    {
                        _mOldHeight = new List<double>();
                    }
                    _mOldHeight.Add(desiredSize.Height);
                    if (_mNewWidth == null)
                    {
                        _mNewWidth = new List<double>();
                    }
                    _mNewWidth.Add(newItem.Width);
                    if (_mNewHeight == null)
                    {
                        _mNewHeight = new List<double>();
                    }
                    _mNewHeight.Add(newItem.Height);
                    left = Convert.ToInt32(Math.Max(0, position.X - newItem.Width/2));
                    top = Convert.ToInt32(Math.Max(0, position.Y - newItem.Height/2));
                    if (_mOldLeft == null)
                    {
                        _mOldLeft = new List<double>();
                    }
                    _mOldLeft.Add(left);
                    if (_mOldTop == null)
                    {
                        _mOldTop = new List<double>();
                    }
                    _mOldTop.Add(top);
                    left = left + newItem.Width > mgr.CurrentCanvas.ActualWidth
                        ? (int) (mgr.CurrentCanvas.ActualWidth - newItem.Width)
                        : left;
                    top = top + newItem.Height > mgr.CurrentCanvas.ActualHeight
                        ? (int) (mgr.CurrentCanvas.ActualHeight - newItem.Height)
                        : top;
                    if (_mNewLeft == null)
                    {
                        _mNewLeft = new List<double>();
                    }
                    _mNewLeft.Add(left);
                    if (_mNewTop == null)
                    {
                        _mNewTop = new List<double>();
                    }
                    _mNewTop.Add(top);
                }
                else
                {
                    left = Convert.ToInt32(Math.Max(0, position.X));
                    top = Convert.ToInt32(Math.Max(0, position.Y));
                    if (_mOldLeft == null)
                    {
                        _mOldLeft = new List<double>();
                    }
                    _mOldLeft.Add(left);
                    if (_mOldTop == null)
                    {
                        _mOldTop = new List<double>();
                    }
                    _mOldTop.Add(top);
                    left = left + newItem.Width > mgr.CurrentCanvas.ActualWidth
                        ? (int) (mgr.CurrentCanvas.ActualWidth - newItem.Width)
                        : left;
                    top = top + newItem.Height > mgr.CurrentCanvas.ActualHeight
                        ? (int) (mgr.CurrentCanvas.ActualHeight - newItem.Height)
                        : top;
                    if (_mNewLeft == null)
                    {
                        _mNewLeft = new List<double>();
                    }
                    _mNewLeft.Add(left);
                    if (_mNewTop == null)
                    {
                        _mNewTop = new List<double>();
                    }
                    _mNewTop.Add(top);
                    //DesignerCanvas.SetLeft(newItem, Convert.ToInt32(Math.Max(0, position.X)));
                    //DesignerCanvas.SetTop(newItem, Convert.ToInt32(Math.Max(0, position.Y)));
                }
                Canvas.SetLeft(newItem, left);
                Canvas.SetTop(newItem, top);

                mgr.CurrentCanvas.Add(newItem);

                //update selection
                mgr.CurrentCanvas.SelectionService.SelectItem(newItem);
                var pv = mgr.CurrentCanvas.FindName("pv") as PropertyView;
                if (pv != null) pv.InitProView();
                newItem.Focus();
            }

            e.Handled = true;
        }
    }
}