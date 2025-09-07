using System.Collections.Generic;
using UnityEngine;

namespace EzGame.Shared
{
    /// <summary>
    /// 插件管理器
    /// 负责管理所有EzGame插件的注册、初始化和通信
    /// </summary>
    public static class PluginManager
    {
        private static readonly Dictionary<string, PluginInfo> _registeredPlugins = new Dictionary<string, PluginInfo>();
        private static bool _isInitialized = false;
        
        /// <summary>
        /// 插件信息结构
        /// </summary>
        public struct PluginInfo
        {
            public string Name;
            public string Version;
            public System.DateTime RegisterTime;
            public bool IsActive;
        }
        
        /// <summary>
        /// 插件注册事件
        /// </summary>
        public static event System.Action<string, string> OnPluginRegistered;
        
        /// <summary>
        /// 插件数据共享事件
        /// </summary>
        public static event System.Action<string, string, object> OnDataShared;
        
        /// <summary>
        /// 初始化插件管理器
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_isInitialized) return;
            
            _isInitialized = true;
            PluginLogger.Log("PluginManager", "插件管理器已初始化");
        }
        
        /// <summary>
        /// 注册插件
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <param name="version">插件版本</param>
        public static void RegisterPlugin(string pluginName, string version)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                PluginLogger.LogError("PluginManager", "插件名称不能为空");
                return;
            }
            
            var pluginInfo = new PluginInfo
            {
                Name = pluginName,
                Version = version ?? "Unknown",
                RegisterTime = System.DateTime.Now,
                IsActive = true
            };
            
            _registeredPlugins[pluginName] = pluginInfo;
            
            PluginLogger.Log("PluginManager", $"插件已注册: {pluginName} v{version}");
            OnPluginRegistered?.Invoke(pluginName, version);
        }
        
        /// <summary>
        /// 注销插件
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        public static void UnregisterPlugin(string pluginName)
        {
            if (_registeredPlugins.ContainsKey(pluginName))
            {
                _registeredPlugins.Remove(pluginName);
                PluginLogger.Log("PluginManager", $"插件已注销: {pluginName}");
            }
        }
        
        /// <summary>
        /// 检查插件是否已注册
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <returns>是否已注册</returns>
        public static bool IsPluginRegistered(string pluginName)
        {
            return _registeredPlugins.ContainsKey(pluginName);
        }
        
        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <returns>插件信息，如果不存在返回null</returns>
        public static PluginInfo? GetPluginInfo(string pluginName)
        {
            if (_registeredPlugins.TryGetValue(pluginName, out var info))
            {
                return info;
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有已注册的插件
        /// </summary>
        /// <returns>插件信息字典</returns>
        public static Dictionary<string, PluginInfo> GetAllPlugins()
        {
            return new Dictionary<string, PluginInfo>(_registeredPlugins);
        }
        
        /// <summary>
        /// 插件间数据共享
        /// </summary>
        /// <param name="fromPlugin">发送插件名称</param>
        /// <param name="dataKey">数据键</param>
        /// <param name="data">数据对象</param>
        public static void ShareData(string fromPlugin, string dataKey, object data)
        {
            PluginLogger.Log("PluginManager", $"数据共享: {fromPlugin} -> {dataKey}");
            OnDataShared?.Invoke(fromPlugin, dataKey, data);
        }
        
        /// <summary>
        /// 设置插件激活状态
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <param name="isActive">是否激活</param>
        public static void SetPluginActive(string pluginName, bool isActive)
        {
            if (_registeredPlugins.TryGetValue(pluginName, out var info))
            {
                info.IsActive = isActive;
                _registeredPlugins[pluginName] = info;
                
                string status = isActive ? "激活" : "停用";
                PluginLogger.Log("PluginManager", $"插件{status}: {pluginName}");
            }
        }
    }
}
