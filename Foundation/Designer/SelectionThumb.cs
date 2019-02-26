using System;
using System.Windows.Controls.Primitives;

namespace Wss.Foundation.Designer
{
    public abstract class SelectionThumb: Thumb
    {
        protected DesignerCanvas Designer
        {
            get { return DataContext as DesignerCanvas; }
        }
        protected SelectionThumb()
        {
            DragStarted += SelectionThumb_DragStarted;
            DragDelta += SelectionThumb_DragDelta;
            DragCompleted += SelectionThumb_DragCompleted;
        }
        private void SelectionThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            OnDragCompleted(e);
            OnThumbUpdated();
        }
        protected abstract void OnDragCompleted(DragCompletedEventArgs e);
        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            OnDragDelta(e);
            OnThumbUpdated();
        }
        protected abstract void OnDragDelta(DragDeltaEventArgs e);
        private void SelectionThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            OnDragStarted(e);
        }
        protected abstract void OnDragStarted(DragStartedEventArgs e);
        protected void OnThumbUpdated()
        {
            var handle = ThumbUpdated;
            if (handle != null)
            {
                handle(this, new EventArgs());
            }
        }
        public event EventHandler ThumbUpdated;
    }
}