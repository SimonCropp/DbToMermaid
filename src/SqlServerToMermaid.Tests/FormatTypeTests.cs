public class FormatTypeTests
{
    [Test]
    public async Task NVarCharMax_maps_to_nvarchar_max() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.NVarCharMax))).IsEqualTo("nvarchar(max)");

    [Test]
    public async Task NVarChar_includes_length() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.NVarChar, 50))).IsEqualTo("nvarchar(50)");

    [Test]
    public async Task Char_includes_length() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.Char, 3))).IsEqualTo("char(3)");

    [Test]
    public async Task Decimal_uses_default_precision_and_scale() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.Decimal))).IsEqualTo("decimal(18,0)");

    [Test]
    public async Task Decimal_includes_precision_and_scale() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.Decimal, 18, 2))).IsEqualTo("decimal(18,2)");

    [Test]
    public async Task DateTime2_omits_scale() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.DateTime2))).IsEqualTo("datetime2");

    [Test]
    public async Task DateTime2_with_explicit_scale_omits_scale() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.DateTime2, 3))).IsEqualTo("datetime2");

    [Test]
    public async Task Float_omits_precision() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.Float))).IsEqualTo("float");

    [Test]
    public async Task Float_with_explicit_precision_omits_precision() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.Float, 24))).IsEqualTo("float");

    [Test]
    public async Task VarBinaryMax_maps_to_varbinary_max() =>
        await Assert.That(SchemaReader.FormatType(new(SqlDataType.VarBinaryMax))).IsEqualTo("varbinary(max)");
}
