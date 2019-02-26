using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Common.Package
{
    /// <summary>
    /// 定义依赖属性的唯一标识
    /// </summary>
    class PropertyGuid
    {
        private System.Windows.DependencyObject _dObj;
        private Guid _guid;

        public DependencyObject DObj
        {
            get
            {
                return _dObj;
            }

            set
            {
                _dObj = value;
            }
        }

        public Guid Guid
        {
            get
            {
                return _guid;
            }

            set
            {
                _guid = value;
            }
        }
    }
}
