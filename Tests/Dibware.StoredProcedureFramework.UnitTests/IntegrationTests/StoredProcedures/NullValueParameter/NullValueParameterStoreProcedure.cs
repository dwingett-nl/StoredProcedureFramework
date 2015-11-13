﻿using System.Collections.Generic;
using Dibware.StoredProcedureFramework.Base;
using Dibware.StoredProcedureFramework.StoredProcedureAttributes;

namespace Dibware.StoredProcedureFramework.Tests.IntegrationTests.StoredProcedures.NullValueParameter
{
    [Schema("app")]
    [Name("NullValueParameterAndResult")]
    internal class NullValueParameterStoreProcedure
        : StoredProcedureBase<List<NullValueParameterNullableReturnType>, NullValueParameterParameters>
    {
        public NullValueParameterStoreProcedure(NullValueParameterParameters parameters)
            : base(parameters)
        {

        }
    }

    //[Schema("app")]
    //[Name("NullValueParameterAndResult")]
    //internal class NullValueParameterStoreProcedure
    //    : StoredProcedureBase<NullValueParameterStoreProcedureResultSet, NullValueParameterParameters>
    //{
    //    public NullValueParameterStoreProcedure(NullValueParameterParameters parameters)
    //        : base(parameters)
    //    {

    //    }
    //}

    //internal class NullValueParameterStoreProcedureResultSet
    //{
    //    public List<NullValueParameterNullableReturnType> RecordSet1 { get; set; }

    //    public NullValueParameterStoreProcedureResultSet()
    //    {
    //        RecordSet1 = new List<NullValueParameterNullableReturnType>();
    //    }
    //}

    [Schema("app")]
    [Name("NullValueParameterAndResult")]
    internal class NullValueParameterStoreProcedure2
        : StoredProcedureBase<List<NullValueParameterNonNullableReturnType>, NullValueParameterParameters>
    {
        public NullValueParameterStoreProcedure2(NullValueParameterParameters parameters)
            : base(parameters)
        {

        }
    }

    //[Schema("app")]
    //[Name("NullValueParameterAndResult")]
    //internal class NullValueParameterStoreProcedure2
    //    : StoredProcedureBase<NullValueParameterStoreProcedure2ResultSet, NullValueParameterParameters>
    //{
    //    public NullValueParameterStoreProcedure2(NullValueParameterParameters parameters)
    //        : base(parameters)
    //    {

    //    }
    //}

    //internal class NullValueParameterStoreProcedure2ResultSet
    //{
    //    public List<NullValueParameterNonNullableReturnType> RecordSet1 { get; set; }

    //    public NullValueParameterStoreProcedure2ResultSet()
    //    {
    //        RecordSet1 = new List<NullValueParameterNonNullableReturnType>();
    //    }
    //}
}