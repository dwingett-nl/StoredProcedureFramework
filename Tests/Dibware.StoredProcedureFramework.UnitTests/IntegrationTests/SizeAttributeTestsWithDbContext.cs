﻿using Dibware.StoredProcedureFramework.Exceptions;
using Dibware.StoredProcedureFramework.Tests.IntegrationTests.Base;
using Dibware.StoredProcedureFramework.Tests.IntegrationTests.StoredProcedures.SizeAttributeProcedures;
using Dibware.StoredProcedureFrameworkForEF.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Dibware.StoredProcedureFramework.Tests.IntegrationTests
{
    [TestClass]
    public class SizeAttributeTestsWithDbContext : BaseIntegrationTestWithDbContext
    {
        //[TestMethod]
        //[ExpectedException(typeof(SqlParameterOutOfRangeException))]
        //public void CallVarcharProcedureWithIncorrectParameterSize_ThrowsSqlParameterOutOfRangeException()
        //{
        //    // ARRANGE
        //    const string initialvalue = @"12345678901234567890";
        //    var parameters = new VarCharTestProcedureParameters
        //    {
        //        Parameter1 = initialvalue
        //    };
        //    var procedure = new VarCharTestProcedureStoredProcedure(parameters);

        //    // ACT
        //    Context.ExecuteStoredProcedure(procedure);

        //    // ASSERT
        //    Assert.Fail();
        //}


        [TestMethod]
        public void CallProcedureWithSameSizeAttributeAsData_ResultsInNoLossOfData()
        {
            // ARRANGE
            const string initialValue = @"12345678901234567890";
            var parameters = new CorrectSizeAttributeParameters
            {
                Value1 = initialValue
            };
            var procedure = new CorrectSizeAttributeStoredProcedure(parameters);

            // ACT
            var resultSet = Context.ExecuteStoredProcedure(procedure);
            //var results = resultSet.RecordSet1;
            var result = resultSet.First();

            // ASSERT
            Assert.AreEqual(initialValue, result.Value1);
        }

        [TestMethod]
        public void CallProcedureWithDataSmallerThanSizeAttribute_ResultsInNoLossOfData()
        {
            // ARRANGE
            const string initialValue = @"1234567890";
            var parameters = new CorrectSizeAttributeParameters
            {
                Value1 = initialValue
            };
            var procedure = new CorrectSizeAttributeStoredProcedure(parameters);

            // ACT
            var resultSet = Context.ExecuteStoredProcedure(procedure);
            //var results = resultSet.RecordSet1;
            var result = resultSet.First();

            // ASSERT
            Assert.AreEqual(initialValue, result.Value1);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlParameterOutOfRangeException))]
        public void CallProcedureWithDataLargerThanSizeAttribute_ThrowsException()
        {
            // ARRANGE
            const string initialValue = @"123456789012345678901234567890";
            var parameters = new CorrectSizeAttributeParameters
            {
                Value1 = initialValue
            };
            var procedure = new CorrectSizeAttributeStoredProcedure(parameters);

            // ACT
            Context.ExecuteStoredProcedure(procedure);
            //var result = results.First();

            // ASSERT
            Assert.Fail();
            //Assert.AreEqual(initialValue, result.Value1);
        }

        [TestMethod]
        public void CallProcedureWithSizeAttributeSmallerThanProcedure_ReturnsCorrectData()
        {
            // ARRANGE
            const string initialValue = @"1234567890";
            var parameters = new TooSmallSizeAttributeParameters
            {
                Value1 = initialValue
            };
            var procedure = new TooSmallSizeAttributeStoredProcedure(parameters);

            // ACT
            var resultSet = Context.ExecuteStoredProcedure(procedure);
            //var results = resultSet.RecordSet1;
            var result = resultSet.First();

            // ASSERT
            Assert.AreEqual(initialValue, result.Value1);
        }

        // DEV NOTE: To be able to write code for this test to pass we would need to 
        // call SqlCommand.DeriveParameters. This can have quite a performance hit 
        // without some level of parameter caching.
        [TestMethod]
        [Ignore]
        [ExpectedException(typeof(SqlParameterOutOfRangeException))]
        public void CallProcedureWithSizeAttributeLargerThanProcedure_ThrowsException()
        {
            // ARRANGE
            const string initialValue = @"123456789012345678901234567890";
            var parameters = new TooLargeSizeAttributeParameters
            {
                Value1 = initialValue
            };
            var procedure = new TooLargeSizeAttributeStoredProcedure(parameters);

            // ACT
            var resultSet = Context.ExecuteStoredProcedure(procedure);

            // ASSERT
            // Exception should have been thrown by here!
        }

        [TestMethod]
        public void CallProcedureWithParameterValueAndSizeAttributeLongerThanProcedureParameterLength_TruncatesValueOncedPassedToStoredProcedure()
        {
            // ARRANGE
            const int expectedTruncatedLength = 20;
            const string initialValue = @"123456789012345678901234567890";
            var parameters = new TooLargeSizeAttributeParameters
            {
                Value1 = initialValue
            };
            var procedure = new TooLargeSizeAttributeStoredProcedure(parameters);

            // ACT
            var resultSet = Context.ExecuteStoredProcedure(procedure);
            var result = resultSet.First();

            // ASSERT
            Assert.AreEqual(expectedTruncatedLength, result.Value1.Length);
        }

        // TODO: Investigate if we can set VARCHAR size from the value of the parameters,
        // and where is best to perform this... Refer to Issue #1
        [TestMethod]
        [Ignore]
        public void CallProcedureWithParameterValueLongerThanProcedureParameterLengthAndNoSizeAttributeSet_TruncatesValueOncedPassedToStoredProcedure()
        {
            // ARRANGE
            const int expectedTruncatedLength = 20;
            const string initialValue = @"123456789012345678901234567890";
            var parameters = new TooLargeValueButNoSizeAttribute
            {
                Value1 = initialValue
            };
            var procedure = new TooLargeValueButNoSizeAttributeStoredProcedure(parameters);

            // ACT
            var resultSet = Context.ExecuteStoredProcedure(procedure);
            var result = resultSet.First();

            // ASSERT
            Assert.AreEqual(expectedTruncatedLength, result.Value1.Length);
        }
    }
}