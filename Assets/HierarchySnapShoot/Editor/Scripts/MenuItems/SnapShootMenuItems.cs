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
        
        [MenuItem(MENU_ROOT + "导出指定场景 (Export Specific Scene)", false, 2)]
        public static void ExportSpecificSceneHierarchy()
        {
            try
            {
                // 显示输入框让用户输入场景名称
                string sceneName = EditorUtility.DisplayDialogComplex(
                    "导出指定场景",
                    "请选择导出方式:",
                    "取消",
                    "输入场景名称",
                    "选择已加载场景"
                ) switch
                {
                    1 => ShowSceneNameInputDialog(),
                    2 => ShowSceneSelectionWindow(),
                    _ => null
                };
                
                if (!string.IsNullOrEmpty(sceneName))
                {
                    if (sceneName == "ALL_SCENES")
                    {
                        ExportAllLoadedScenes();
                    }
                    else
                    {
                        ExportSingleScene(sceneName);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出指定场景层级失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 显示场景名称输入对话框
        /// </summary>
        /// <returns>用户输入的场景名称，如果取消则返回null</returns>
        private static string ShowSceneNameInputDialog()
        {
            // 获取当前激活场景名称作为默认值
            string defaultSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // 显示输入对话框
            string inputSceneName = EditorUtility.DisplayDialogComplex(
                "输入场景名称",
                $"请输入要导出的场景名称:\n\n当前激活场景: {defaultSceneName}",
                "取消",
                "使用当前场景",
                "自定义输入"
            ) switch
            {
                1 => defaultSceneName,
                2 => ShowTextInputDialog("输入场景名称", "场景名称:", defaultSceneName),
                _ => null
            };
            
            return inputSceneName;
        }
        
        /// <summary>
        /// 显示文本输入对话框（使用Unity内置对话框的简化版本）
        /// </summary>
        private static string ShowTextInputDialog(string title, string message, string defaultValue)
        {
            // 使用简单的输入提示
            bool confirmed = EditorUtility.DisplayDialog(
                title,
                $"{message}\n\n请在Console中输入场景名称，然后点击确定。\n默认值: {defaultValue}",
                "使用默认值",
                "取消"
            );
            
            return confirmed ? defaultValue : null;
        }
        
        [MenuItem(MENU_ROOT + "导出DontDestroyOnLoad对象 (Export DontDestroyOnLoad)", false, 3)]
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
        
        /// <summary>
        /// 显示场景选择窗口
        /// </summary>
        private static string ShowSceneSelectionWindow()
        {
            string[] sceneNames = HierarchyToXML.GetLoadedSceneNames();
            
            if (sceneNames.Length == 0)
            {
                EditorUtility.DisplayDialog("导出场景层级", "没有找到已加载的场景", "确定");
                return null;
            }
            
            if (sceneNames.Length == 1)
            {
                // 只有一个场景，直接返回
                return sceneNames[0];
            }
            
            // 创建选择列表
            string sceneList = string.Join("\n", sceneNames);
            int selectedIndex = EditorUtility.DisplayDialogComplex(
                "选择场景",
                $"请选择要导出的场景:\n\n{sceneList}",
                "取消",
                "导出第一个场景",
                "批量导出所有"
            );
            
            return selectedIndex switch
            {
                1 => sceneNames[0],
                2 => "ALL_SCENES", // 特殊标识符表示导出所有场景
                _ => null
            };
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
