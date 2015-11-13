﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dibware.StoredProcedureFramework.Contracts;
using Dibware.StoredProcedureFramework.Exceptions;
using Dibware.StoredProcedureFramework.Helpers;
using Dibware.StoredProcedureFramework.Resources;
using Dibware.StoredProcedureFramework.Validators;

namespace Dibware.StoredProcedureFramework.Extensions
{
    /// <summary>
    /// Extension methods for the DbConnection object
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="connection">The connection we are extending.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="procedureParameters">The procedure parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        public static DbCommand CreateStoredProcedureCommand(
            this DbConnection connection,
            string procedureName,
            IEnumerable<SqlParameter> procedureParameters,
            int? commandTimeout = null,
            SqlTransaction transaction = null)
        {
            DbCommand command = connection.CreateCommand();

            // Command to execute is our stored storedProcedure
            command.Transaction = transaction;
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;

            // Assign command timeout value, if one was provided
            if (commandTimeout.HasValue) command.CommandTimeout = commandTimeout.Value;

            // Transfer any parameters to the command
            if (procedureParameters != null)
            {
                LoadCommandParameters(procedureParameters, command);
            }

            return command;
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static TResultSetType ExecuteStoredProcedure<TResultSetType, TParameterType>(
            this DbConnection connection,
            IStoredProcedure<TResultSetType, TParameterType> storedProcedure,
            int? commandTimeout = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            SqlTransaction transaction = null)
            where TResultSetType : class, new()
            where TParameterType : class
        {
            // Validate arguments
            if (storedProcedure == null) throw new ArgumentNullException("storedProcedure");

            // Ensure the procedure is fully constructed
            storedProcedure.EnsureFullyConstructed();

            string procedureName = storedProcedure.GetTwoPartName();

            // Prepare the parameters if any exist
            IEnumerable<SqlParameter> procedureParameters =
                (storedProcedure.Parameters is NullStoredProcedureParameters) ?
                null :
                BuildProcedureParameters(storedProcedure);

            // Populate results using an overload
            var results = ExecuteStoredProcedure<TResultSetType>(
                connection,
                procedureName,
                procedureParameters,
                commandTimeout,
                commandBehavior,
                transaction);

            // Process any output parameters
            ProcessOutputParms(procedureParameters, storedProcedure);

            // return the results
            return results;
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static TResultSetType ExecuteStoredProcedure<TResultSetType>(
            this DbConnection connection,
            string procedureName,
            IEnumerable<SqlParameter> procedureParameters = null,
            int? commandTimeout = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            SqlTransaction transaction = null) 
            where TResultSetType : class, new()
        {
            // Validate arguments
            if (procedureName == null) throw new ArgumentNullException("procedureName");
            if (procedureName == string.Empty) throw new ArgumentOutOfRangeException("procedureName");
            
            // A flag to track whether we opened the connection or not
            bool connectionWasOpen = (connection.State == ConnectionState.Open);

            try
            {
                TResultSetType results; // Create a result object

                if (!connectionWasOpen) connection.Open(); // Open a closed connection

                // Create a command to execute the stored storedProcedure...
                using (DbCommand command = connection.CreateStoredProcedureCommand(
                    procedureName,
                    procedureParameters,
                    commandTimeout,
                    transaction))
                {
                    // ...execute the command to get results
                    results = ExecuteCommand<TResultSetType>(commandBehavior, command);
                }

                // Return the results
                return results;
            }
            catch (Exception ex)
            {
                // We want to add a slightly more informative message to the
                // exception before rethrowing it
                var message = string.Format(
                    ExceptionMessages.ErrorReadingStoredProcedure,
                    procedureName,
                    ex.Message);

                Type exceptionType = ex.GetType();

                // Option 1: Edit the actual message field insode the exception and rethrow
                var fieldInfo = exceptionType.GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo != null) fieldInfo.SetValue(ex, message);
                throw;

                // Option 2: Create a new insatnce of the same type as the caught
                // exception with a new message, and throw that
                //throw (Exception)Activator.CreateInstance(exceptionType, message, ex);
            }
            finally
            {
                if (!connectionWasOpen) connection.Close();  // Close connection if it arrived closed
            }
        }

        private static TResultSetType ExecuteCommand<TResultSetType>(
            CommandBehavior commandBehavior, 
            DbCommand command)
            where TResultSetType : class, new()
        {
            var procedureHasNoReturnType =
                (typeof (TResultSetType) == typeof (NullStoredProcedureResult));

            var results = procedureHasNoReturnType 
                ? ExecuteCommandWithNoReturnType<TResultSetType>(command) 
                : ExecuteCommandWithResultSet<TResultSetType>(commandBehavior, command);

            return results;
        }

        private static TResultSetType ExecuteCommandWithResultSet<TResultSetType>(
            CommandBehavior commandBehavior,
            DbCommand command) 
            where TResultSetType : class, new()
        {
            TResultSetType resultSet = new TResultSetType();
            Type resultSetType = typeof (TResultSetType);
            
            string resultSetTypeName = resultSetType.Name;

            // Populate a DataReder by calling the command
            using (DbDataReader reader = command.ExecuteReader(commandBehavior))
            {
                bool isSingleRecordSet = ImplementsICollection(resultSetType);
                if (isSingleRecordSet)
                {
                    IList recordSetDtoList = (IList)new TResultSetType();
                    ReadRecordSet(reader, recordSetDtoList);
                    resultSet = (TResultSetType)recordSetDtoList;
                }
                else // Multiple recordsets
                {
                    var recordSetIndex = 0;

                    // Get properties to save for the current destination type
                    PropertyInfo[] resultSetTypePropertyInfos = resultSetType.GetMappedProperties();

                    // Iterate through each result set and read records into DTO list
                    bool readerContainsAnotherResult;
                    do
                    {
                        var recordSetPropertyName = resultSetTypePropertyInfos[recordSetIndex].Name;
                        IList recordSetDtoList = GetRecordSetDtoList(resultSetType, recordSetPropertyName, resultSet);
                        EnsureRecorsetListIsInstantiated(recordSetDtoList, resultSetTypeName, recordSetPropertyName);
                        ReadRecordSet(reader, recordSetDtoList);

                        recordSetIndex++;
                        readerContainsAnotherResult = reader.NextResult();
                    } 
                    while (readerContainsAnotherResult);
                }

                // Close the reader
                reader.Close();
            }
            return resultSet;
        }

        private static bool ImplementsICollection(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType)
                {
                    if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        // if needed, you can also return the type used as generic argument
                        return true;
                    }
                }
            }
            return false;
        }


        private static TResultSetType ExecuteCommandWithNoReturnType<TResultSetType>(DbCommand command)
            where TResultSetType : class, new()
        {
            command.ExecuteNonQuery();
            return null;
        }

        private static void ReadRecordSet(DbDataReader reader, IList recordSetDtoList)
        {
            // Recordset row 
            Type dtoListItemType = recordSetDtoList.GetType().GetGenericArguments()[0];
            PropertyInfo[] dtoListItemTypePropertyInfo = dtoListItemType.GetMappedProperties();

            // Process the result set line by line
            while (reader.Read())
            {
                AddRecordToResults(dtoListItemType, recordSetDtoList, reader, dtoListItemTypePropertyInfo);
            }
        }

        private static IList GetRecordSetDtoList<TResultSetType>(
            Type resultSetType, 
            string recordSetPropertyName,
            TResultSetType resultSet) 
            where TResultSetType : class, new()
        {
            PropertyInfo recordSetPropertyInfo = resultSetType.GetProperty(recordSetPropertyName);
            IList recordSetDtoList = (IList) recordSetPropertyInfo.GetValue(resultSet);
            return recordSetDtoList;
        }

        private static void EnsureRecorsetListIsInstantiated(
            IList dtoList,
            string resultSetTypeName,
            string listPropertyName)
        {

            if (dtoList == null)
            {
                string errorMessage = string.Format(
                   ExceptionMessages.RecordSetListNotInstatiated,
                   resultSetTypeName,
                   listPropertyName);

                throw new NullReferenceException(errorMessage);
            }
        }

        /// <summary>
        /// Adds the record to results.
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="results">The results.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="dtoListItemTypePropertyInfos">The DTO ListItem Type Property Information.</param>
        private static void AddRecordToResults(
            Type outputType,
            IList results,
            DbDataReader reader,
            PropertyInfo[] dtoListItemTypePropertyInfos)
        {
            // Providing we have a constructor...
            var constructorInfo = (outputType).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null) return;

            
            //TODO: Investigate FastActivator
            // Even at 2M records there is still neglidgable difference between
            // standard Activator and FastActivator
            //var item = FastActivator.CreateInstance(outputType);
            //var item = FastActivator2.CreateInstance(outputType);

            // ...create an object to hold this result
            var item = Activator.CreateInstance(outputType);
            //if (item == null) return;

            // Providing we created an object
            // Copy data elements by parameter name from result to destination object
            reader.ReadRecord(item, dtoListItemTypePropertyInfos);

            // add newly populated item to our output list
            results.Add(item);
        }

        private static ICollection<SqlParameter> BuildProcedureParameters<TReturnType, TParameterType>(
            IStoredProcedure<TReturnType, TParameterType> procedure)
            where TReturnType : class
            where TParameterType : class
        {
            // create mapped properties
            //var mappedProperties = typeof(TParameterType).GetMappedProperties();
            var mappedProperties = procedure.Parameters.GetType().GetMappedProperties();

            // Create parameters from mapped properties
            //var sqlParameters = mappedProperties.ToSqlParameters();
            var sqlParameters = SqlParameterHelper.CreateSqlParametersFromPropertyInfoArray(mappedProperties);

            // Populate parameters from storedProcedure parameters
            PopulateSqlParametersFromProperties(sqlParameters, mappedProperties, procedure);

            // Return parameters
            return sqlParameters;
        }

        private static void ProcessOutputParms<TReturnType, TParameterType>(IEnumerable<SqlParameter> procedureParameters,
            IStoredProcedure<TReturnType, TParameterType> storedProcedure)
            where TReturnType : class
            where TParameterType : class
        {
            // Validate arguments
            if (storedProcedure == null) throw new ArgumentNullException("storedProcedure");

            // If the procdeure has no parameters then bug out of this method
            if (storedProcedure.Parameters is NullStoredProcedureParameters ||
                procedureParameters == null) return;

            // create mapped properties
            var mappedProperties = typeof(TParameterType).GetMappedProperties();

            // we want to write data back to properties for every non-input only parameter
            foreach (SqlParameter sqlParameter in procedureParameters
                .Where(p => p.Direction != ParameterDirection.Input)
                .Select(p => p))
            {
                // get the property name mapped to this parameter
                //String propertyName = MappedParams.Where(p => p.Key == sqlParameter.ParameterName).Select(p => p.Value).First();
                String propertyName = sqlParameter.ParameterName;

                // extract the matchingproperty and set its value
                PropertyInfo propertyInfo = mappedProperties.FirstOrDefault(p => p.Name == propertyName);
                if (propertyInfo != null) propertyInfo.SetValue(storedProcedure.Parameters, sqlParameter.Value, null);
            }
        }

        /// <summary>
        /// Loads the command parameters.
        /// </summary>
        /// <param name="sqlParameters">The SQLParameters to load command with.</param>
        /// <param name="command">The command.</param>
        private static void LoadCommandParameters(IEnumerable<SqlParameter> sqlParameters, DbCommand command)
        {
            // Clear any existing command parameters
            if (command.Parameters.Count > 0) command.Parameters.Clear();

            // add the specified parameters
            foreach (SqlParameter p in sqlParameters)
            {
                command.Parameters.Add(p);
            }
        }

        private static void PopulateSqlParametersFromProperties<TReturnType, TParameterType>(
            ICollection<SqlParameter> sqlParameters,
            PropertyInfo[] mappedProperties,
            IStoredProcedure<TReturnType, TParameterType> procedure)
            where TReturnType : class
            where TParameterType : class
        {
            // Get all input type parameters
            foreach (SqlParameter sqlParameter in sqlParameters
                .Where(p => p.Direction == ParameterDirection.Input)
                .Select(p => p))
            {
                // Get the property name mapped to this parameter
                String propertyName = sqlParameter.ParameterName;

                // Extract the matching property...
                PropertyInfo propertyInfo = mappedProperties.FirstOrDefault(p => p.Name == propertyName);
                if (propertyInfo == null) throw new NullReferenceException("Mapped property not found");

                // Use the PropertyInfo to get the value from the parameters,
                // then validate the value and if validation passes, set it 
                var value = propertyInfo.GetValue(procedure.Parameters);
                ValidateParameterValueIsInRange(sqlParameter, value);
                if (value == null) value = DBNull.Value; /* Handle null values */
                if (sqlParameter.SqlDbType == SqlDbType.Structured)
                {
                    sqlParameter.Value = TableValuedParameterHelper.TableValuedParameter((IList) value);
                }
                else
                {
                    sqlParameter.Value = value;
                }
            }
        }

        private static void ValidateParameterValueIsInRange(SqlParameter sqlParameter, object value)
        {
            // For parameters that have precision and scale we need to validate the value
            if (sqlParameter.RequiresPrecisionAndScaleValidation())
            {
                ValidateDecimal(sqlParameter, value);
            }

            if (sqlParameter.RequiresLengthValidation())
            {
                ValidateString(sqlParameter, value);
            }
        }

        private static void ValidateDecimal(SqlParameter sqlParameter, object value)
        {
            if (value is decimal)
            {
                var validator = new SqlParameterDecimalValueValidator();
                validator.Validate((decimal)value, sqlParameter);
            }
            else
            {
                throw new SqlParameterInvalidDataTypeException(
                    sqlParameter.ParameterName,
                    typeof(decimal), value.GetType());
            }
        }

        private static void ValidateString(SqlParameter sqlParameter, object value)
        {
            // Check if the value type is valid type... 
            if (value is string)
            {
                // ... and validate it if it is
                var validator = new SqlParameterStringValueValidator();
                validator.Validate((string)value, sqlParameter);
            }
            else if (value is char[])
            {
                // ... and validate it if it is
                var validator = new SqlParameterStringValueValidator();
                validator.Validate(new string((char[])value), sqlParameter);
            }
            else
            {
                // Throw a wobbler!
                throw new SqlParameterInvalidDataTypeException(
                    sqlParameter.ParameterName,
                    typeof(string), value.GetType());
            }
        }
    }
}