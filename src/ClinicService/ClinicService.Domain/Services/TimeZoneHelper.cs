namespace ClinicService.Domain.Services;

public static class TimeZoneHelper
{
    public static TimeZoneInfo Find(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            // Windows ID supplied but running on Linux — try IANA equivalent
            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(timeZoneId, out var ianaId))
                return TimeZoneInfo.FindSystemTimeZoneById(ianaId);

            // IANA ID supplied but running on Windows — try Windows equivalent
            if (TimeZoneInfo.TryConvertIanaIdToWindowsId(timeZoneId, out var windowsId))
                return TimeZoneInfo.FindSystemTimeZoneById(windowsId);

            throw;
        }
    }
}
