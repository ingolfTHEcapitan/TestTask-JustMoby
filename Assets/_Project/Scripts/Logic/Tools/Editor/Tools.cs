using Unity.Services.CloudSave;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Logic.Tools.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Delete Cloud Save")]
        public async static void DeleteCloudSave()
        {
            await CloudSaveService.Instance.Data.Player.DeleteAllAsync();
            Debug.Log("Progress was deleted from Cloud Save");
        }
        
        [MenuItem("Tools/Clear addressable remote Cache ")]
        public  static void ClearAddressableRemoteCache()
        {
            bool success = Caching.ClearCache();

            if (!success) 
                Debug.Log("Unable to clear cache");
            else
                Debug.Log("Successfully cleared local cache");
        }
    }
}