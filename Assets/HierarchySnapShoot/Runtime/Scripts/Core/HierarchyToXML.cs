using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Text;


namespace EzGame.SnapShoot
{
    /// <summary>
    /// 导出配置选项
    /// </summary>
    public struct ExportOptions
    {
        public bool IncludeTransform;
        public bool IncludeComponents;
        public bool IncludeMaterials;
        public bool IncludeInactiveObjects;
        public bool IncludeChildObjects;
        public int MaxDepth;
        
        public static ExportOptions Default => new ExportOptions
        {
            IncludeTransform = true,
            IncludeComponents = true,
            IncludeMaterials = true,
            IncludeInactiveObjects = false,
            IncludeChildObjects = true,
            MaxDepth = -1 // 无限制
        };
    }
    
    public class HierarchyToXML
    {
        public static StringWriter GetDontDestroyOnLoadHierarchyToStr()
        {
            XmlDocument xml = GetDontDestroyOnLoadHierarchyToXML();
            StringWriter sw = new StringWriter();
            xml.Save(sw);
            return sw;
        }
        public static XmlDocument GetDontDestroyOnLoadHierarchyToXML()
        {
            return GetDontDestroyOnLoadHierarchyToXML(ExportOptions.Default);
        }
        
        /// <summary>
        /// 获取DontDestroyOnLoad对象层级结构的XML文档（带配置选项）
        /// </summary>
        /// <param name="options">导出配置选项</param>
        /// <returns>包含DontDestroyOnLoad对象层级结构的XML文档</returns>
        public static XmlDocument GetDontDestroyOnLoadHierarchyToXML(ExportOptions options)
        {
            try
            {
                Debug.Log("[EzGame.SnapShoot] " + "开始导出DontDestroyOnLoad对象层级结构");
                
                // 创建XmlDocument对象
                XmlDocument xmlDoc = new XmlDocument();

                // 创建根元素
                XmlElement root = xmlDoc.CreateElement("Hierarchy");
                root.SetAttribute("exportTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                root.SetAttribute("unityVersion", Application.unityVersion);
                xmlDoc.AppendChild(root);

                // 创建场景元素
                XmlElement sceneElement = xmlDoc.CreateElement("Scene");
                sceneElement.SetAttribute("name", "DontDestroyOnLoad");
                sceneElement.SetAttribute("active", "true");
                sceneElement.SetAttribute("path", "");
                root.AppendChild(sceneElement);
                
                GameObject[] DontDestroyOnLoad = getDontDestroyOnLoadGameObjects();
                Debug.Log("[EzGame.SnapShoot] " + $"找到 {DontDestroyOnLoad.Length} 个DontDestroyOnLoad对象");
                
                foreach (GameObject rootObject in DontDestroyOnLoad)
                {
                    AppendGameObject(sceneElement, rootObject, xmlDoc, options, 0);
                }

                Debug.Log("[EzGame.SnapShoot] " + "DontDestroyOnLoad对象层级结构导出完成");
                return xmlDoc;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出DontDestroyOnLoad对象层级结构时发生错误: {ex.Message}");
                throw;
            }
        }
        public static XmlDocument GetCurrentSceneLoadedHierarchyToXML()
        {
            return GetCurrentSceneLoadedHierarchyToXML(ExportOptions.Default);
        }
        
        /// <summary>
        /// 获取当前已加载场景的层级结构的XML文档（包括所有已加载场景和DontDestroyOnLoad对象）
        /// </summary>
        /// <param name="options">导出配置选项</param>
        /// <returns>包含所有已加载场景层级结构的XML文档</returns>
        public static XmlDocument GetCurrentSceneLoadedHierarchyToXML(ExportOptions options)
        {
            try
            {
                Debug.Log("[EzGame.SnapShoot] " + "开始导出当前已加载场景的层级结构");
                
                // 创建XmlDocument对象
                XmlDocument xmlDoc = new XmlDocument();

                // 创建根元素
                XmlElement root = xmlDoc.CreateElement("Hierarchy");
                root.SetAttribute("exportTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                root.SetAttribute("unityVersion", Application.unityVersion);
                xmlDoc.AppendChild(root);

                // 获取当前激活的场景
                var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

                // 创建场景元素
                XmlElement sceneElement = xmlDoc.CreateElement("Scene");
                sceneElement.SetAttribute("name", activeScene.name);
                sceneElement.SetAttribute("active", "true");
                sceneElement.SetAttribute("path", activeScene.path);
                root.AppendChild(sceneElement);
                
                // 获取所有根对象
                GameObject[] rootObjects = activeScene.GetRootGameObjects();
                Debug.Log("[EzGame.SnapShoot] " + $"当前场景 '{activeScene.name}' 包含 {rootObjects.Length} 个根对象");

                // 遍历每个根对象
                foreach (GameObject rootObject in rootObjects)
                {
                    AppendGameObject(sceneElement, rootObject, xmlDoc, options, 0);
                }

                // 获取所有已加载但未激活的场景
                int sceneCount = SceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    
                    // 跳过已经处理的激活场景
                    if (scene == activeScene)
                        continue;
                        
                    // 只处理已加载的场景
                    if (scene.isLoaded)
                    {
                        XmlElement inactiveSceneElement = xmlDoc.CreateElement("Scene");
                        inactiveSceneElement.SetAttribute("name", scene.name);
                        inactiveSceneElement.SetAttribute("active", "false");
                        inactiveSceneElement.SetAttribute("path", scene.path);
                        root.AppendChild(inactiveSceneElement);
                        
                        // 获取该场景的所有根对象
                        GameObject[] inactiveSceneRootObjects = scene.GetRootGameObjects();
                        Debug.Log("[EzGame.SnapShoot] " + $"非激活场景 '{scene.name}' 包含 {inactiveSceneRootObjects.Length} 个根对象");
                        
                        // 遍历每个根对象
                        foreach (GameObject rootObject in inactiveSceneRootObjects)
                        {
                            AppendGameObject(inactiveSceneElement, rootObject, xmlDoc, options, 0);
                        }
                    }
                }

                // 添加DontDestroyOnLoad场景
                XmlElement dontDestroyElement = xmlDoc.CreateElement("Scene");
                dontDestroyElement.SetAttribute("name", "DontDestroyOnLoad");
                dontDestroyElement.SetAttribute("active", "true");  // DontDestroyOnLoad总是激活的
                dontDestroyElement.SetAttribute("path", "");
                root.AppendChild(dontDestroyElement);
                
                GameObject[] DontDestroyOnLoad = getDontDestroyOnLoadGameObjects();
                Debug.Log("[EzGame.SnapShoot] " + $"DontDestroyOnLoad 包含 {DontDestroyOnLoad.Length} 个对象");
                
                foreach (GameObject rootObject in DontDestroyOnLoad)
                {
                    AppendGameObject(dontDestroyElement, rootObject, xmlDoc, options, 0);
                }

                Debug.Log("[EzGame.SnapShoot] " + "已加载场景的层级结构导出完成");
                return xmlDoc;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出已加载场景的层级结构时发生错误: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取指定场景的层级结构的XML文档
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>包含指定场景层级结构的XML文档，如果场景未找到则返回null</returns>
        public static XmlDocument GetSpecificSceneHierarchyToXML(string sceneName)
        {
            return GetSpecificSceneHierarchyToXML(sceneName, ExportOptions.Default);
        }
        
        /// <summary>
        /// 获取指定场景的层级结构的XML文档（包含DontDestroyOnLoad对象，带配置选项）
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="options">导出配置选项</param>
        /// <returns>包含指定场景层级结构的XML文档，如果场景未找到则返回null</returns>
        public static XmlDocument GetSpecificSceneHierarchyToXML(string sceneName, ExportOptions options)
        {
            try
            {
                Debug.Log("[EzGame.SnapShoot] " + $"开始导出指定场景 '{sceneName}' 的层级结构（包含DontDestroyOnLoad）");
                
                // 创建XmlDocument对象
                XmlDocument xmlDoc = new XmlDocument();

                // 创建根元素
                XmlElement root = xmlDoc.CreateElement("Hierarchy");
                root.SetAttribute("exportTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                root.SetAttribute("unityVersion", Application.unityVersion);
                root.SetAttribute("targetScene", sceneName);
                xmlDoc.AppendChild(root);

                // 查找并导出指定场景
                bool sceneFound = false;
                
                // 检查是否是DontDestroyOnLoad场景
                if (sceneName == "DontDestroyOnLoad")
                {
                    XmlElement dontDestroyElement = xmlDoc.CreateElement("Scene");
                    dontDestroyElement.SetAttribute("name", "DontDestroyOnLoad");
                    dontDestroyElement.SetAttribute("active", "true");
                    dontDestroyElement.SetAttribute("path", "");
                    root.AppendChild(dontDestroyElement);
                    
                    GameObject[] dontDestroyObjects = getDontDestroyOnLoadGameObjects();
                    Debug.Log("[EzGame.SnapShoot] " + $"DontDestroyOnLoad 包含 {dontDestroyObjects.Length} 个对象");
                    
                    foreach (GameObject rootObject in dontDestroyObjects)
                    {
                        AppendGameObject(dontDestroyElement, rootObject, xmlDoc, options, 0);
                    }
                    sceneFound = true;
                }
                else
                {
                    // 在所有已加载的场景中查找
                    int sceneCount = SceneManager.sceneCount;
                    for (int i = 0; i < sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);
                        if (scene.name == sceneName && scene.isLoaded)
                        {
                            // 创建指定场景元素
                            XmlElement sceneElement = xmlDoc.CreateElement("Scene");
                            sceneElement.SetAttribute("name", scene.name);
                            sceneElement.SetAttribute("active", (scene == SceneManager.GetActiveScene()).ToString());
                            sceneElement.SetAttribute("path", scene.path);
                            root.AppendChild(sceneElement);
                            
                            // 获取场景的所有根对象
                            GameObject[] rootObjects = scene.GetRootGameObjects();
                            Debug.Log("[EzGame.SnapShoot] " + $"场景 '{sceneName}' 包含 {rootObjects.Length} 个根对象");

                            // 遍历每个根对象
                            foreach (GameObject rootObject in rootObjects)
                            {
                                AppendGameObject(sceneElement, rootObject, xmlDoc, options, 0);
                            }
                            
                            sceneFound = true;
                            break;
                        }
                    }
                }
                
                if (!sceneFound)
                {
                    Debug.LogWarning("[EzGame.SnapShoot] " + $"未找到已加载的场景: '{sceneName}'");
                    return null;
                }
                
                // 始终添加DontDestroyOnLoad场景（除非指定场景本身就是DontDestroyOnLoad）
                if (sceneName != "DontDestroyOnLoad")
                {
                    XmlElement dontDestroyElement = xmlDoc.CreateElement("Scene");
                    dontDestroyElement.SetAttribute("name", "DontDestroyOnLoad");
                    dontDestroyElement.SetAttribute("active", "true");
                    dontDestroyElement.SetAttribute("path", "");
                    root.AppendChild(dontDestroyElement);
                    
                    GameObject[] dontDestroyObjects = getDontDestroyOnLoadGameObjects();
                    Debug.Log("[EzGame.SnapShoot] " + $"DontDestroyOnLoad 包含 {dontDestroyObjects.Length} 个对象");
                    
                    foreach (GameObject rootObject in dontDestroyObjects)
                    {
                        AppendGameObject(dontDestroyElement, rootObject, xmlDoc, options, 0);
                    }
                }

                Debug.Log("[EzGame.SnapShoot] " + $"场景 '{sceneName}' 的层级结构导出完成（包含DontDestroyOnLoad）");
                return xmlDoc;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出场景 '{sceneName}' 的层级结构时发生错误: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 获取指定场景的层级结构的XML文档（通过场景索引）
        /// </summary>
        /// <param name="sceneIndex">场景在SceneManager中的索引</param>
        /// <returns>包含指定场景层级结构的XML文档，如果索引无效则返回null</returns>
        public static XmlDocument GetSpecificSceneHierarchyToXML(int sceneIndex)
        {
            return GetSpecificSceneHierarchyToXML(sceneIndex, ExportOptions.Default);
        }
        
        /// <summary>
        /// 获取指定场景的层级结构的XML文档（通过场景索引，带配置选项）
        /// </summary>
        /// <param name="sceneIndex">场景在SceneManager中的索引</param>
        /// <param name="options">导出配置选项</param>
        /// <returns>包含指定场景层级结构的XML文档，如果索引无效则返回null</returns>
        public static XmlDocument GetSpecificSceneHierarchyToXML(int sceneIndex, ExportOptions options)
        {
            try
            {
                if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCount)
                {
                    Debug.LogWarning("[EzGame.SnapShoot] " + $"场景索引 {sceneIndex} 无效，当前已加载场景数量: {SceneManager.sceneCount}");
                    return null;
                }
                
                Scene targetScene = SceneManager.GetSceneAt(sceneIndex);
                if (!targetScene.isLoaded)
                {
                    Debug.LogWarning("[EzGame.SnapShoot] " + $"索引 {sceneIndex} 对应的场景 '{targetScene.name}' 未加载");
                    return null;
                }
                
                return GetSpecificSceneHierarchyToXML(targetScene.name, options);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"通过索引 {sceneIndex} 导出场景层级结构时发生错误: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 获取所有已加载场景的名称列表
        /// </summary>
        /// <returns>已加载场景名称的数组，包括DontDestroyOnLoad</returns>
        public static string[] GetLoadedSceneNames()
        {
            var sceneNames = new System.Collections.Generic.List<string>();
            
            // 添加所有已加载的场景
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    sceneNames.Add(scene.name);
                }
            }
            
            // 检查是否有DontDestroyOnLoad对象
            GameObject[] dontDestroyObjects = getDontDestroyOnLoadGameObjects();
            if (dontDestroyObjects.Length > 0)
            {
                sceneNames.Add("DontDestroyOnLoad");
            }
            
            return sceneNames.ToArray();
        }
        
        /// <summary>
        /// 获取指定场景的层级结构XML字符串（包含DontDestroyOnLoad对象）
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>XML字符串，如果场景未找到则返回null</returns>
        public static string GetSpecificSceneHierarchyToXMLString(string sceneName)
        {
            return GetSpecificSceneHierarchyToXMLString(sceneName, ExportOptions.Default);
        }
        
        /// <summary>
        /// 获取指定场景的层级结构XML字符串（包含DontDestroyOnLoad对象，带配置选项）
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="options">导出配置选项</param>
        /// <returns>XML字符串，如果场景未找到则返回null</returns>
        public static string GetSpecificSceneHierarchyToXMLString(string sceneName, ExportOptions options)
        {
            try
            {
                Debug.Log("[EzGame.SnapShoot] " + $"开始导出指定场景 '{sceneName}' 的层级结构（包含DontDestroyOnLoad）");
                
                // 创建XmlDocument对象
                XmlDocument xmlDoc = new XmlDocument();

                // 创建根元素
                XmlElement root = xmlDoc.CreateElement("Hierarchy");
                root.SetAttribute("exportTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                root.SetAttribute("unityVersion", Application.unityVersion);
                root.SetAttribute("targetScene", sceneName);
                xmlDoc.AppendChild(root);

                // 查找并导出指定场景
                bool sceneFound = false;
                
                // 检查是否是DontDestroyOnLoad场景
                if (sceneName == "DontDestroyOnLoad")
                {
                    XmlElement dontDestroyElement = xmlDoc.CreateElement("Scene");
                    dontDestroyElement.SetAttribute("name", "DontDestroyOnLoad");
                    dontDestroyElement.SetAttribute("active", "true");
                    dontDestroyElement.SetAttribute("path", "");
                    root.AppendChild(dontDestroyElement);
                    
                    GameObject[] dontDestroyObjects = getDontDestroyOnLoadGameObjects();
                    Debug.Log("[EzGame.SnapShoot] " + $"DontDestroyOnLoad 包含 {dontDestroyObjects.Length} 个对象");
                    
                    foreach (GameObject rootObject in dontDestroyObjects)
                    {
                        AppendGameObject(dontDestroyElement, rootObject, xmlDoc, options, 0);
                    }
                    sceneFound = true;
                }
                else
                {
                    // 在所有已加载的场景中查找
                    int sceneCount = SceneManager.sceneCount;
                    for (int i = 0; i < sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);
                        if (scene.name == sceneName && scene.isLoaded)
                        {
                            // 创建指定场景元素
                            XmlElement sceneElement = xmlDoc.CreateElement("Scene");
                            sceneElement.SetAttribute("name", scene.name);
                            sceneElement.SetAttribute("active", (scene == SceneManager.GetActiveScene()).ToString());
                            sceneElement.SetAttribute("path", scene.path);
                            root.AppendChild(sceneElement);
                            
                            // 获取场景的所有根对象
                            GameObject[] rootObjects = scene.GetRootGameObjects();
                            Debug.Log("[EzGame.SnapShoot] " + $"场景 '{sceneName}' 包含 {rootObjects.Length} 个根对象");

                            // 遍历每个根对象
                            foreach (GameObject rootObject in rootObjects)
                            {
                                AppendGameObject(sceneElement, rootObject, xmlDoc, options, 0);
                            }
                            
                            sceneFound = true;
                            break;
                        }
                    }
                }
                
                if (!sceneFound)
                {
                    Debug.LogWarning("[EzGame.SnapShoot] " + $"未找到已加载的场景: '{sceneName}'");
                    return null;
                }
                
                // 始终添加DontDestroyOnLoad场景（除非指定场景本身就是DontDestroyOnLoad）
                if (sceneName != "DontDestroyOnLoad")
                {
                    XmlElement dontDestroyElement = xmlDoc.CreateElement("Scene");
                    dontDestroyElement.SetAttribute("name", "DontDestroyOnLoad");
                    dontDestroyElement.SetAttribute("active", "true");
                    dontDestroyElement.SetAttribute("path", "");
                    root.AppendChild(dontDestroyElement);
                    
                    GameObject[] dontDestroyObjects = getDontDestroyOnLoadGameObjects();
                    Debug.Log("[EzGame.SnapShoot] " + $"DontDestroyOnLoad 包含 {dontDestroyObjects.Length} 个对象");
                    
                    foreach (GameObject rootObject in dontDestroyObjects)
                    {
                        AppendGameObject(dontDestroyElement, rootObject, xmlDoc, options, 0);
                    }
                }

                // 转换为字符串
                using (var stringWriter = new StringWriter())
                using (var xmlWriter = new System.Xml.XmlTextWriter(stringWriter))
                {
                    xmlWriter.Formatting = System.Xml.Formatting.Indented;
                    xmlWriter.Indentation = 2;
                    xmlDoc.Save(xmlWriter);
                    
                    string result = stringWriter.ToString();
                    Debug.Log("[EzGame.SnapShoot] " + $"场景 '{sceneName}' 的层级结构导出完成（包含DontDestroyOnLoad）");
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"导出场景 '{sceneName}' 的层级结构时发生错误: {ex.Message}");
                return null;
            }
        }

        private static GameObject[] getDontDestroyOnLoadGameObjects()
        {
            var allGameObjects = new List<GameObject>();
            allGameObjects.AddRange(GameObject.FindObjectsOfType<GameObject>());
            //移除所有场景包含的对象
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var objs = scene.GetRootGameObjects();
                for (var j = 0; j < objs.Length; j++)
                {
                    allGameObjects.Remove(objs[j]);
                }
            }

            //移除父级不为null的对象
            int k = allGameObjects.Count;
            while (--k >= 0)
            {
                if (allGameObjects[k].transform.parent != null)
                {
                    allGameObjects.RemoveAt(k);
                }
            }

            return allGameObjects.ToArray();
        }

        /// <summary>
        /// 优化版本的AppendGameObject方法
        /// </summary>
        private static void AppendGameObject(XmlElement parentElement, GameObject gameObject, XmlDocument xmlDoc)
        {
            AppendGameObject(parentElement, gameObject, xmlDoc, ExportOptions.Default, 0);
        }
        
        /// <summary>
        /// 带配置选项的AppendGameObject方法
        /// </summary>
        private static void AppendGameObject(XmlElement parentElement, GameObject gameObject, XmlDocument xmlDoc, ExportOptions options, int currentDepth)
        {
            try
            {
                // 检查深度限制
                if (options.MaxDepth >= 0 && currentDepth > options.MaxDepth)
                {
                    return;
                }
                
                // 检查是否包含非激活对象
                if (!options.IncludeInactiveObjects && !gameObject.activeSelf)
                {
                    return;
                }
                
                // 创建GameObject元素并设置基本属性
                XmlElement gameObjectElement = xmlDoc.CreateElement("GameObject");
                SetGameObjectBasicAttributes(gameObjectElement, gameObject);
                
                // 添加Transform信息（如果需要）
                if (options.IncludeTransform)
                {
                    AppendTransformInfo(gameObjectElement, gameObject.transform, xmlDoc);
                }
                
                // 添加组件信息（如果需要）
                if (options.IncludeComponents)
                {
                    AppendComponentsInfo(gameObjectElement, gameObject, xmlDoc, options.IncludeMaterials);
                }
                
                // 将GameObject元素添加到父元素
                parentElement.AppendChild(gameObjectElement);
                
                // 递归处理子对象（如果需要）
                if (options.IncludeChildObjects)
                {
                    AppendChildObjects(gameObjectElement, gameObject.transform, xmlDoc, options, currentDepth + 1);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[EzGame.SnapShoot] " + $"处理GameObject '{gameObject.name}' 时发生错误: {ex.Message}");
                
                // 创建错误节点
                XmlElement errorElement = xmlDoc.CreateElement("GameObject");
                errorElement.SetAttribute("name", gameObject.name);
                errorElement.SetAttribute("error", ex.Message);
                parentElement.AppendChild(errorElement);
            }
        }
        
        /// <summary>
        /// 设置GameObject的基本属性
        /// </summary>
        private static void SetGameObjectBasicAttributes(XmlElement element, GameObject gameObject)
        {
            element.SetAttribute("name", gameObject.name ?? "null");
            element.SetAttribute("active", gameObject.activeSelf.ToString());
            element.SetAttribute("layer", LayerMask.LayerToName(gameObject.layer) ?? gameObject.layer.ToString());
            element.SetAttribute("tag", gameObject.tag ?? "Untagged");
            element.SetAttribute("childCount", gameObject.transform.childCount.ToString());
        }
        
        /// <summary>
        /// 添加Transform信息
        /// </summary>
        private static void AppendTransformInfo(XmlElement parentElement, Transform transform, XmlDocument xmlDoc)
        {
            // 使用StringBuilder优化字符串操作
            var sb = new StringBuilder();
            
            // 位置信息
            XmlElement positionElement = xmlDoc.CreateElement("Position");
            Vector3 position = transform.localPosition;
            positionElement.SetAttribute("x", FormatFloat(position.x));
            positionElement.SetAttribute("y", FormatFloat(position.y));
            positionElement.SetAttribute("z", FormatFloat(position.z));
            parentElement.AppendChild(positionElement);
            
            // 旋转信息
            XmlElement rotationElement = xmlDoc.CreateElement("Rotation");
            Vector3 rotation = transform.eulerAngles;
            rotationElement.SetAttribute("x", FormatFloat(rotation.x));
            rotationElement.SetAttribute("y", FormatFloat(rotation.y));
            rotationElement.SetAttribute("z", FormatFloat(rotation.z));
            parentElement.AppendChild(rotationElement);
            
            // 缩放信息
            XmlElement scaleElement = xmlDoc.CreateElement("Scale");
            Vector3 scale = transform.localScale;
            scaleElement.SetAttribute("x", FormatFloat(scale.x));
            scaleElement.SetAttribute("y", FormatFloat(scale.y));
            scaleElement.SetAttribute("z", FormatFloat(scale.z));
            parentElement.AppendChild(scaleElement);
        }
        
        /// <summary>
        /// 添加组件信息
        /// </summary>
        private static void AppendComponentsInfo(XmlElement parentElement, GameObject gameObject, XmlDocument xmlDoc, bool includeMaterials)
        {
            // 一次性获取所有组件，避免重复查询
            Component[] components = gameObject.GetComponents<Component>();
            if (components == null || components.Length == 0)
            {
                return;
            }
            
            XmlElement componentsElement = xmlDoc.CreateElement("Components");
            int scriptCount = 0;
            Renderer renderer = null;
            
            // 遍历组件，分类处理
            foreach (Component component in components)
            {
                if (component == null) continue;
                
                // 跳过Transform组件（已在Transform信息中处理）
                if (component is Transform) continue;
                
                // 处理Renderer组件
                if (component is Renderer rendererComponent)
                {
                    renderer = rendererComponent;
                    continue;
                }
                
                // 处理MonoBehaviour组件
                if (component is MonoBehaviour monoBehaviour)
                {
                    scriptCount++;
                    AppendMonoBehaviourInfo(componentsElement, monoBehaviour, xmlDoc);
                }
                else
                {
                    // 处理其他组件
                    AppendComponentInfo(componentsElement, component, xmlDoc);
                }
            }
            
            // 处理Renderer和材质信息
            if (renderer != null)
            {
                AppendRendererInfo(componentsElement, renderer, xmlDoc, includeMaterials);
            }
            
            componentsElement.SetAttribute("scriptCount", scriptCount.ToString());
            componentsElement.SetAttribute("totalCount", components.Length.ToString());
            parentElement.AppendChild(componentsElement);
        }
        
        /// <summary>
        /// 添加MonoBehaviour组件信息
        /// </summary>
        private static void AppendMonoBehaviourInfo(XmlElement parentElement, MonoBehaviour monoBehaviour, XmlDocument xmlDoc)
        {
            XmlElement componentElement = xmlDoc.CreateElement("Component");
            componentElement.SetAttribute("type", monoBehaviour.GetType().ToString());
            componentElement.SetAttribute("enabled", monoBehaviour.enabled.ToString());
            componentElement.SetAttribute("category", "Script");
            parentElement.AppendChild(componentElement);
        }
        
        /// <summary>
        /// 添加普通组件信息
        /// </summary>
        private static void AppendComponentInfo(XmlElement parentElement, Component component, XmlDocument xmlDoc)
        {
            XmlElement componentElement = xmlDoc.CreateElement("Component");
            componentElement.SetAttribute("type", component.GetType().ToString());
            componentElement.SetAttribute("category", "Built-in");
            
            // 尝试获取enabled属性
            var enabledProperty = component.GetType().GetProperty("enabled");
            if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
            {
                try
                {
                    bool enabled = (bool)enabledProperty.GetValue(component);
                    componentElement.SetAttribute("enabled", enabled.ToString());
                }
                catch
                {
                    // 忽略获取enabled属性失败的情况
                }
            }
            
            parentElement.AppendChild(componentElement);
        }
        
        /// <summary>
        /// 添加Renderer信息
        /// </summary>
        private static void AppendRendererInfo(XmlElement parentElement, Renderer renderer, XmlDocument xmlDoc, bool includeMaterials)
        {
            XmlElement rendererElement = xmlDoc.CreateElement("Renderer");
            rendererElement.SetAttribute("type", renderer.GetType().ToString());
            rendererElement.SetAttribute("enabled", renderer.enabled.ToString());
            
            if (includeMaterials)
            {
                AppendMaterialsInfo(rendererElement, renderer, xmlDoc);
            }
            
            parentElement.AppendChild(rendererElement);
        }
        
        /// <summary>
        /// 添加材质信息
        /// </summary>
        private static void AppendMaterialsInfo(XmlElement parentElement, Renderer renderer, XmlDocument xmlDoc)
        {
            XmlElement materialsElement = xmlDoc.CreateElement("Materials");
            
            try
            {
                Material[] materials = renderer.materials;
                
                if (materials != null && materials.Length > 0)
                {
                    for (int i = 0; i < materials.Length; i++)
                    {
                        Material material = materials[i];
                        XmlElement materialElement = xmlDoc.CreateElement("Material");
                        
                        materialElement.SetAttribute("index", i.ToString());
                        materialElement.SetAttribute("name", material?.name ?? "null");
                        materialElement.SetAttribute("shader", material?.shader?.name ?? "null");
                        
                        // 在编辑器模式下尝试获取资源路径
                        #if UNITY_EDITOR
                        if (material != null)
                        {
                            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(material);
                            materialElement.SetAttribute("path", !string.IsNullOrEmpty(assetPath) ? assetPath : "Built-in");
                        }
                        else
                        {
                            materialElement.SetAttribute("path", "null");
                        }
                        #else
                        materialElement.SetAttribute("path", material?.name ?? "null");
                        #endif
                        
                        materialsElement.AppendChild(materialElement);
                    }
                }
                else
                {
                    // 没有材质时添加空材质节点
                    XmlElement emptyMaterialElement = xmlDoc.CreateElement("Material");
                    emptyMaterialElement.SetAttribute("index", "0");
                    emptyMaterialElement.SetAttribute("name", "null");
                    emptyMaterialElement.SetAttribute("shader", "null");
                    emptyMaterialElement.SetAttribute("path", "null");
                    materialsElement.AppendChild(emptyMaterialElement);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[EzGame.SnapShoot] " + $"获取材质信息时发生错误: {ex.Message}");
                
                // 添加错误信息节点
                XmlElement errorElement = xmlDoc.CreateElement("Material");
                errorElement.SetAttribute("error", ex.Message);
                materialsElement.AppendChild(errorElement);
            }
            
            parentElement.AppendChild(materialsElement);
        }
        
        /// <summary>
        /// 添加子对象
        /// </summary>
        private static void AppendChildObjects(XmlElement parentElement, Transform parentTransform, XmlDocument xmlDoc, ExportOptions options, int currentDepth)
        {
            int childCount = parentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                if (child != null && child.gameObject != null)
                {
                    AppendGameObject(parentElement, child.gameObject, xmlDoc, options, currentDepth);
                }
            }
        }
        
        // 格式化浮点数，去除多余的0，整数去除小数点
        private static string FormatFloat(float value)
        {
            // 检查是否为整数
            if (Mathf.Approximately(value, Mathf.Round(value)))
            {
                return Mathf.RoundToInt(value).ToString();
            }
            
            // 使用F6格式，然后去除尾部的0
            string formatted = value.ToString("F6", CultureInfo.InvariantCulture).TrimEnd('0');
            
            // 如果最后以小数点结尾，移除小数点
            if (formatted.EndsWith("."))
            {
                formatted = formatted.TrimEnd('.');
            }
            
            return formatted;
        }
    }
}