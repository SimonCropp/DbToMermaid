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
    public async Task WithNullable()
    {
        var options = new DbContextOptionsBuilder<WithNullableDbContext>()
            .UseSqlServer("Fake")
            .Options;

        await using var context = new WithNullableDbContext(options);

        var markdown = await EfToMermaid.RenderMarkdown(context.Model);

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }

    [Test]
    public async Task WithComments()
    {
        var options = new DbContextOptionsBuilder<WithCommentsDbContext>()
            .UseSqlServer("Fake")
            .Options;

        await using var context = new WithCommentsDbContext(options);

        var model = context.GetService<Microsoft.EntityFrameworkCore.Metadata.IDesignTimeModel>().Model;
        var markdown = await EfToMermaid.RenderMarkdown(model);

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }

    [Test]
    public async Task WithEscaping()
    {
        var options = new DbContextOptionsBuilder<WithEscapingDbContext>()
            .UseSqlServer("Fake")
            .Options;

        await using var context = new WithEscapingDbContext(options);

        var model = context.GetService<Microsoft.EntityFrameworkCore.Metadata.IDesignTimeModel>().Model;
        var markdown = await EfToMermaid.RenderMarkdown(model);

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
