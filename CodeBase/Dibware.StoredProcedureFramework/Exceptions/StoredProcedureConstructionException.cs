﻿using System;

namespace Dibware.StoredProcedureFramework.Exceptions
{
    public sealed class StoredProcedureConstructionException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureConstructionException"/> class.
        /// </summary>
        public StoredProcedureConstructionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureConstructionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StoredProcedureConstructionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureConstructionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null 
        /// reference (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public StoredProcedureConstructionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}