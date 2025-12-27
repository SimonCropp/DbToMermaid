return await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetExecutableName("sql2mermaid")
    .SetTitle("SqlServerToMermaid CLI")
    .Build()
    .RunAsync();
