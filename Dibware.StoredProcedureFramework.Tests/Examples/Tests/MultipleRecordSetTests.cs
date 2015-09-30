﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dibware.StoredProcedureFramework.Extensions;
using Dibware.StoredProcedureFramework.Tests.Integration_Tests.StoredProcedures.MultipleRecordSets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dibware.StoredProcedureFramework.Tests.Examples.Tests
{
    [TestClass]
    public class MultipleRecordSetTests
    {
        [TestMethod]
        public void MultipleRecordSetStoredProcedure_WithThreeSelects_ReturnsThreeRecordSets()
        {
            // ARRANGE
            const int expectedId = 10;
            const string expectedName = "Sid";
            const bool expectedActive = true;
            const decimal expectedPrice = 10.99M;
            Guid expectedUniqueIdentifier = Guid.NewGuid();
            const byte expectedCount = 17;
            var parameters = new MultipleRecordSetStoredProcedureParameters
            {
                Id = expectedId,
                Name = expectedName,
                Active = expectedActive,
                Price = expectedPrice,
                UniqueIdentifier = expectedUniqueIdentifier,
                Count = expectedCount
            };
            MultipleRecordSetStoredProcedureResultSet resultSet;
            var procedure = new MultipleRecordSetStoredProcedure(parameters);
            var connectionString = ConfigurationManager.ConnectionStrings["IntegrationTestConnection"].ConnectionString;

            // ACT
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                resultSet = connection.ExecuteStoredProcedure(procedure);
            }
            var results1 = resultSet.RecordSet1;
            var result1 = results1.First();

            var results2 = resultSet.RecordSet2;
            var result2 = results2.First();

            var results3 = resultSet.RecordSet3;
            var result3 = results3.First();

            // ASSERT
            Assert.AreEqual(expectedId, result1.Id);
            Assert.AreEqual(expectedName, result1.Name);

            Assert.AreEqual(expectedActive, result2.Active);
            Assert.AreEqual(expectedPrice, result2.Price);

            Assert.AreEqual(expectedUniqueIdentifier, result3.UniqueIdentifier);
            Assert.AreEqual(expectedCount, result3.Count);
        }
    }
}