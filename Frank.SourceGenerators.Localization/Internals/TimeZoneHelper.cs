namespace Frank.SourceGenerators.Localization.Internals;

internal static class TimeZoneHelper
{
    public static TimeZoneInfo[] GetAllTimeZones() => TimeZoneInfo.GetSystemTimeZones().ToArray();
    
    public static TimeZoneInfo GetTimeZone(string id) => TimeZoneInfo.FindSystemTimeZoneById(id);
    
    public static TimeZoneInfo GetTimeZone(int id) => TimeZoneInfo.FindSystemTimeZoneById(id.ToString());
    
    public static TimeZoneInfo GetLocalTimeZone() => TimeZoneInfo.Local;
    
    public static TimeZoneInfo GetUtcTimeZone() => TimeZoneInfo.Utc;
}