﻿using Dibware.StoredProcedureFramework.Extensions;
using Dibware.StoredProcedureFramework.Helpers.Contracts;
using Dibware.StoredProcedureFramework.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Reflection;

namespace Dibware.StoredProcedureFramework.Helpers.Base
{
    internal abstract class SqlProgrammabilityObjectExecuterBase<TResultSetType>
        : IDisposable
        where TResultSetType : new()
    {
        #region Fields

        private readonly IDbConnection _connection;
        private readonly Type _resultSetType;
        private readonly string _programmabilityObjectName;
        private bool _connectionAlreadyOpen;
        private int? _commandTimeoutOverride;
        private CommandBehavior _commandBehavior;
        private IEnumerable<SqlParameter> _procedureParameters;
        private SqlTransaction _transaction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgrammabilityObjectExecuterBase{TResultSetType}"/> class.
        /// </summary>
        /// <param name="connection">
        /// The databse connection to execute the programmability object against.
        /// </param>
        /// <param name="programmabilityObjectName">Name of the programmability object.</param>
        protected SqlProgrammabilityObjectExecuterBase(IDbConnection connection,
            string programmabilityObjectName)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrWhiteSpace(programmabilityObjectName)) throw new ArgumentNullException("programmabilityObjectName");

            _connection = connection;
            _programmabilityObjectName = programmabilityObjectName;
            _resultSetType = typeof(TResultSetType);
        }

        #endregion

        #region Dispose and Finalise

        /// <summary>
        /// Gets a value indicating whether this <see cref="SqlProgrammabilityObjectExecuterBase{TResultSetType}"/> 
        /// is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        private bool Disposed { get; set; }

        /// <summary>
        /// Finalizes an instance of the <see cref="SqlProgrammabilityObjectExecuterBase{TResultSetType}"/> class.
        /// </summary>
        ~SqlProgrammabilityObjectExecuterBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    DisposeCommand();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            Disposed = true;
        }

        #endregion

        #region Public Members

        protected IDbCommand Command { get; private set; }

        protected IDbConnection Connection
        {
            get { return _connection; }
        }

        protected string ProgrammabilityObjectName
        {
            get { return _programmabilityObjectName; }
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ObjectDisposedException">
        /// Cannot call Execute when this object is disposed</exception>
        public SqlProgrammabilityObjectExecuterBase<TResultSetType> Execute()
        {
            if (Disposed) throw new ObjectDisposedException("Cannot call Execute when this object is disposed");

            CacheOriginalConnectionState();

            try
            {
                OpenClosedConnection();
                CreateCommand();
                ExecuteCommand();
            }
            catch (Exception ex)
            {
                AddMoreInformativeInformationToExecuteError(ref ex);
                throw;
            }
            finally
            {
                DisposeCommand();
                RestoreOriginalConnectionState();
            }
            return this;
        }

        /// <summary>
        /// Gets the results of the stored procedure call.
        /// </summary>
        /// <value>
        /// The results of the stored procedure call.
        /// </value>
        public TResultSetType Results { get; protected set; }

        protected void WithCommandBehavior(CommandBehavior commandBehavior)
        {
            _commandBehavior = commandBehavior;
        }

        protected void WithParameters(IEnumerable<SqlParameter> procedureParameters)
        {
            if (procedureParameters == null) throw new ArgumentNullException("procedureParameters");

            _procedureParameters = procedureParameters;
        }

        protected void WithCommandTimeoutOverride(int commandTimeoutOverride)
        {
            _commandTimeoutOverride = commandTimeoutOverride;
        }

        protected void WithTransaction(SqlTransaction transaction)
        {
            _transaction = transaction;
        }

        #endregion

        #region Private Members

        private void AddMoreInformativeInformationToExecuteError(ref Exception ex)
        {
            var detailedMessage = string.Format(
                ExceptionMessages.ErrorReadingStoredProcedure,
                _programmabilityObjectName,
                ex.Message);
            Type exceptionType = ex.GetType();
            var fieldInfo = exceptionType.GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo != null) fieldInfo.SetValue(ex, detailedMessage);
        }

        private static string[] CacheDynamicFieldNames(IDataReader reader, int fieldCount)
        {
            string[] dynamicFieldNames = new string[fieldCount];

            for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex += 1)
            {
                dynamicFieldNames[fieldIndex] = reader.GetName(fieldIndex);
            }
            return dynamicFieldNames;
        }

        private void CacheOriginalConnectionState()
        {
            _connectionAlreadyOpen = (_connection.State == ConnectionState.Open);
        }

        protected abstract IDbCommandCreator CreateCommandCreator();

        private void CreateCommand()
        {
            DisposeCommand();

            IDbCommandCreator creator = CreateCommandCreator();

            bool hasCommandTimeoutOverride = _commandTimeoutOverride.HasValue;
            if (hasCommandTimeoutOverride)
            {
                creator.WithCommandTimeout(_commandTimeoutOverride.Value);
            }

            if (HasParameters)
            {
                creator.WithParameters(_procedureParameters);
            }

            if (HasTransaction)
            {
                creator.WithTransaction(_transaction);
            }

            Command = creator
                    .BuildCommand()
                    .Command;
        }

        private void DisposeCommand()
        {
            if (Command != null)
            {
                Command.Dispose();
                Command = null;
            }
        }

        private void EnsureRecorsetListIsInstantiated(
            IList dtoList,
            string listPropertyName)
        {
            if (dtoList != null) return;

            string errorMessage = string.Format(
                ExceptionMessages.RecordSetListNotInstatiated,
                _resultSetType.Name,
                listPropertyName);
            throw new NullReferenceException(errorMessage);
        }

        protected abstract void ExecuteCommand();

        protected void ExecuteCommandWithNoReturnType()
        {
            Command.ExecuteNonQuery();
        }

        protected void ExecuteCommandWithResultSet()
        {
            if (HasSingleRecordSetOnly)
            {
                ExecuteCommandForSingleRecordSet();
            }
            else
            {
                ExecuteCommandForMultipleRecordSets();
            }
        }

        private void ExecuteCommandForMultipleRecordSets()
        {
            Results = new TResultSetType();
            var recordSetIndex = 0;
            var resultSetTypeProperties = _resultSetType.GetMappedProperties();

            using (IDataReader reader = Command.ExecuteReader(_commandBehavior))
            {
                bool readerContainsAnotherResult;
                do
                {
                    var recordSetDtoList = GetRecordSetDtoList(resultSetTypeProperties, recordSetIndex);
                    var recordSetType = recordSetDtoList.GetType();
                    bool isDynamic = recordSetType.IsGenericTypeWithFirstDynamicTypeArgument();
                    if (isDynamic)
                    {
                        ReadDynamicRecordSetFromReader(reader, recordSetDtoList);
                    }
                    else
                    {
                        ReadRecordSetFromReader(reader, recordSetDtoList);
                    }

                    recordSetIndex += 1;
                    readerContainsAnotherResult = reader.NextResult();

                } while (readerContainsAnotherResult);
                reader.Close();
            }
        }

        private void ExecuteCommandForSingleRecordSet()
        {
            var recordSetDtoList = (IList)new TResultSetType();

            using (IDataReader reader = Command.ExecuteReader(_commandBehavior))
            {
                if (_resultSetType.IsGenericTypeWithFirstDynamicTypeArgument())
                {
                    ReadDynamicRecordSetFromReader(reader, recordSetDtoList);
                }
                else
                {
                    ReadRecordSetFromReader(reader, recordSetDtoList);
                }
                reader.Close();
            }

            Results = (TResultSetType)recordSetDtoList;
        }

        private IList GetRecordSetDtoList(PropertyInfo[] resultSetTypePropertyInfos, int recordSetIndex)
        {
            var recordSetPropertyName = resultSetTypePropertyInfos[recordSetIndex].Name;
            var recordSetDtoList = GetRecordSetDtoList(recordSetPropertyName);
            EnsureRecorsetListIsInstantiated(recordSetDtoList, recordSetPropertyName);

            return recordSetDtoList;
        }

        private bool HasSingleRecordSetOnly
        {
            get { return _resultSetType.ImplementsICollectionInterface(); }
        }

        protected static bool HasNoReturnType
        {
            get { return (typeof(TResultSetType) == typeof(NullStoredProcedureResult)); }
        }

        private bool HasParameters
        {
            get { return _procedureParameters != null; }
        }

        private bool HasTransaction
        {
            get { return _transaction != null; }
        }

        private IList GetRecordSetDtoList(string recordSetPropertyName)
        {
            PropertyInfo recordSetPropertyInfo = _resultSetType.GetProperty(recordSetPropertyName);
            IList recordSetDtoList = (IList)recordSetPropertyInfo.GetValue(Results, null);
            return recordSetDtoList;
        }

        private void OpenClosedConnection()
        {
            if (!_connectionAlreadyOpen) _connection.Open();
        }

        private void ReadDynamicRecordSetFromReader(IDataReader reader, IList records)
        {
            Type recordsType = records.GetType();
            if (!recordsType.IsGenericTypeWithFirstDynamicTypeArgument()) throw new InvalidOperationException();

            var dynamicFieldCount = reader.FieldCount;
            var dynamicFieldNames = CacheDynamicFieldNames(reader, dynamicFieldCount);

            while (reader.Read())
            {
                var row = new ExpandoObject() as IDictionary<string, Object>;
                for (int fieldIndex = 0; fieldIndex < dynamicFieldCount; fieldIndex += 1)
                {
                    row.Add(dynamicFieldNames[fieldIndex], reader.GetValue(fieldIndex));
                }
                records.Add(row);
            }
        }

        private void ReadRecordSetFromReader(IDataReader reader, IList records)
        {
            Type recordType = records.GetType().GetGenericArguments()[0];
            var mapper = new DateReaderRecordToObjectMapper(reader, recordType);
            while (reader.Read())
            {
                mapper.PopulateMappedTargetFromReaderRecord();
                records.Add(mapper.MappedTarget);
            }
        }

        private void RestoreOriginalConnectionState()
        {
            if (!_connectionAlreadyOpen) _connection.Close();
        }

        #endregion
    }
}