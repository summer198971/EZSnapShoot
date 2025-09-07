using UnityEngine;
using UnityEditor;
using System.IO;

namespace EzGame.SnapShoot.Editor
{
    public class SnapShootSettingsWindow : EditorWindow
    {
        private const string PREF_EXPORT_PATH = "EzGame.SnapShoot.ExportPath";
        private const string PREF_AUTO_TIMESTAMP = "EzGame.SnapShoot.AutoTimestamp";
        private const string PREF_INCLUDE_INACTIVE = "EzGame.SnapShoot.IncludeInactive";
        
        private string _exportPath;
        private bool _autoTimestamp;
        private bool _includeInactive;
        
        [MenuItem("EzGame/SnapShoot/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<SnapShootSettingsWindow>("SnapShoot Settings");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        
        private void OnEnable()
        {
            LoadSettings();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("EZ SnapShoot Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 导出路径设置
            EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Export Path:", GUILayout.Width(100));
            _exportPath = EditorGUILayout.TextField(_exportPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Export Folder", _exportPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _exportPath = FileUtil.GetProjectRelativePath(selectedPath);
                    if (string.IsNullOrEmpty(_exportPath))
                    {
                        _exportPath = selectedPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 其他设置
            _autoTimestamp = EditorGUILayout.Toggle("Auto Add Timestamp", _autoTimestamp);
            _includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", _includeInactive);
            
            EditorGUILayout.Space();
            
            // 按钮区域
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to Default"))
            {
                ResetToDefault();
            }
            if (GUILayout.Button("Save Settings"))
            {
                SaveSettings();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 信息区域
            EditorGUILayout.LabelField("Plugin Information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Version: 1.0.0");
            EditorGUILayout.LabelField("Author: EzGame");
            
            if (GUILayout.Button("Open Export Folder"))
            {
                string fullPath = Path.Combine(Application.dataPath, "..", _exportPath);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                EditorUtility.RevealInFinder(fullPath);
            }
        }
        
        private void LoadSettings()
        {
            _exportPath = EditorPrefs.GetString(PREF_EXPORT_PATH, "Assets/SnapShoot_Exports/");
            _autoTimestamp = EditorPrefs.GetBool(PREF_AUTO_TIMESTAMP, true);
            _includeInactive = EditorPrefs.GetBool(PREF_INCLUDE_INACTIVE, false);
        }
        
        private void SaveSettings()
        {
            EditorPrefs.SetString(PREF_EXPORT_PATH, _exportPath);
            EditorPrefs.SetBool(PREF_AUTO_TIMESTAMP, _autoTimestamp);
            EditorPrefs.SetBool(PREF_INCLUDE_INACTIVE, _includeInactive);
            
            Debug.Log("[EzGame.SnapShoot] Settings saved successfully.");
        }
        
        private void ResetToDefault()
        {
            _exportPath = "Assets/SnapShoot_Exports/";
            _autoTimestamp = true;
            _includeInactive = false;
        }
    }
}
