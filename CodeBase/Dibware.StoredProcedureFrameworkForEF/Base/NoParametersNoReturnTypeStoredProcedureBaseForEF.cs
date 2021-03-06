﻿using System.Data.Entity;
using Dibware.StoredProcedureFramework;

namespace Dibware.StoredProcedureFrameworkForEF.Base
{
    /// <summary>
    /// Represents the base class that a stored procedures without parameters
    /// or a return type should inherit from if used in conjunction with Entity Framework.
    /// </summary>
    public abstract class NoParametersNoReturnTypeStoredProcedureBaseForEf
        : StoredProcedureBaseForEf<NullStoredProcedureResult, NullStoredProcedureParameters>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NoParametersNoReturnTypeStoredProcedureBaseForEf" />
        /// class with parameters. This is the minimum requirement for constructing
        /// a stored procedure.
        /// </summary>
        /// <param name="context">The context.</param>
        protected NoParametersNoReturnTypeStoredProcedureBaseForEf(DbContext context)
            : base(context, new NullStoredProcedureParameters())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoParametersNoReturnTypeStoredProcedureBaseForEf"/> 
        /// class with parameters and procedure name.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        protected NoParametersNoReturnTypeStoredProcedureBaseForEf(DbContext context,
            string procedureName)
            : base(context, procedureName, new NullStoredProcedureParameters())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoParametersNoReturnTypeStoredProcedureBaseForEf"/> 
        /// class with parameters, schema name and procedure name
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        protected NoParametersNoReturnTypeStoredProcedureBaseForEf(DbContext context,
            string schemaName, string procedureName)
            : base(context, schemaName, procedureName, new NullStoredProcedureParameters())
        {
        }

        #endregion
    }
}
