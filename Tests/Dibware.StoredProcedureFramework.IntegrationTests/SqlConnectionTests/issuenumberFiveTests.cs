﻿using Dibware.StoredProcedureFramework.Extensions;
using Dibware.StoredProcedureFramework.IntegrationTests.StoredProcedures;
using Dibware.StoredProcedureFramework.IntegrationTests.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dibware.StoredProcedureFramework.IntegrationTests.SqlConnectionTests
{
    /// <summary>
    /// These tests are for the issue identified in Issue No 5
    /// </summary>
    [TestClass]
    public class I005IssueNumberFiveTests
         : BaseSqlConnectionIntegrationTest
    {
        [TestMethod]
        public void IssueNumberFive_WithCorrectDataType_ExecutesWithoutIssue()
        {
            // ARRANGE
            var procedure = new IssueNumberFiveWithCorrectDataType();

            // ACT
            Connection.ExecuteStoredProcedure(procedure);

            // ASSERT

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void IssueNumberFiveWithIncorrectDataType_ThrowsInvalidCastException()
        {
            // ARRANGE
            var procedure = new IssueNumberFiveWithIncorrectDataType();

            // ACT
            Connection.ExecuteStoredProcedure(procedure);

            // ASSERT
        }
    }
}