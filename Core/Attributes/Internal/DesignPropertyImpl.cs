using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Wss.FoundationCore.Attributes.Internal
{
    internal class DesignPropertyImpl : SerializePropertyImpl, IDesignProperty
    {
        public string DisplayName { get; set; }
        public string Guid { get; set; }
        public bool ReadOnly { get; set; }
        public Type Editer { get; set; }
        public Binding Binding { get; set; }
        /// <summary>
        /// ÒÀÀµ°ó¶¨×ªÒÆ
        /// </summary>
        /// <param name="obj"></param>
        public override void CopyFrom(object obj)
        {
            var dp = (IDesignProperty) obj;
            base.CopyFrom(obj);
            ReadOnly = dp.ReadOnly;
            Editer = dp.Editer;
            DisplayName = dp.DisplayName;
            Guid = dp.Guid;
            if (dp.Binding == null)
            {
                return;
            }
            Binding = new Binding
            {
                Path = dp.Binding.Path,
                Source = dp.Binding.Source,
                Mode = dp.Binding.Mode
            };
        }
        public static DesignPropertyImpl Create(Type ownerType, PropertyDescriptor pd, string displayName, string path, Boolean readOnly)//,string guid
        {
            return new DesignPropertyImpl
            {
                OwnerType = ownerType,
                Descriptor = pd,
                DisplayName = displayName,
                ReadOnly = readOnly,
                //Guid=guid,
                Binding = path != null
                    ? new Binding
                    {
                        Path = new PropertyPath(path),
                        Mode = readOnly ? BindingMode.OneWay : BindingMode.TwoWay
                    }
                    : null
            };
        }
    }
}