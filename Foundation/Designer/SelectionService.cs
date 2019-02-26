using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer
{
    /// <summary>
    /// 选择方法类
    /// </summary>
    internal class SelectionService
    {
        private readonly DesignerCanvas _designerCanvas;   //绘图区对象
        private List<ISelectable> _currentSelection;       //用来存放当前选中图元列表
        private DragResizeAdorner _dragResizeAdorner;

        public SelectionService(DesignerCanvas canvas)
        {
            _designerCanvas = canvas;
            if (_dragResizeAdorner == null)
            {
                // create rubberband adorner
                var adornerLayer = AdornerLayer.GetAdornerLayer(_designerCanvas);


                _dragResizeAdorner = new DragResizeAdorner(_designerCanvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Add(_dragResizeAdorner);
                }
            }
        }

        /// <summary>
        /// 当前选择的图元的集合
        /// </summary>
        internal List<ISelectable> SelectedDesignerContainer
        {
            get
            {
                if (_currentSelection == null)
                    _currentSelection = new List<ISelectable>();

                return _currentSelection;
            }
        }
        /// <summary>
        /// 选择指定图元
        /// </summary>
        /// <param name="item"></param>
        internal void SelectItem(ISelectable item)
        {
            ClearSelection();
            AddToSelection(item);
        }
        /// <summary>
        /// 将指定图元添加到集合
        /// </summary>
        /// <param name="item"></param>
        internal void AddToSelection(ISelectable item)
        {
            if (item == null) return;

            FrameworkElement frameworkElement = item as FrameworkElement;
            if (frameworkElement != null) frameworkElement.LayoutUpdated += SelectionService_LayoutUpdated;
            if (item is IGroupable)
            {
                var root = GetGroupRoot(item as IGroupable);
                if (root != null)
                {
                    item = root as ISelectable;
                }
            }
            item.IsSelected = true;
            SelectedDesignerContainer.Add(item);
        }
        private void SelectionService_LayoutUpdated(object sender, EventArgs e)
        {
            _dragResizeAdorner.UpdateAdorner();
        }

        //从集合中删除指定图元
        internal void RemoveFromSelection(ISelectable item)
        {
            item.IsSelected = false;
            SelectedDesignerContainer.Remove(item);
            var frameworkElement = item as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.LayoutUpdated -= SelectionService_LayoutUpdated;
            }
            //_dragResizeAdorner.UpdateAdorner();
        }

        //清空集合
        internal void ClearSelection()
        {
            SelectedDesignerContainer.ForEach(item => item.IsSelected = false);
            SelectedDesignerContainer.Clear();
            _dragResizeAdorner.UpdateAdorner();
        }

        //选择画布上的所有图元
        internal void SelectAll()
        {
            ClearSelection();
            //_currentSelection.AddRange(_designerCanvas.Children.OfType<ISelectable>());
            //_currentSelection.ForEach(Item => { if ((Item as UIElement).IsEnabled) { Item.IsSelected = true; }});

            foreach (var item in _designerCanvas.Children.OfType<ISelectable>())
            {
                DesignerContainer di = (item as DesignerContainer);
                item.IsSelected = di == null ? true : di.IsEnabled && di.Editable;
                if (item.IsSelected) SelectedDesignerContainer.Add(item);
            }
            _dragResizeAdorner.UpdateAdorner();
        }

        //从组合后的图元中取得图元的列表
        internal List<IGroupable> GetGroupMembers(IGroupable item)
        {
            var groupList = new List<IGroupable>();
            var rootItem = GetGroupRoot(item);
            if (rootItem == null)
            {
                groupList.Add(item);
                return groupList;
            }
            return GetGroupMembersFromCanvas(rootItem);
        }

        //获取组合的图元的每个子对象
        internal List<IGroupable> GetGroupChilds(IGroupable parent)
        {
            var list = _designerCanvas.Children.OfType<IGroupable>();
            var children = list.Where(node => node.ParentID == parent.ID);

            return children.ToList();
        }

        //取得图元列表
        private List<IGroupable> GetGroupMembersFromCanvas(IGroupable parent)
        {
            if (parent == null)
            {
                return null;
            }
            var list = _designerCanvas.Children.OfType<IGroupable>();
            var groupMembers = new List<IGroupable>();
            groupMembers.Add(parent);
            IEnumerable<IGroupable> children = null;
            if (parent != null)
            {
                children = list.Where(node => node.ParentID == parent.ID);
            }
            foreach (var child in children)
            {
                groupMembers.AddRange(GetGroupMembersFromCanvas(child));
            }

            return groupMembers;
        }

        //根据图元取得组合后的图元
        internal IGroupable GetGroupRoot(IGroupable item)
        {
            var list = _designerCanvas.Children.OfType<IGroupable>();
            return GetRoot(list, item);
        }

        //取得组合后的顶层图元
        private IGroupable GetRoot(IEnumerable<IGroupable> list, IGroupable node)
        {
            if (node == null || node.ParentID == Guid.Empty)
            {
                return node;
            }
            foreach (IGroupable item in list)
            {
                if (item.ID == node.ParentID)
                {
                    return GetRoot(list, item);
                }
            }
            return null;
        }
    }
}