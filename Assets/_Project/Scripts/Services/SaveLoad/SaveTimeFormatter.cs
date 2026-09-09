using System;

namespace _Project.Scripts.Services.SaveLoad
{
    public class SaveTimeFormatter
    {
        public string Format(long lastSaveTimeUnix, bool useLocalDateTime = true)
        {
            DateTimeOffset unixSaveTime = DateTimeOffset.FromUnixTimeSeconds(lastSaveTimeUnix);

            if (useLocalDateTime) 
                unixSaveTime = unixSaveTime.LocalDateTime;
            
            return unixSaveTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}