using System.Windows;

namespace Wss.FoundationCore.Attributes
{
    public interface IDesignerChildable
    {
        DependencyObject DesignerChild { get; set; }
    }
}