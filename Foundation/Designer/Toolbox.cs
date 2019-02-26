using System.Windows;
using System.Windows.Controls;

namespace Wss.Foundation.Designer
{
    public class Toolbox : ItemsControl
    {
        private Size itemSize = new Size(50, 50);
        public Size ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }

        protected override DependencyObject GetContainerForItemOverride()// Creates or identifies the element that is used to display the given Item.
        {
            return new ToolboxViewItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item)// Determines if the specified Item is (or is eligible to be) its own container.     
        {
            // return true;
            return (item is ToolboxViewItem);
        }
    }
}