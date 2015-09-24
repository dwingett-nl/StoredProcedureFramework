﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dibware.StoredProcedureFramework.Extensions;
using Dibware.StoredProcedureFramework.Tests.Examples.StoredProcedures;
using Dibware.StoredProcedureFramework.Tests.Integration_Tests.Base;
using Dibware.StoredProcedureFrameworkForEF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace Dibware.StoredProcedureFramework.Tests.Examples.Tests
{
    [TestClass]
    public class MostBasicStoredProcedureTests : BaseIntegrationTest
    {
        [TestMethod]
        public void NormalStoredProcedure_WhenCalledOnDbContext_ReturnsCorrectValues()
        {
            // ARRANGE  
            const int expectedId = 10;
            const string expectedName = @"Dave";
            const bool expectedActive = true;

            var parameters = new NormalStoredProcedureParameters
            {
                Id = expectedId
            };
            var procedure = new NormalStoredProcedure(parameters);
            
            // ACT
            var results = Context.ExecuteStoredProcedure(procedure);
            var result = results.First();

            // ASSERT
            Assert.AreEqual(expectedId, result.Id);
            Assert.AreEqual(expectedName, result.Name);
            Assert.AreEqual(expectedActive, result.Active);
        }

        [TestMethod]
        public void NormalStoredProcedure_WhenCalledOnSqlConnection_ReturnsCorrectValues()
        {
            // ARRANGE  
            const int expectedId = 10;
            const string expectedName = @"Dave";
            const bool expectedActive = true;

            var parameters = new NormalStoredProcedureParameters
            {
                Id = expectedId
            };
            List<NormalStoredProcedureReturnType> results;
            var procedure = new NormalStoredProcedure(parameters);
            var connectionString = ConfigurationManager.ConnectionStrings["IntegrationTestConnection"].ConnectionString;
            
            // ACT
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                results = connection.ExecuteStoredProcedure(procedure);
            }
            var result = results.First();
            
            // ASSERT
            Assert.AreEqual(expectedId, result.Id);
            Assert.AreEqual(expectedName, result.Name);
            Assert.AreEqual(expectedActive, result.Active);
        }
    }
}