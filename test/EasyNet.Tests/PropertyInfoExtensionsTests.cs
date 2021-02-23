using System;
using EasyNet.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Tests
{
    public class PropertyInfoExtensionsTests
    {
        [Fact]
        public void TestSetValueAndAutoFit()
        {
            // Arrange
            var test = new TestClass();
            var type = test.GetType();

            // Act && Assert

            #region short

            type.GetProperty("Short")?.SetValueAndAutoFit<short>(test, 1);
            Assert.Equal(1, test.Short);

            type.GetProperty("Short")?.SetValueAndAutoFit<short>(test, null);
            Assert.Equal(0, test.Short);

            type.GetProperty("ShortOrNull")?.SetValueAndAutoFit<short?>(test, 2);
            Assert.Equal(2, test.ShortOrNull.Value);

            type.GetProperty("ShortOrNull")?.SetValueAndAutoFit<short?>(test, null);
            Assert.Null(test.ShortOrNull);

            #endregion

            #region int

            type.GetProperty("Int")?.SetValueAndAutoFit<int>(test, 1);
            Assert.Equal(1, test.Int);

            type.GetProperty("Int")?.SetValueAndAutoFit<int>(test, null);
            Assert.Equal(0, test.Int);

            type.GetProperty("IntOrNull")?.SetValueAndAutoFit<int?>(test, 2);
            Assert.Equal(2, test.IntOrNull.Value);

            type.GetProperty("IntOrNull")?.SetValueAndAutoFit<int?>(test, null);
            Assert.Null(test.IntOrNull);

            #endregion

            #region long

            type.GetProperty("Long")?.SetValueAndAutoFit<long>(test, 1);
            Assert.Equal(1, test.Long);

            type.GetProperty("Long")?.SetValueAndAutoFit<long>(test, null);
            Assert.Equal(0, test.Long);

            type.GetProperty("LongOrNull")?.SetValueAndAutoFit<long?>(test, 2);
            Assert.Equal(2, test.LongOrNull.Value);

            type.GetProperty("LongOrNull")?.SetValueAndAutoFit<long?>(test, null);
            Assert.Null(test.LongOrNull);

            #endregion

            #region DateTime

            var now = DateTime.Now;

            type.GetProperty("DateTime")?.SetValueAndAutoFit<DateTime>(test, now);
            Assert.Equal(now, test.DateTime);

            type.GetProperty("DateTime")?.SetValueAndAutoFit<DateTime>(test, null);
            Assert.Equal(default(DateTime), test.DateTime);

            type.GetProperty("DateTimeOrNull")?.SetValueAndAutoFit<DateTime?>(test, now);
            Assert.Equal(now, test.DateTimeOrNull.Value);

            type.GetProperty("DateTimeOrNull")?.SetValueAndAutoFit<DateTime?>(test, null);
            Assert.Null(test.DateTimeOrNull);

            #endregion

            #region float

            type.GetProperty("Float")?.SetValueAndAutoFit<float>(test, 1);
            Assert.Equal(1, test.Float);

            type.GetProperty("Float")?.SetValueAndAutoFit<float>(test, null);
            Assert.Equal(0, test.Float);

            type.GetProperty("FloatOrNull")?.SetValueAndAutoFit<float?>(test, 2);
            Assert.Equal(2, test.FloatOrNull.Value);

            type.GetProperty("FloatOrNull")?.SetValueAndAutoFit<float?>(test, null);
            Assert.Null(test.FloatOrNull);

            #endregion

            #region double

            type.GetProperty("Double")?.SetValueAndAutoFit<double>(test, 1);
            Assert.Equal(1, test.Double);

            type.GetProperty("Double")?.SetValueAndAutoFit<double>(test, null);
            Assert.Equal(0, test.Double);

            type.GetProperty("DoubleOrNull")?.SetValueAndAutoFit<double?>(test, 2);
            Assert.Equal(2, test.DoubleOrNull.Value);

            type.GetProperty("DoubleOrNull")?.SetValueAndAutoFit<double?>(test, null);
            Assert.Null(test.DoubleOrNull);

            #endregion

            #region decimal

            type.GetProperty("Decimal")?.SetValueAndAutoFit<decimal>(test, 1);
            Assert.Equal(1, test.Decimal);

            type.GetProperty("Decimal")?.SetValueAndAutoFit<decimal>(test, null);
            Assert.Equal(0, test.Decimal);

            type.GetProperty("DecimalOrNull")?.SetValueAndAutoFit<decimal?>(test, 2);
            Assert.Equal(2, test.DecimalOrNull.Value);

            type.GetProperty("DecimalOrNull")?.SetValueAndAutoFit<decimal?>(test, null);
            Assert.Null(test.DecimalOrNull);

            #endregion

            #region byte

            type.GetProperty("Byte")?.SetValueAndAutoFit<byte>(test, 1);
            Assert.Equal(1, test.Byte);

            type.GetProperty("Byte")?.SetValueAndAutoFit<byte>(test, null);
            Assert.Equal(default(byte), test.Byte);

            type.GetProperty("ByteOrNull")?.SetValueAndAutoFit<byte?>(test, 2);
            Assert.Equal(2, test.ByteOrNull.Value);

            type.GetProperty("ByteOrNull")?.SetValueAndAutoFit<byte?>(test, null);
            Assert.Null(test.ByteOrNull);

            #endregion

            #region char

            type.GetProperty("Char")?.SetValueAndAutoFit<char>(test, '1');
            Assert.Equal('1', test.Char);

            type.GetProperty("Char")?.SetValueAndAutoFit<char>(test, null);
            Assert.Equal(default(char), test.Char);

            type.GetProperty("CharOrNull")?.SetValueAndAutoFit<char?>(test, '2');
            Assert.Equal('2', test.CharOrNull.Value);

            type.GetProperty("CharOrNull")?.SetValueAndAutoFit<char?>(test, null);
            Assert.Null(test.CharOrNull);

            #endregion

            #region Guid

            type.GetProperty("Guid")?.SetValueAndAutoFit<Guid>(test, "E82EFFB7-9577-4856-7FF8-F7839BF6D140");
            Assert.Equal(Guid.Parse("E82EFFB7-9577-4856-7FF8-F7839BF6D140"), test.Guid);

            type.GetProperty("Guid")?.SetValueAndAutoFit<Guid>(test, null);
            Assert.Equal(default(Guid), test.Guid);

            type.GetProperty("GuidOrNull")?.SetValueAndAutoFit<Guid?>(test, "960888E9-E921-3BFA-75ED-5BC6D954F328");
            Assert.Equal(Guid.Parse("960888E9-E921-3BFA-75ED-5BC6D954F328"), test.GuidOrNull.Value);

            type.GetProperty("GuidOrNull")?.SetValueAndAutoFit<Guid?>(test, null);
            Assert.Null(test.GuidOrNull);

            #endregion
        }
    }

    public class TestClass
    {
        public short Short { get; set; }

        public short? ShortOrNull { get; set; }

        public int Int { get; set; }

        public int? IntOrNull { get; set; }

        public long Long { get; set; }

        public long? LongOrNull { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? DateTimeOrNull { get; set; }

        public float Float { get; set; }

        public float? FloatOrNull { get; set; }

        public double Double { get; set; }

        public double? DoubleOrNull { get; set; }

        public decimal Decimal { get; set; }

        public decimal? DecimalOrNull { get; set; }

        public byte Byte { get; set; }

        public byte? ByteOrNull { get; set; }

        public char Char { get; set; }

        public char? CharOrNull { get; set; }

        public Guid Guid { get; set; }

        public Guid? GuidOrNull { get; set; }
    }
}