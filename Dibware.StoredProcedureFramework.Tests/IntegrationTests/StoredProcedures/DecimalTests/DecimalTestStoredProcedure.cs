﻿using System.Collections.Generic;
using Dibware.StoredProcedureFramework.Base;
using Dibware.StoredProcedureFramework.StoredProcedureAttributes;

namespace Dibware.StoredProcedureFramework.Tests.IntegrationTests.StoredProcedures.DecimalTests
{
    [Schema("app")]
    [Name("DecimalTest")]
    internal class DecimalTestStoredProcedure
        : StoredProcedureBase<List<DecimalTestExtendedReturnType>, NullStoredProcedureParameters>
    {
        public DecimalTestStoredProcedure()
            : base(new NullStoredProcedureParameters())
        {
        }
    }

    //[Schema("app")]
    //[Name("DecimalTest")]
    //internal class DecimalTestStoredProcedure
    //    : StoredProcedureBase<DecimalTestExtendedResultSet, NullStoredProcedureParameters>
    //{
    //    public DecimalTestStoredProcedure()
    //        : base(new NullStoredProcedureParameters())
    //    {
    //    }
    //}

    //internal class DecimalTestExtendedResultSet
    //{
    //    public List<DecimalTestExtendedReturnType> RecordSet1 { get; set; }

    //        public DecimalTestExtendedResultSet()
    //    {
    //        RecordSet1 = new List<DecimalTestExtendedReturnType>();
    //    }
    //}
}
