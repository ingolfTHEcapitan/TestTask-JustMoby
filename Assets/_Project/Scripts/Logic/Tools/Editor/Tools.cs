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
    }
}