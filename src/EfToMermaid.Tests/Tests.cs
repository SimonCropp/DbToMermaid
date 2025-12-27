public class Tests
{
    [Test]
    public async Task RenderMarkdown()
    {
        #region EfUsage

        var options = new DbContextOptionsBuilder<SampleDbContext>()
            // required to get an instace of a model without a running DB intsance
            .UseSqlServer("Fake")
            .Options;

        await using var context = new SampleDbContext(options);

        var markdown = await EfToMermaid.RenderMarkdown(context.Model);

        #endregion

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }

    [Test]
    public async Task WithSchema()
    {
        var options = new DbContextOptionsBuilder<WithSchemaDbContext>()
            .UseSqlServer("Fake")
            .Options;

        await using var context = new WithSchemaDbContext(options);

        var markdown = await EfToMermaid.RenderMarkdown(context.Model);

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }
}
