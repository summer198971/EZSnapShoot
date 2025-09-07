using UnityEngine;
using UnityEditor;
using System.IO;
using EzGame.SnapShoot;

namespace EzGame.SnapShoot.Editor
{
    public static class SnapShootMenuItems
    {
        private const string MENU_ROOT = "EzGame/SnapShoot/";
        private const string DEFAULT_EXPORT_PATH = "Temp/SnapShoot_Exports/";
        
        [MenuItem(MENU_ROOT + "导出所有已加载场景 (Export All Loaded Scenes)", false, 1)]
        public static void ExportCurrentSceneHierarchy()
        {
            try
            {
                var xmlDoc = HierarchyToXML.GetCurrentSceneLoadedHierarchyToXML();
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                string fileName = $"Hierarchy_{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.xml";
                
                SaveXmlToFile(xmlDoc, fileName);
                Debug.Log("[EzGame.SnapShoot] " + $"场景层级已导出: {fileName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出场景层级失败: {e.Message}");
            }
        }
        
        [MenuItem(MENU_ROOT + "导出DontDestroyOnLoad对象 (Export DontDestroyOnLoad)", false, 2)]
        public static void ExportDontDestroyOnLoadHierarchy()
        {
            try
            {
                var xmlDoc = HierarchyToXML.GetDontDestroyOnLoadHierarchyToXML();
                string fileName = $"Hierarchy_DontDestroyOnLoad_{System.DateTime.Now:yyyyMMdd_HHmmss}.xml";
                
                SaveXmlToFile(xmlDoc, fileName);
                Debug.Log("[EzGame.SnapShoot] " + $"DontDestroyOnLoad层级已导出: {fileName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出DontDestroyOnLoad层级失败: {e.Message}");
            }
        }
        
        [MenuItem(MENU_ROOT + "打开导出文件夹 (Open Export Folder)", false, 21)]
        public static void OpenExportFolder()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", DEFAULT_EXPORT_PATH);
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            
            EditorUtility.RevealInFinder(fullPath);
        }
        
        [MenuItem(MENU_ROOT + "设置 (Settings)", false, 41)]
        public static void OpenSettings()
        {
            SnapShootSettingsWindow.ShowWindow();
        }
        
        private static void SaveXmlToFile(System.Xml.XmlDocument xmlDoc, string fileName)
        {
            // 确保导出目录存在
            string exportPath = DEFAULT_EXPORT_PATH;
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }
            
            string fullPath = Path.Combine(exportPath, fileName);
            
            // 保存XML文件
            using (var writer = new System.Xml.XmlTextWriter(fullPath, System.Text.Encoding.UTF8))
            {
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;
                xmlDoc.Save(writer);
            }
            
            // 刷新AssetDatabase以显示新文件
            AssetDatabase.Refresh();
        }
    }
}
