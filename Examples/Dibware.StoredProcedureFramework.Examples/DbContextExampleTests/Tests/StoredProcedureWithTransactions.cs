﻿using Dibware.StoredProcedureFramework.Examples.DbContextExampleTests.Context;
using Dibware.StoredProcedureFramework.Examples.StoredProcedures;
using Dibware.StoredProcedureFramework.Examples.StoredProcedures.Parameters;
using Dibware.StoredProcedureFrameworkForEF.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Dibware.StoredProcedureFramework.Examples.DbContextExampleTests.Tests
{
    [TestClass]
    public class StoredProcedureWithTransactions
    {
        [TestMethod]
        public void StoredProcedure_WithTransactionScopeNotCommited_DoesNotWriteRecords()
        {
            // ARRANGE
            const int expectedIntermediateCount = 5;
            int originalCount;
            int intermediateCount;
            int finalCount;
            string connectionName = Properties.Settings.Default.ExampleDatabaseConnection;
            var companiesToAdd = new List<CompaniesAdd.CompanyTableType>
            {
                new CompaniesAdd.CompanyTableType { CompanyName = "Company 1", IsActive = true, TenantId = 2 },
                new CompaniesAdd.CompanyTableType { CompanyName = "Company 2", IsActive = false, TenantId = 2 },
                new CompaniesAdd.CompanyTableType { CompanyName = "Company 3", IsActive = true, TenantId = 2 }
            };
            var parameters = new CompaniesAdd.CompaniesAddParameters
            {
                Companies = companiesToAdd
            };
            var companyAddProcedure = new CompaniesAdd(parameters);
            var companyCountProcedure = new CompanyCountAll();

            // ACT
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (var context = new ApplicationDbContext(connectionName))
                {
                    originalCount = context.ExecuteStoredProcedure(companyCountProcedure).First().CountOfCompanies;
                    context.ExecuteStoredProcedure(companyAddProcedure);
                    intermediateCount = context.ExecuteStoredProcedure(companyCountProcedure).First().CountOfCompanies;
                }
            }
            using (var context = new ApplicationDbContext(connectionName))
            {

                finalCount = context.ExecuteStoredProcedure(companyCountProcedure).First().CountOfCompanies;

            }

            // ASSERT
            Assert.AreEqual(originalCount, finalCount);
            Assert.AreEqual(expectedIntermediateCount, intermediateCount);
        }

        [TestMethod]
        public void StoredProcedure_WithTransactionScopeCompleted_DoesWriteRecords()
        {
            // ARRANGE
            const int expectedIntermediateCount = 5;
            int originalCount;
            int intermediateCount;
            int finalCount;
            string connectionName = Properties.Settings.Default.ExampleDatabaseConnection;
            var companiesToAdd = new List<CompaniesAdd.CompanyTableType>
            {
                new CompaniesAdd.CompanyTableType { CompanyName = "Company 1", IsActive = true, TenantId = 2 },
                new CompaniesAdd.CompanyTableType { CompanyName = "Company 2", IsActive = false, TenantId = 2 },
                new CompaniesAdd.CompanyTableType { CompanyName = "Company 3", IsActive = true, TenantId = 2 }
            };
            var companiesAddParameters = new CompaniesAdd.CompaniesAddParameters
            {
                Companies = companiesToAdd
            };
            var companyAddProcedure = new CompaniesAdd(companiesAddParameters);
            var companyCountProcedure = new CompanyCountAll();
            var companyDeleteParameters = new TenantIdParameters
            {
                TenantId = 2
            };
            var companyDeleteProcedure = new CompanyDeleteForTenantId(companyDeleteParameters);

            // ACT
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (var context = new ApplicationDbContext(connectionName))
                {
                    originalCount = context.ExecuteStoredProcedure(companyCountProcedure).First().CountOfCompanies;
                    context.ExecuteStoredProcedure(companyAddProcedure);
                    transactionScope.Complete();
                }
            }
            using (var context = new ApplicationDbContext(connectionName))
            {
                intermediateCount = context.ExecuteStoredProcedure(companyCountProcedure).First().CountOfCompanies;
                context.ExecuteStoredProcedure(companyDeleteProcedure);
                finalCount = context.ExecuteStoredProcedure(companyCountProcedure).First().CountOfCompanies;
            }

            // ASSERT
            Assert.AreEqual(originalCount, finalCount);
            Assert.AreEqual(expectedIntermediateCount, intermediateCount);
        }
    }
}