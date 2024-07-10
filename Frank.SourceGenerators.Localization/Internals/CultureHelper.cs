using System.Globalization;

namespace Frank.SourceGenerators.Localization.Internals;

internal static class CultureHelper
{
    public static CultureInfo[] GetAllCultures() => CultureInfo.GetCultures(CultureTypes.AllCultures);
    
    public static CultureInfo[] GetNeutralCultures() => CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(c => Equals(c.Parent, CultureInfo.InvariantCulture)).ToArray();
}