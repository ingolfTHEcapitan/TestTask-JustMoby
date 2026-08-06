using System;

namespace _Project.Scripts.Services.SaveLoad
{
    public class SaveTimeFormater
    {
        public string GetFormatedSaveTime(long lastSaveTimeUnix, bool getLocalDateTime = true)
        {
            DateTimeOffset unixSaveTime = DateTimeOffset.FromUnixTimeSeconds(lastSaveTimeUnix);

            if (getLocalDateTime) 
                unixSaveTime = unixSaveTime.LocalDateTime;
            
            return unixSaveTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}