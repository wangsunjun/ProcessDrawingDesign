﻿using System;

namespace Wss.FoundationCore.Controls
{
    public interface IGroupable
    {
        Guid ID { get; }
        Guid ParentID { get; set; }
        bool IsGroup { get; set; }
    }
}