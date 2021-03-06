﻿using System.Collections.Generic;
using System.Linq;
using Dibware.StoredProcedureFramework.Examples.Dtos;
using Dibware.StoredProcedureFramework.Examples.SqlConnectionExampleTests.Base;
using Dibware.StoredProcedureFramework.Examples.StoredProcedures;
using Dibware.StoredProcedureFramework.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dibware.StoredProcedureFramework.Examples.SqlConnectionExampleTests.Tests
{
    [TestClass]
    public class StoredProcedureWithoutParametersButWithReturnType
        : SqlConnectionExampleTestBase
    {
        [TestMethod]
        public void TenantGetAll()
        {
            // ARRANGE
            var procedure = new TenantGetAll();
            const int expectedTenantCount = 2;

            // ACT
            List<TenantDto> tenants = Connection.ExecuteStoredProcedure(procedure);
            TenantDto tenant1 = tenants.FirstOrDefault();

            // ASSERT
            Assert.AreEqual(expectedTenantCount, tenants.Count);
            Assert.IsNotNull(tenant1);
        }

        [TestMethod]
        public void CompanyGetAll()
        {
            // ARRANGE
            var procedure = new CompanyGetAll();
            const int expectedCompanyCount = 2;

            // ACT
            var companies = Connection.ExecuteStoredProcedure(procedure);
            CompanyDto company1 = companies.FirstOrDefault();

            // ASSERT
            Assert.AreEqual(expectedCompanyCount, companies.Count);
            Assert.IsNotNull(company1);
        }
    }
}