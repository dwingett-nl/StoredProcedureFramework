﻿using Dibware.StoredProcedureFramework.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using Dibware.StoredProcedureFramework.Tests.IntegrationTests.StoredProcedures.VolumnAndPerformance;

namespace Dibware.StoredProcedureFramework.Tests.VolumeAndPerformance
{
    [TestClass]
    public class PerformanceTest
    {
        [TestMethod]
        [Ignore] /* long running so switch in  / out as required */
        public void HowLongToSelect250KRows()
        {
            /* Note it takes 3 minutes to INSERT 250k of these rows in sql server */
            /* Note it takes 3 seconds to SELECT 250k of these rows in sql server */

            // ARRANGE
            const int expectedNumberOfRecords = 250000;
            const int commandTimeOut = 600;
            bool truncateData = false;
            bool dataIsPresent = true;
            var truncateTableProcedure = new VolumeAndPerformanceTruncateStoredProcedure();
            var insertTableProcedureParameters = new VolumeAndPerformanceInsertNRecordsStoredProcedureParameters
            {
                NumberOfRecords = expectedNumberOfRecords
            };
            var insertTableProcedure = new VolumeAndPerformanceInsertNRecordsStoredProcedure(insertTableProcedureParameters);
            var getAllStoredProcedure = new VolumeAndPerformanceGetAllStoredProcedure();
            var connectionString = ConfigurationManager.ConnectionStrings["IntegrationTestConnection"].ConnectionString;
            //VolumeAndPerformanceGetAllStoredProcedureResultSet resultSet;
            List<VolumeAndPerformanceGetAllStoredProcedureReturnType> results;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan elapsed;

            // ACT
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (truncateData) connection.ExecuteStoredProcedure(truncateTableProcedure);
                if (!dataIsPresent) connection.ExecuteStoredProcedure(insertTableProcedure, commandTimeout: commandTimeOut);

                stopwatch.Start();
                results = connection.ExecuteStoredProcedure(getAllStoredProcedure);
                stopwatch.Stop();
            }
            elapsed = stopwatch.Elapsed;
            //var results = resultSet.RecordSet1;

            // ASSERT
            Assert.AreEqual(expectedNumberOfRecords, results.Count);
            Assert.Inconclusive("Return Duration: " + elapsed);
        }

        [TestMethod]
        [Ignore] /* long running so switch in  / out as required */
        public void HowLongToSelect1MRows()
        {
            /* Note it takes 9:55 minutes to INSERT 1M of these rows in sql server */
            /* Note it takes 10 seconds to SELECT 1M of these rows in sql server */
            /* Approx time to select and process in framework is 00:00:09.6394072 seconds */

            // ARRANGE
            const int expectedNumberOfRecords = 1000000;
            const int commandTimeOut = 800;
            bool truncateData = false;
            bool dataIsPresent = true;
            var truncateTableProcedure = new VolumeAndPerformanceTruncateStoredProcedure();
            var insertTableProcedureParameters = new VolumeAndPerformanceInsertNRecordsStoredProcedureParameters
            {
                NumberOfRecords = expectedNumberOfRecords
            };
            var insertTableProcedure = new VolumeAndPerformanceInsertNRecordsStoredProcedure(insertTableProcedureParameters);
            var getAllStoredProcedure = new VolumeAndPerformanceGetAllStoredProcedure();
            var connectionString = ConfigurationManager.ConnectionStrings["IntegrationTestConnection"].ConnectionString;
            //VolumeAndPerformanceGetAllStoredProcedureResultSet resultSet;
            List<VolumeAndPerformanceGetAllStoredProcedureReturnType> results;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan elapsed;

            // ACT
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (truncateData) connection.ExecuteStoredProcedure(truncateTableProcedure);
                if (!dataIsPresent) connection.ExecuteStoredProcedure(insertTableProcedure, commandTimeout: commandTimeOut);

                stopwatch.Start();
                results = connection.ExecuteStoredProcedure(getAllStoredProcedure);
                stopwatch.Stop();
            }
            elapsed = stopwatch.Elapsed;
            //var results = resultSet.RecordSet1;

            // ASSERT
            Assert.AreEqual(expectedNumberOfRecords, results.Count);
            Assert.Inconclusive("Return Duration: " + elapsed);
        }

        [TestMethod]
        /*[Ignore] /* long running so switch in  / out as required */
        public void HowLongToSelect2MRows()
        {
            /* Note it takes 20 minutes to INSERT 2M of these rows in sql server */
            /* Note it takes 20 seconds to SELECT 2M of these rows in sql server */
            /* Approx time to select and process in framework is 00:00:19.6106512 seconds With Activator */
            /* Approx time to select and process in framework is 00:00:18.9935831 seconds With FastActivator */
            /* Approx time to select and process in framework is 00:00:19.8269147 seconds With FastActivator */
            // Even at 2M records there is still neglidgable difference between
            // standard Activator and FastActivator

            // ARRANGE
            const int expectedNumberOfRecords = 2000000;
            const int commandTimeOut = 1200;
            bool truncateData = false;
            bool dataIsPresent = true;
            var truncateTableProcedure = new VolumeAndPerformanceTruncateStoredProcedure();
            var insertTableProcedureParameters = new VolumeAndPerformanceInsertNRecordsStoredProcedureParameters
            {
                NumberOfRecords = expectedNumberOfRecords
            };
            var insertTableProcedure = new VolumeAndPerformanceInsertNRecordsStoredProcedure(insertTableProcedureParameters);
            var getAllStoredProcedure = new VolumeAndPerformanceGetAllStoredProcedure();
            var connectionString = ConfigurationManager.ConnectionStrings["IntegrationTestConnection"].ConnectionString;
            //VolumeAndPerformanceGetAllStoredProcedureResultSet resultSet;
            List<VolumeAndPerformanceGetAllStoredProcedureReturnType> results;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan elapsed;

            // ACT
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (truncateData) connection.ExecuteStoredProcedure(truncateTableProcedure);
                if (!dataIsPresent) connection.ExecuteStoredProcedure(insertTableProcedure, commandTimeout: commandTimeOut);

                stopwatch.Start();
                results = connection.ExecuteStoredProcedure(getAllStoredProcedure);
                stopwatch.Stop();
            }
            elapsed = stopwatch.Elapsed;
            //var results = resultSet.RecordSet1;

            // ASSERT
            Assert.AreEqual(expectedNumberOfRecords, results.Count);
            Assert.Inconclusive("Return Duration: " + elapsed);
        }

        [TestMethod]
        [Ignore] /* long running so switch in  / out as required */
        public void HowLongToSelect3MRows()
        {
            /* Note it takes ?? minutes to INSERT 3M of these rows in sql server */
            /* Note it takes ?? seconds to SELECT 3M of these rows in sql server */

            // ARRANGE
            const int expectedNumberOfRecords = 3000000;
            const int commandTimeOut = 1200;
            bool truncateData = false;
            bool dataIsPresent = true;
            var truncateTableProcedure = new VolumeAndPerformanceTruncateStoredProcedure();
            var insertTableProcedureParameters = new VolumeAndPerformanceInsertNRecordsStoredProcedureParameters
            {
                NumberOfRecords = expectedNumberOfRecords
            };
            var insertTableProcedure = new VolumeAndPerformanceInsertNRecordsStoredProcedure(insertTableProcedureParameters);
            var getAllStoredProcedure = new VolumeAndPerformanceGetAllStoredProcedure();
            var connectionString = ConfigurationManager.ConnectionStrings["IntegrationTestConnection"].ConnectionString;
            //VolumeAndPerformanceGetAllStoredProcedureResultSet resultSet;
            List<VolumeAndPerformanceGetAllStoredProcedureReturnType> results;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan elapsed;

            // ACT
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (truncateData) connection.ExecuteStoredProcedure(truncateTableProcedure);
                if (!dataIsPresent) connection.ExecuteStoredProcedure(insertTableProcedure, commandTimeout: commandTimeOut);

                stopwatch.Start();
                results = connection.ExecuteStoredProcedure(getAllStoredProcedure);
                stopwatch.Stop();
            }
            elapsed = stopwatch.Elapsed;
            //var results = resultSet.RecordSet1;

            // ASSERT
            Assert.AreEqual(expectedNumberOfRecords, results.Count);
            Assert.Inconclusive("Return Duration: " + elapsed);
        }
    }
}
