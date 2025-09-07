using UnityEngine;

namespace EzGame.Shared
{
    /// <summary>
    /// 插件统一日志管理器
    /// 提供统一的日志输出格式和级别控制
    /// </summary>
    public static class PluginLogger
    {
        private static readonly string LOG_PREFIX = "[EzGame]";
        
        /// <summary>
        /// 日志级别枚举
        /// </summary>
        public enum LogLevel
        {
            Log = 0,
            Warning = 1,
            Error = 2
        }
        
        /// <summary>
        /// 当前日志级别，低于此级别的日志不会输出
        /// </summary>
        public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Log;
        
        /// <summary>
        /// 输出普通日志
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <param name="message">日志消息</param>
        public static void Log(string pluginName, string message)
        {
            if (CurrentLogLevel <= LogLevel.Log)
            {
                Debug.Log($"{LOG_PREFIX}[{pluginName}] {message}");
            }
        }
        
        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <param name="message">警告消息</param>
        public static void LogWarning(string pluginName, string message)
        {
            if (CurrentLogLevel <= LogLevel.Warning)
            {
                Debug.LogWarning($"{LOG_PREFIX}[{pluginName}] {message}");
            }
        }
        
        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <param name="message">错误消息</param>
        public static void LogError(string pluginName, string message)
        {
            if (CurrentLogLevel <= LogLevel.Error)
            {
                Debug.LogError($"{LOG_PREFIX}[{pluginName}] {message}");
            }
        }
        
        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <param name="exception">异常对象</param>
        public static void LogException(string pluginName, System.Exception exception)
        {
            if (CurrentLogLevel <= LogLevel.Error)
            {
                Debug.LogError($"{LOG_PREFIX}[{pluginName}] Exception: {exception.Message}");
                Debug.LogException(exception);
            }
        }
    }
}
