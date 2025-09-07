using UnityEngine;

namespace EzGame.Shared.Extensions
{
    /// <summary>
    /// GameObject扩展方法
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 获取或添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标GameObject</param>
        /// <returns>组件实例</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
        
        /// <summary>
        /// 安全销毁GameObject
        /// </summary>
        /// <param name="gameObject">要销毁的GameObject</param>
        /// <param name="delay">延迟时间</param>
        public static void SafeDestroy(this GameObject gameObject, float delay = 0f)
        {
            if (gameObject != null)
            {
                if (Application.isPlaying)
                {
                    if (delay > 0f)
                    {
                        Object.Destroy(gameObject, delay);
                    }
                    else
                    {
                        Object.Destroy(gameObject);
                    }
                }
                else
                {
                    Object.DestroyImmediate(gameObject);
                }
            }
        }
        
        /// <summary>
        /// 设置层级（包括所有子对象）
        /// </summary>
        /// <param name="gameObject">目标GameObject</param>
        /// <param name="layer">目标层级</param>
        /// <param name="includeChildren">是否包括子对象</param>
        public static void SetLayerRecursively(this GameObject gameObject, int layer, bool includeChildren = true)
        {
            gameObject.layer = layer;
            
            if (includeChildren)
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.SetLayerRecursively(layer, true);
                }
            }
        }
        
        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="gameObject">目标GameObject</param>
        /// <returns>从根对象到当前对象的完整路径</returns>
        public static string GetFullPath(this GameObject gameObject)
        {
            string path = gameObject.name;
            Transform parent = gameObject.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        /// <summary>
        /// 检查是否在指定层级
        /// </summary>
        /// <param name="gameObject">目标GameObject</param>
        /// <param name="layerMask">层级掩码</param>
        /// <returns>是否在指定层级</returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            return (layerMask.value & (1 << gameObject.layer)) != 0;
        }
        
        /// <summary>
        /// 查找子对象（支持路径）
        /// </summary>
        /// <param name="gameObject">父对象</param>
        /// <param name="path">子对象路径，如 "Child/SubChild"</param>
        /// <returns>找到的子对象，如果不存在返回null</returns>
        public static GameObject FindChildByPath(this GameObject gameObject, string path)
        {
            Transform child = gameObject.transform.Find(path);
            return child != null ? child.gameObject : null;
        }
    }
}
