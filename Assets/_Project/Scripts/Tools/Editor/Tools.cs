using System.IO;
using _Project.Scripts.Services.SaveLoad.LocalSave;
using Unity.Services.CloudSave;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Tools.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Clear cloud save")]
        public async static void ClearCloudSave()
        {
            await CloudSaveService.Instance.Data.Player.DeleteAllAsync();
            Debug.Log("Progress was deleted from cloud save");
        }

        [MenuItem("Tools/Clear local file save")]
        public  static void ClearLocalFileSave()
        {
            string saveDirectoryPath = Path.Combine(Application.persistentDataPath, FileSaveService.FolderName);

            if (Directory.Exists(saveDirectoryPath))
            {
                Directory.Delete(saveDirectoryPath, recursive: true);
                Debug.Log($"Progress was deleted from local file save\nPath: {saveDirectoryPath}");
            }
        }
        
        [MenuItem("Tools/Clear local playerPrefs save")]
        public  static void ClearLocalPlayerPrefsSave()
        {
           PlayerPrefs.DeleteAll();
           Debug.Log("Progress was deleted from local playerPrefs save");
        }

        [MenuItem("Tools/Clear addressable remote cache")]
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