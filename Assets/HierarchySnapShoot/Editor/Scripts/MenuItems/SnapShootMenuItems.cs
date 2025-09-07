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
        
        [MenuItem(MENU_ROOT + "Export Current Scene Hierarchy", false, 1)]
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
        
        [MenuItem(MENU_ROOT + "Export Specific Scene Hierarchy", false, 2)]
        public static void ExportSpecificSceneHierarchy()
        {
            try
            {
                // 获取所有已加载场景的名称
                string[] sceneNames = HierarchyToXML.GetLoadedSceneNames();
                
                if (sceneNames.Length == 0)
                {
                    EditorUtility.DisplayDialog("导出场景层级", "没有找到已加载的场景", "确定");
                    return;
                }
                
                // 显示场景选择对话框
                int selectedIndex = EditorUtility.DisplayDialogComplex(
                    "选择要导出的场景",
                    $"请选择要导出层级结构的场景:\n\n可用场景: {string.Join(", ", sceneNames)}",
                    "取消",
                    "显示选择窗口",
                    "导出所有场景"
                );
                
                if (selectedIndex == 0) // 取消
                {
                    return;
                }
                else if (selectedIndex == 1) // 显示选择窗口
                {
                    ShowSceneSelectionWindow();
                }
                else if (selectedIndex == 2) // 导出所有场景
                {
                    ExportAllLoadedScenes();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出指定场景层级失败: {e.Message}");
            }
        }
        
        [MenuItem(MENU_ROOT + "Export DontDestroyOnLoad Hierarchy", false, 3)]
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
        
        [MenuItem(MENU_ROOT + "Open Export Folder", false, 21)]
        public static void OpenExportFolder()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", DEFAULT_EXPORT_PATH);
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            
            EditorUtility.RevealInFinder(fullPath);
        }
        
        [MenuItem(MENU_ROOT + "Settings", false, 41)]
        public static void OpenSettings()
        {
            SnapShootSettingsWindow.ShowWindow();
        }
        
        /// <summary>
        /// 显示场景选择窗口
        /// </summary>
        private static void ShowSceneSelectionWindow()
        {
            string[] sceneNames = HierarchyToXML.GetLoadedSceneNames();
            
            if (sceneNames.Length == 1)
            {
                // 只有一个场景，直接导出
                ExportSingleScene(sceneNames[0]);
                return;
            }
            
            // 创建一个简单的选择对话框
            GenericMenu menu = new GenericMenu();
            
            foreach (string sceneName in sceneNames)
            {
                string currentSceneName = sceneName; // 避免闭包问题
                menu.AddItem(new GUIContent($"导出 {sceneName}"), false, () => ExportSingleScene(currentSceneName));
            }
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("导出所有场景"), false, ExportAllLoadedScenes);
            
            menu.ShowAsContext();
        }
        
        /// <summary>
        /// 导出单个场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        private static void ExportSingleScene(string sceneName)
        {
            try
            {
                var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(sceneName);
                
                if (xmlDoc == null)
                {
                    EditorUtility.DisplayDialog("导出失败", $"无法导出场景 '{sceneName}'，请确保场景已加载", "确定");
                    return;
                }
                
                string fileName = $"Hierarchy_{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.xml";
                SaveXmlToFile(xmlDoc, fileName);
                Debug.Log("[EzGame.SnapShoot] " + $"场景 '{sceneName}' 层级已导出: {fileName}");
                
                // 显示成功消息
                EditorUtility.DisplayDialog("导出成功", $"场景 '{sceneName}' 的层级结构已成功导出到:\n{fileName}", "确定");
            }
            catch (System.Exception e)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出场景 '{sceneName}' 失败: {e.Message}");
                EditorUtility.DisplayDialog("导出失败", $"导出场景 '{sceneName}' 时发生错误:\n{e.Message}", "确定");
            }
        }
        
        /// <summary>
        /// 导出所有已加载的场景
        /// </summary>
        private static void ExportAllLoadedScenes()
        {
            try
            {
                string[] sceneNames = HierarchyToXML.GetLoadedSceneNames();
                int successCount = 0;
                int failCount = 0;
                
                foreach (string sceneName in sceneNames)
                {
                    try
                    {
                        var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(sceneName);
                        
                        if (xmlDoc != null)
                        {
                            string fileName = $"Hierarchy_{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.xml";
                            SaveXmlToFile(xmlDoc, fileName);
                            Debug.Log("[EzGame.SnapShoot] " + $"场景 '{sceneName}' 层级已导出: {fileName}");
                            successCount++;
                        }
                        else
                        {
                            Debug.LogWarning("[EzGame.SnapShoot] " + $"跳过场景 '{sceneName}'：无法获取层级结构");
                            failCount++;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("[EzGame.SnapShoot] " + $"导出场景 '{sceneName}' 失败: {e.Message}");
                        failCount++;
                    }
                }
                
                // 显示结果摘要
                string message = $"批量导出完成!\n\n成功: {successCount} 个场景\n失败: {failCount} 个场景";
                EditorUtility.DisplayDialog("批量导出结果", message, "确定");
                
                Debug.Log("[EzGame.SnapShoot] " + $"批量导出完成 - 成功: {successCount}, 失败: {failCount}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"批量导出失败: {e.Message}");
                EditorUtility.DisplayDialog("批量导出失败", $"批量导出时发生错误:\n{e.Message}", "确定");
            }
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
