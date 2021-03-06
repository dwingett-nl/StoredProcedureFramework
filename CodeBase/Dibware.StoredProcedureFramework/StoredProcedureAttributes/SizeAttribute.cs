﻿using System;

namespace Dibware.StoredProcedureFramework.StoredProcedureAttributes
{
    /// <summary>
    /// Defines the number of characters allowed for character-based data types. 
    /// Should be used on output and return code parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SizeAttribute : Attribute
    {
        public Int32 Value { get; private set; }

        public SizeAttribute(Int32 value)
        {
            Value = value;
        }
    }
}