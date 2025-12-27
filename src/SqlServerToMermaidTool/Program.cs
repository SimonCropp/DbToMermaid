using CliFx;

return await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetExecutableName("sql2mermaid")
    .SetTitle("SqlServerToMermaid CLI")
    .SetVersion("0.1.0")
    .Build()
    .RunAsync();
