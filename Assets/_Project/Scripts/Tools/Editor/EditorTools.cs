using System;
using System.IO;
using _Project.Scripts.Services.SaveLoad.LocalSave;
using Unity.Services.CloudSave;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Tools.Editor
{
    public class EditorTools
    {
        [MenuItem("Tools/Project/Clear cloud save")]
        public static async void ClearCloudSave()
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.DeleteAllAsync();
                Debug.Log("[EDITOR TOOLS] Progress was deleted from cloud");
            }
            catch (Exception e)
            {
                Debug.LogError($"[EDITOR TOOLS] Failed to delete cloud save: {e}");
            }
        }

        [MenuItem("Tools/Project/Clear file save")]
        public  static void ClearLocalFileSave()
        {
            string saveDirectoryPath = Path.Combine(Application.persistentDataPath, FileSaveService.FolderName);

            if (Directory.Exists(saveDirectoryPath))
            {
                Directory.Delete(saveDirectoryPath, recursive: true);
                Debug.Log($"[EDITOR TOOLS] Progress was deleted from file\nPath: {saveDirectoryPath}");
            }
            else
                Debug.Log("[EDITOR TOOLS] Nothing to delete — folder not exist");
        }
        
        [MenuItem("Tools/Project/Clear playerPrefs save")]
        public  static void ClearLocalPlayerPrefsSave()
        {
           PlayerPrefs.DeleteAll();
           Debug.Log("[EDITOR TOOLS] Progress was deleted from playerPrefs");
        }

        [MenuItem("Tools/Project/Clear addressable remote cache")]
        public  static void ClearAddressableRemoteCache()
        {
            bool success = Caching.ClearCache();

            if (success)
                Debug.Log("[EDITOR TOOLS] Successfully cleared local cache");
            else
                Debug.LogWarning("[EDITOR TOOLS] Unable to clear cache");
        }
    }
}