using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
//using Wss.Common;

namespace Wss.FoundationCore.Attributes.Internal
{
    internal class ModelPropertyImpl : DesignPropertyImpl, IModelProperty
    {
        public string FieldName { get; set; }

        //public UnitMark UnitMark { get; set; }

        public TargetGroup TargetGroup { get; set; }

        public override void CopyFrom(object obj)
        {
            var dp = (IModelProperty) obj;
            base.CopyFrom(obj);
            TargetGroup = dp.TargetGroup;
            FieldName = dp.FieldName;
            //UnitMark = dp.UnitMark;
        }

        public static ModelPropertyImpl Create(Type ownerType, PropertyDescriptor pd, string displayName,
            string fieldName, string bindPath,
            TargetGroup targetGroup)
        {
            return new ModelPropertyImpl
            {
                OwnerType = ownerType,
                Descriptor = pd,
                DisplayName = displayName,
                TargetGroup = targetGroup,
                FieldName = fieldName,
                Binding = !string.IsNullOrWhiteSpace(bindPath)
                    ? new Binding
                    {
                        Path = new PropertyPath(bindPath),
                        NotifyOnSourceUpdated = true,
                        Mode = BindingMode.TwoWay
                    }
                    : null
            };
        }
    }
}