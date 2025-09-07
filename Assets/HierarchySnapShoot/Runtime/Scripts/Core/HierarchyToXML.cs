using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;


namespace EzGame.SnapShoot
{
    public class HierarchyToXML
    {
        public static StringWriter GetDontDestroyOnLoadHierarchyToStr()
        {
            XmlDocument xml = GetCurrentSceneHierarchyToXML();
            StringWriter sw = new StringWriter();
            xml.Save(sw);
            return sw;
        }
        public static XmlDocument GetDontDestroyOnLoadHierarchyToXML()
        {
            // 创建XmlDocument对象
            XmlDocument xmlDoc = new XmlDocument();

            // 创建根元素
            XmlElement root = xmlDoc.CreateElement("Hierarchy");
            xmlDoc.AppendChild(root);

            // 创建场景元素
            XmlElement sceneElement = xmlDoc.CreateElement("Scene");
            sceneElement.SetAttribute("name", "DontDestroyOnLoad");
            root.AppendChild(sceneElement);
            GameObject[] DontDestroyOnLoad = getDontDestroyOnLoadGameObjects();
            foreach (GameObject rootObject in DontDestroyOnLoad)
            {
                AppendGameObject(sceneElement, rootObject, xmlDoc);
            }

            return xmlDoc;
        }
        public static XmlDocument GetCurrentSceneHierarchyToXML()
        {
            // 创建XmlDocument对象
            XmlDocument xmlDoc = new XmlDocument();

            // 创建根元素
            XmlElement root = xmlDoc.CreateElement("Hierarchy");
            xmlDoc.AppendChild(root);

            // 获取当前激活的场景
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            // 创建场景元素
            XmlElement sceneElement = xmlDoc.CreateElement("Scene");
            sceneElement.SetAttribute("name", activeScene.name);
            sceneElement.SetAttribute("active", "true");
            root.AppendChild(sceneElement);
            // 获取所有根对象
            GameObject[] rootObjects = activeScene.GetRootGameObjects();

            // 遍历每个根对象
            foreach (GameObject rootObject in rootObjects)
            {
                AppendGameObject(sceneElement, rootObject, xmlDoc);
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
                    root.AppendChild(inactiveSceneElement);
                    
                    // 获取该场景的所有根对象
                    GameObject[] inactiveSceneRootObjects = scene.GetRootGameObjects();
                    
                    // 遍历每个根对象
                    foreach (GameObject rootObject in inactiveSceneRootObjects)
                    {
                        AppendGameObject(inactiveSceneElement, rootObject, xmlDoc);
                    }
                }
            }

            // 添加DontDestroyOnLoad场景
            XmlElement dontDestroyElement = xmlDoc.CreateElement("Scene");
            dontDestroyElement.SetAttribute("name", "DontDestroyOnLoad");
            dontDestroyElement.SetAttribute("active", "true");  // DontDestroyOnLoad总是激活的
            root.AppendChild(dontDestroyElement);
            GameObject[] DontDestroyOnLoad = getDontDestroyOnLoadGameObjects();
            foreach (GameObject rootObject in DontDestroyOnLoad)
            {
                AppendGameObject(dontDestroyElement, rootObject, xmlDoc);
            }

            return xmlDoc;
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

        private static void AppendGameObject(XmlElement parentElement, GameObject gameObject, XmlDocument xmlDoc)
        {
            // 创建GameObject元素并设置属性
            XmlElement gameObjectElement = xmlDoc.CreateElement("GameObject");
            gameObjectElement.SetAttribute("name", gameObject.name);
            gameObjectElement.SetAttribute("active", gameObject.activeSelf.ToString());
            gameObjectElement.SetAttribute("ChildCount", gameObject.transform.childCount.ToString());
            
            // 添加位置信息
            XmlElement positionElement = xmlDoc.CreateElement("Position");
            Vector3 position = gameObject.transform.localPosition;
            positionElement.SetAttribute("x", FormatFloat(position.x));
            positionElement.SetAttribute("y", FormatFloat(position.y));
            positionElement.SetAttribute("z", FormatFloat(position.z));
            gameObjectElement.AppendChild(positionElement);
            
            // 添加旋转角度信息
            XmlElement rotationElement = xmlDoc.CreateElement("Rotation");
            Vector3 rotation = gameObject.transform.eulerAngles;
            rotationElement.SetAttribute("x", FormatFloat(rotation.x));
            rotationElement.SetAttribute("y", FormatFloat(rotation.y));
            rotationElement.SetAttribute("z", FormatFloat(rotation.z));
            gameObjectElement.AppendChild(rotationElement);
            
            // 添加缩放信息
            XmlElement scaleElement = xmlDoc.CreateElement("Scale");
            Vector3 scale = gameObject.transform.localScale;
            scaleElement.SetAttribute("x", FormatFloat(scale.x));
            scaleElement.SetAttribute("y", FormatFloat(scale.y));
            scaleElement.SetAttribute("z", FormatFloat(scale.z));
            gameObjectElement.AppendChild(scaleElement);

            // 获取Component
            Component[] components = gameObject.GetComponents<Component>();

            // 挂载脚本的数量
            int scriptCount = 0;

            // 创建Components元素
            XmlElement componentsElement = xmlDoc.CreateElement("Components");

            // 检查是否有渲染组件
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                XmlElement rendererElement = xmlDoc.CreateElement("Renderer");
                rendererElement.SetAttribute("type", renderer.GetType().ToString());
                
                // 创建Materials元素
                XmlElement materialsElement = xmlDoc.CreateElement("Materials");
                
                // 获取所有材质
                Material[] materials = renderer.materials;
                
                if (materials != null && materials.Length > 0)
                {
                    for (int i = 0; i < materials.Length; i++)
                    {
                        Material material = materials[i];
                        XmlElement materialElement = xmlDoc.CreateElement("Material");
                        
                        // 设置材质名称
                        materialElement.SetAttribute("name", material != null ? material.name : "null");
                        
                        // 设置Shader名称
                        if (material != null && material.shader != null)
                        {
                            materialElement.SetAttribute("shader", material.shader.name);
                        }
                        else
                        {
                            materialElement.SetAttribute("shader", "null");
                        }
                        
                        // 尝试获取材质文件路径
                        string materialPath = "null";
                        if (material != null)
                        {
                            // 在运行时，我们无法直接获取材质的资源路径，但可以使用名称作为标识
                            materialPath = material.name;
                        }
                        materialElement.SetAttribute("path", materialPath);
                        
                        materialsElement.AppendChild(materialElement);
                    }
                }
                else
                {
                    XmlElement materialElement = xmlDoc.CreateElement("Material");
                    materialElement.SetAttribute("name", "null");
                    materialElement.SetAttribute("shader", "null");
                    materialElement.SetAttribute("path", "null");
                    materialsElement.AppendChild(materialElement);
                }
                
                rendererElement.AppendChild(materialsElement);
                componentsElement.AppendChild(rendererElement);
            }

            foreach (Component component in components)
            {
                // 检查是否是MonoBehaviour，排除Transform
                if (component is MonoBehaviour monoBehaviour)
                {
                    scriptCount++;
                    XmlElement componentElement = xmlDoc.CreateElement("Component");
                    componentElement.SetAttribute("type", component.GetType().ToString());
                    componentElement.SetAttribute("enabled", monoBehaviour.enabled.ToString());
                    componentsElement.AppendChild(componentElement);
                }
            }

            // 将Components元素添加到GameObject元素
            componentsElement.SetAttribute("scriptCount", scriptCount.ToString());
            gameObjectElement.AppendChild(componentsElement);

            // 将GameObject元素添加到父元素
            parentElement.AppendChild(gameObjectElement);

            // 递归处理子对象
            foreach (Transform child in gameObject.transform)
            {
                AppendGameObject(gameObjectElement, child.gameObject, xmlDoc);
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