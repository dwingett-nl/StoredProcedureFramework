﻿using System.Collections.Generic;
using System.Data;
using Dibware.StoredProcedureFramework.Base;
using Dibware.StoredProcedureFramework.StoredProcedureAttributes;

namespace Dibware.StoredProcedureFramework.Tests.IntegrationTests.StoredProcedures
{
    internal class NotInstantiatedRecordSetStoredProcedure
        : StoredProcedureBase<
            List<NotInstantiatedRecordSetStoredProcedureReturnType>,
            NullStoredProcedureParameters>
    {
        public NotInstantiatedRecordSetStoredProcedure(NullStoredProcedureParameters parameters)
            : base(parameters)
        {
        }
    }

    //internal class NotInstantiatedRecordSetStoredProcedure
    //    : StoredProcedureBase<
    //        NotInstantiatedRecordSetStoredProcedureResultSet,
    //        NullStoredProcedureParameters>
    //{
    //    public NotInstantiatedRecordSetStoredProcedure(NullStoredProcedureParameters parameters)
    //        : base(parameters)
    //    {
    //    }
    //}

    //internal class NotInstantiatedRecordSetStoredProcedureResultSet
    //{
    //    public List<NotInstantiatedRecordSetStoredProcedureReturnType> RecordSet1 { get; set; }
    //}

    internal class NotInstantiatedRecordSetStoredProcedureReturnType
    {
        [ParameterDbType(SqlDbType.Int)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
