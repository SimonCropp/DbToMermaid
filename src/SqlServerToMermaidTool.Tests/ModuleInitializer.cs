public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.InitializePlugins();
        // Skia text antialiasing varies by a few pixels between runs
        VerifyImageMagick.RegisterComparers(.01);
    }
}
