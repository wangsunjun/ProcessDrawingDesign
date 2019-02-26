using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wss.FoundationCore.Controls
{
    public interface IPropertyValueChanged
    {
        bool PropertyChanged(object value, string propertyName);
    }
}
