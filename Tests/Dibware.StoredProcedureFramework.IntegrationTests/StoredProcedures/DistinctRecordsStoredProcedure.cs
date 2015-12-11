﻿using Dibware.StoredProcedureFramework.Base;
using System.Collections.Generic;

namespace Dibware.StoredProcedureFramework.IntegrationTests.StoredProcedures
{
    internal class DistinctRecordsStoredProcedure
        : NoParametersStoredProcedureBase<List<DistinctRecordsStoredProcedure.Return>>
    {
        internal class Return
        {
            public int Value1 { get; set; }
            public bool Value2 { get; set; }
            public string Value3 { get; set; }
        }
    }
}