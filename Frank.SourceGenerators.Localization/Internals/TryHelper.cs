namespace Frank.SourceGenerators.Localization.Internals;

internal static class TryHelper
{
    public static T? Try<T>(Func<T> func, T? @default = default)
    {
        try
        {
            return func();
        }
        catch
        {
            return @default;
        }
    }
}