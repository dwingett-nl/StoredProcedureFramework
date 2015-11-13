﻿
using Dibware.StoredProcedureFramework.StoredProcedureAttributes;

namespace Dibware.StoredProcedureFramework.Tests.IntegrationTests.StoredProcedures.PrecisionAndScale
{
    internal class DecimalPrecisionAndScaleReturnType
    {
        [Precision(10)]
        [Scale(3)]
        public decimal Value1 { get; set; }

        [Precision(10)]
        [Scale(3)]
        public decimal Value2 { get; set; }
    }
}