﻿using System;

namespace Dibware.StoredProcedureFramework.Tests.Integration_Tests.StoredProcedures.AllCommonDataTypes
{
    internal class AllCommonDataTypesParameters
    {
        public Int64 BigInt { get; set; }
        public Byte[] Binary { get; set; }
        public Boolean Bit { get; set; }
        public Char[] Char { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime DateTime2 { get; set; }
        public Decimal Decimal { get; set; }
        public Double Float { get; set; }
        public Byte[] Image { get; set; }
        public Int32 Int { get; set; }
        public Decimal Money { get; set; }
        public String NChar { get; set; }
        public String NText { get; set; }
        public Decimal Numeric { get; set; }
        public String NVarchar { get; set; }
        public Single Real { get; set; }
        public DateTime SmallDateTime { get; set; }
        public Int16 Smallint { get; set; }
        public Decimal Smallmoney { get; set; }
        public String Text { get; set; }
        public TimeSpan Time { get; set; }
        public Byte[] Timestamp { get; set; }
        public Byte TinyInt { get; set; }
        public Guid UniqueIdentifier { get; set; }
        public Byte[] VarBinary { get; set; }
        public String VarChar { get; set; }
        public String Xml { get; set; }
    }
}