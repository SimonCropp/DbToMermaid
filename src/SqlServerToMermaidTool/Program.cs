return await new CommandLineApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetExecutableName("sql2mermaid")
    .SetTitle("SqlServerToMermaid CLI")
    .Build()
    .RunAsync();
