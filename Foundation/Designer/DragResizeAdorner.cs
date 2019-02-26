using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Wss.Foundation.Controls;
using Wss.Foundation.Designer.Resources;

namespace Wss.Foundation.Designer
{
    public class DragResizeAdorner : Adorner
    {
        private readonly DesignerCanvas _designerCanvas;
        private AyRadioCheckedConverter converter = new AyRadioCheckedConverter();
        public readonly DragAdornerThumb DragAdornerThumb;
        private List<DesignerContainer> hitResultsList;
        public readonly ResizeAdornerThumb ResizeAdornerThumb;
        public DragResizeAdorner(DesignerCanvas designerCanvas) : base(designerCanvas)
        {
            _designerCanvas = designerCanvas;
            ResizeAdornerThumb = new ResizeAdornerThumb();
            ResizeAdornerThumb.DataContext = _designerCanvas;
            ResizeAdornerThumb.ThumbUpdated+= (s, e) =>
            {
                UpdateAdorner();
            };
            DragAdornerThumb = new DragAdornerThumb();
            DragAdornerThumb.ThumbUpdated += (s, e) =>
            {
                UpdateAdorner();
            };
            DragAdornerThumb.DataContext = _designerCanvas;
            AddVisualChild(ResizeAdornerThumb);
            AddVisualChild(DragAdornerThumb);
            
        }
        public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
        {
            if (o.GetType() == typeof(DesignerContainer))// Test for the object value you want to filter.
            {
                hitResultsList.Add(o as DesignerContainer);
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;// Visual object and descendants are NOT part of hit test results enumeration.
            }
            return HitTestFilterBehavior.Continue;// Visual object is part of hit test results enumeration.
        }
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)// Return the result of the hit test to the callback.
        {
            return HitTestResultBehavior.Continue;
        }
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (DragAdornerThumb.IsDragging)
            {
                return;
            }
            if (hitResultsList == null)
            {
                hitResultsList = new List<DesignerContainer>();
            }
            hitResultsList.Clear();
            // Retrieve the coordinate of the mouse position.
            Point pt = e.GetPosition(_designerCanvas);

            // Clear the contents of the list used for hit test results.
            // Set up a callback to receive the hit test result enumeration.
            VisualTreeHelper.HitTest(_designerCanvas,
                               MyHitTestFilter,
                               MyHitTestResult,
                               new PointHitTestParameters(pt));
            // Perform actions on the hit test results list.
            if (hitResultsList.Count > 0)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
                {
                    foreach (var item in hitResultsList)
                    {
                        _designerCanvas.SelectContainer(item);
                    }
                }
                else
                {
                    
                    
                }
            }
        }
        #region No use
        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 1:
                    { return ResizeAdornerThumb; }
            }
            return DragAdornerThumb;
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return 2;
            }
        }
        #endregion
        public void UpdateAdorner()
        {
            if (_designerCanvas.SelectionService.SelectedDesignerContainer.Count > 0)
            {
                this.Visibility = Visibility.Visible;
                var items = _designerCanvas.SelectionService.SelectedDesignerContainer.OfType<FrameworkElement>().ToList();
                var rect = ResizeThumb.GetBoundingRectangle(items);
                Console.WriteLine("rect1-" + rect);
                ResizeAdornerThumb.Arrange(rect);
                DragAdornerThumb.Arrange(rect);
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            var items = _designerCanvas.SelectionService.SelectedDesignerContainer.OfType<FrameworkElement>().ToList();
            var rect = ResizeThumb.GetBoundingRectangle(items);
            ResizeAdornerThumb.Arrange(rect);
            DragAdornerThumb.Arrange(rect);
            Console.WriteLine("rect2-" + rect);
          return rect.Size;
        }
    }
}