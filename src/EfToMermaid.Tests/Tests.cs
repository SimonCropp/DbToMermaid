public class Tests
{
    [Test]
    public async Task RenderMarkdown()
    {
        #region EfUsage

        var options = new DbContextOptionsBuilder<SampleDbContext>()
            .UseSqlServer("Fake")
            .Options;

        await using var context = new SampleDbContext(options);

        var markdown = await EfToMermaid.RenderMarkdown(context.Model);

        #endregion

        await Verify(markdown, extension: "md")
            .AddScrubber(_ => _.Insert(0, '\n'));
    }
}
