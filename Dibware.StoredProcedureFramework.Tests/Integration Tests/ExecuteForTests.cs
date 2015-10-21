﻿using System;
using Dibware.StoredProcedureFramework.Tests.Examples.StoredProcedures;
using Dibware.StoredProcedureFramework.Tests.Integration_Tests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Dibware.StoredProcedureFramework.Tests.Integration_Tests
{
    [TestClass]
    public class ExecuteForTests : BaseIntegrationTest
    {
        [TestMethod]
        public void ExecuteFor_WhenPassedConstructedParameters_GetsExpectedResults()
        {
            // ARRANGE
            const int expectedId = 10;
            const string expectedName = @"Dave";
            const bool expectedActive = true;
            NormalStoredProcedureResultSet resultSet;

            var parameters = new NormalStoredProcedureParameters
            {
                Id = expectedId
            };

            // ACT
            resultSet = Context.NormalStoredProcedure.ExecuteFor(parameters);
            var results = resultSet.RecordSet1;
            var result = results.First();

            // ASSERT
            Assert.AreEqual(expectedId, result.Id);
            Assert.AreEqual(expectedName, result.Name);
            Assert.AreEqual(expectedActive, result.Active);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteFor_WhenPassedNullParameters_ThrowsArgumentNullException()
        {
            // ARRANGE

            // ACT
            Context.NormalStoredProcedure.ExecuteFor(null);

            // ASSERT
            // Should have thrown exception by here!
        }
    }
}
