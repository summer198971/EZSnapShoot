using UnityEngine;
using UnityEditor;
using System.IO;

namespace EzGame.Shared.Editor
{
    /// <summary>
    /// 编辑器工具类
    /// 提供常用的编辑器功能和工具方法
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// 创建目录（如果不存在）
        /// </summary>
        /// <param name="path">目录路径</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        /// <summary>
        /// 获取项目相对路径
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <returns>相对于项目根目录的路径</returns>
        public static string GetProjectRelativePath(string fullPath)
        {
            string projectPath = Application.dataPath.Replace("/Assets", "");
            if (fullPath.StartsWith(projectPath))
            {
                return fullPath.Substring(projectPath.Length + 1);
            }
            return fullPath;
        }
        
        /// <summary>
        /// 在Inspector中绘制分隔线
        /// </summary>
        /// <param name="height">线条高度</param>
        /// <param name="color">线条颜色</param>
        public static void DrawSeparatorLine(float height = 1f, Color? color = null)
        {
            Color lineColor = color ?? Color.gray;
            
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, lineColor);
        }
        
        /// <summary>
        /// 绘制标题
        /// </summary>
        /// <param name="title">标题文本</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="bold">是否加粗</param>
        public static void DrawTitle(string title, int fontSize = 16, bool bold = true)
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = fontSize,
                fontStyle = bold ? FontStyle.Bold : FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField(title, titleStyle);
        }
        
        /// <summary>
        /// 绘制带背景的区域
        /// </summary>
        /// <param name="drawAction">绘制内容的回调</param>
        /// <param name="backgroundColor">背景颜色</param>
        /// <param name="padding">内边距</param>
        public static void DrawBackgroundArea(System.Action drawAction, Color? backgroundColor = null, int padding = 10)
        {
            Color bgColor = backgroundColor ?? new Color(0.8f, 0.8f, 0.8f, 0.3f);
            
            EditorGUILayout.BeginVertical();
            
            Rect backgroundRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(backgroundRect, bgColor);
            
            GUILayout.Space(padding);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(padding);
            
            EditorGUILayout.BeginVertical();
            drawAction?.Invoke();
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(padding);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(padding);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 显示进度条
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="info">信息</param>
        /// <param name="progress">进度（0-1）</param>
        /// <param name="cancellable">是否可取消</param>
        /// <returns>是否被取消</returns>
        public static bool DisplayProgressBar(string title, string info, float progress, bool cancellable = false)
        {
            if (cancellable)
            {
                return EditorUtility.DisplayCancelableProgressBar(title, info, progress);
            }
            else
            {
                EditorUtility.DisplayProgressBar(title, info, progress);
                return false;
            }
        }
        
        /// <summary>
        /// 清除进度条
        /// </summary>
        public static void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }
        
        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <param name="ok">确定按钮文本</param>
        /// <param name="cancel">取消按钮文本</param>
        /// <returns>用户是否点击了确定</returns>
        public static bool ShowDialog(string title, string message, string ok = "确定", string cancel = "取消")
        {
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        }
        
        /// <summary>
        /// 刷新AssetDatabase
        /// </summary>
        public static void RefreshAssetDatabase()
        {
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 保存所有资源
        /// </summary>
        public static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// 在文件浏览器中显示文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void RevealInFinder(string path)
        {
            EditorUtility.RevealInFinder(path);
        }
    }
}
