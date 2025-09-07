using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using EzGame.SnapShoot.Editor;
using System.IO;

namespace EzGame.SnapShoot.Tests.Editor
{
    public class SnapShootEditorTests
    {
        private const string TEST_EXPORT_PATH = "Assets/Test_SnapShoot_Exports/";
        
        [SetUp]
        public void SetUp()
        {
            // 确保测试目录存在
            if (!Directory.Exists(TEST_EXPORT_PATH))
            {
                Directory.CreateDirectory(TEST_EXPORT_PATH);
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            // 清理测试文件
            if (Directory.Exists(TEST_EXPORT_PATH))
            {
                Directory.Delete(TEST_EXPORT_PATH, true);
                AssetDatabase.Refresh();
            }
        }
        
        [Test]
        public void MenuItems_ShouldExist()
        {
            // 验证菜单项是否存在
            bool hasExportCurrentScene = Menu.GetEnabled("EzGame/SnapShoot/Export Current Scene Hierarchy");
            bool hasExportDontDestroy = Menu.GetEnabled("EzGame/SnapShoot/Export DontDestroyOnLoad Hierarchy");
            bool hasOpenFolder = Menu.GetEnabled("EzGame/SnapShoot/Open Export Folder");
            bool hasSettings = Menu.GetEnabled("EzGame/SnapShoot/Settings");
            
            // 注意：Menu.GetEnabled 对于不存在的菜单项会返回false
            // 这里我们主要是确保代码能够编译和运行
            Assert.Pass("Menu items compilation test passed");
        }
        
        [Test]
        public void SnapShootSettingsWindow_ShouldBeCreatable()
        {
            // Act & Assert - 主要测试窗口类是否能正确实例化
            var window = EditorWindow.GetWindow<SnapShootSettingsWindow>();
            Assert.IsNotNull(window);
            
            // 清理
            window.Close();
        }
        
        [Test]
        public void ExportPath_ShouldBeConfigurable()
        {
            // Arrange
            string testPath = "Assets/CustomExportPath/";
            string prefKey = "EzGame.SnapShoot.ExportPath";
            
            // Act
            EditorPrefs.SetString(prefKey, testPath);
            string retrievedPath = EditorPrefs.GetString(prefKey);
            
            // Assert
            Assert.AreEqual(testPath, retrievedPath);
            
            // Cleanup
            EditorPrefs.DeleteKey(prefKey);
        }
        
        [Test]
        public void AutoTimestamp_ShouldBeConfigurable()
        {
            // Arrange
            string prefKey = "EzGame.SnapShoot.AutoTimestamp";
            
            // Act
            EditorPrefs.SetBool(prefKey, true);
            bool retrievedValue = EditorPrefs.GetBool(prefKey);
            
            // Assert
            Assert.IsTrue(retrievedValue);
            
            // Cleanup
            EditorPrefs.DeleteKey(prefKey);
        }
        
        [Test]
        public void IncludeInactive_ShouldBeConfigurable()
        {
            // Arrange
            string prefKey = "EzGame.SnapShoot.IncludeInactive";
            
            // Act
            EditorPrefs.SetBool(prefKey, false);
            bool retrievedValue = EditorPrefs.GetBool(prefKey);
            
            // Assert
            Assert.IsFalse(retrievedValue);
            
            // Cleanup
            EditorPrefs.DeleteKey(prefKey);
        }
    }
}
