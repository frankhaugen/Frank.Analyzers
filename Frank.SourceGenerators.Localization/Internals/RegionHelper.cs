using System.Globalization;

namespace Frank.SourceGenerators.Localization.Internals;

internal static class RegionHelper
{
    public static RegionInfo[] GetAllRegions() => CultureHelper
        .GetAllCultures()
        .Select(c => TryHelper.Try<RegionInfo>(() => new RegionInfo(c.Name)))
        .Where(r => r != null)
        .Select(r => r!)
        .DistinctBy(r => r.TwoLetterISORegionName)
        .Where(r => r.TwoLetterISORegionName.Length is 2 or 3)
        .Where(r => r.TwoLetterISORegionName.All(char.IsLetter))
        .ToArray();
    
    public static CultureInfo GetRegionCultureInfo(RegionInfo region) => CultureHelper
        .GetAllCultures()
        .Select(c => TryHelper.Try(() => new { RegionInfo = new RegionInfo(c.Name), CultureInfo = c}))
        .Where(r => r != null)
        .Select(r => r!)
        .DistinctBy(r => r.RegionInfo.TwoLetterISORegionName)
        .Where(r => r.RegionInfo.TwoLetterISORegionName.Length is 2 or 3)
        .Where(r => r.RegionInfo.TwoLetterISORegionName.All(char.IsLetter))
        .FirstOrDefault(r => r.RegionInfo.TwoLetterISORegionName == region.TwoLetterISORegionName)
        ?.CultureInfo ?? CultureInfo.InvariantCulture;
}