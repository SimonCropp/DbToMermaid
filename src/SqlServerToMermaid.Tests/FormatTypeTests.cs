public class FormatTypeTests
{
    [Test]
    public async Task NVarCharMax_maps_to_nvarchar() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.NVarCharMax))).IsEqualTo("nvarchar");

    [Test]
    public async Task Decimal_maps_to_decimal() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.Decimal))).IsEqualTo("decimal");

    [Test]
    public async Task DateTime2_maps_to_datetime2() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.DateTime2))).IsEqualTo("datetime2");

    [Test]
    public async Task VarBinaryMax_maps_to_varbinary() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.VarBinaryMax))).IsEqualTo("varbinary");
}
