﻿using System.Collections.Generic;
using System.Data;
using Dibware.StoredProcedureFramework.Base;
using Dibware.StoredProcedureFramework.StoredProcedureAttributes;

namespace Dibware.StoredProcedureFramework.Tests.Examples.StoredProcedures
{
    /// <summary>
    /// Represents a "normal" stored procedure which has parameters and returns
    /// a single result set
    /// </summary>
    internal class NormalStoredProcedure
        : StoredProcedureBase<List<NormalStoredProcedureRecordSetReturnType>, NormalStoredProcedureParameters>
    {
        public NormalStoredProcedure(NormalStoredProcedureParameters parameters)
            : base(parameters)
        {
        }
    }

    internal class NormalStoredProcedureParameters
    {
        [DbType(SqlDbType.Int)]
        public int Id { get; set; }
    }

    internal class NormalStoredProcedureRecordSetReturnType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
