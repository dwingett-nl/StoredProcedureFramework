﻿
using Dibware.StoredProcedureFramework.StoredProcAttributes;

namespace Dibware.StoredProcedureFramework.Tests.Integration_Tests.StoredProcedures.PrecisionAndScale
{
    internal class DecimalPrecisionAndScaleParameters
    {
        [Precision(10)]
        [Scale(3)]
        public decimal Value1 { get; set; }

        [Precision(7)]
        [Scale(1)]
        public decimal Value2 { get; set; }
    }
}