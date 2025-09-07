using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EzGame.SnapShoot;

namespace EzGame.SnapShoot.Tests
{
    public class HierarchyToXMLTests
    {
        private GameObject _testObject;
        
        [SetUp]
        public void SetUp()
        {
            // 创建测试用的GameObject
            _testObject = new GameObject("TestObject");
            _testObject.AddComponent<MeshRenderer>();
            _testObject.AddComponent<BoxCollider>();
        }
        
        [TearDown]
        public void TearDown()
        {
            // 清理测试对象
            if (_testObject != null)
            {
                Object.DestroyImmediate(_testObject);
            }
        }
        
        [Test]
        public void GetCurrentSceneLoadedHierarchyToXML_ShouldReturnValidXML()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneLoadedHierarchyToXML();
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            Assert.IsNotNull(xmlDoc.DocumentElement);
            Assert.AreEqual("Hierarchy", xmlDoc.DocumentElement.Name);
        }
        
        [Test]
        public void GetDontDestroyOnLoadHierarchyToXML_ShouldReturnValidXML()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetDontDestroyOnLoadHierarchyToXML();
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            Assert.IsNotNull(xmlDoc.DocumentElement);
            Assert.AreEqual("Hierarchy", xmlDoc.DocumentElement.Name);
        }
        
        [Test]
        public void GetDontDestroyOnLoadHierarchyToStr_ShouldReturnValidString()
        {
            // Act
            var stringWriter = HierarchyToXML.GetDontDestroyOnLoadHierarchyToStr();
            string xmlString = stringWriter.ToString();
            
            // Assert
            Assert.IsNotNull(xmlString);
            Assert.IsTrue(xmlString.Contains("<Hierarchy"));
            Assert.IsTrue(xmlString.Contains("</Hierarchy>"));
        }
        
        [UnityTest]
        public IEnumerator HierarchyXML_ShouldIncludeTestObject()
        {
            // Arrange - 等待一帧确保对象被正确创建
            yield return null;
            
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneLoadedHierarchyToXML();
            string xmlString = xmlDoc.OuterXml;
            
            // Assert
            Assert.IsTrue(xmlString.Contains("TestObject"), "XML should contain the test object");
        }
        
        [Test]
        public void XMLDocument_ShouldContainSceneElement()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneLoadedHierarchyToXML();
            var sceneElements = xmlDoc.GetElementsByTagName("Scene");
            
            // Assert
            Assert.IsTrue(sceneElements.Count > 0, "XML should contain at least one Scene element");
        }
        
        [Test]
        public void XMLDocument_ShouldContainDontDestroyOnLoadScene()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneLoadedHierarchyToXML();
            string xmlString = xmlDoc.OuterXml;
            
            // Assert
            Assert.IsTrue(xmlString.Contains("DontDestroyOnLoad"), "XML should contain DontDestroyOnLoad scene");
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_WithValidSceneName_ShouldReturnValidXML()
        {
            // Arrange
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(currentSceneName);
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            Assert.IsNotNull(xmlDoc.DocumentElement);
            Assert.AreEqual("Hierarchy", xmlDoc.DocumentElement.Name);
            
            // 验证包含目标场景属性
            Assert.AreEqual(currentSceneName, xmlDoc.DocumentElement.GetAttribute("targetScene"));
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_WithInvalidSceneName_ShouldReturnNull()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML("NonExistentScene");
            
            // Assert
            Assert.IsNull(xmlDoc);
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_WithDontDestroyOnLoad_ShouldReturnValidXML()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML("DontDestroyOnLoad");
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            Assert.IsNotNull(xmlDoc.DocumentElement);
            Assert.AreEqual("Hierarchy", xmlDoc.DocumentElement.Name);
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_WithSceneIndex_ShouldReturnValidXML()
        {
            // Arrange
            int activeSceneIndex = 0; // 通常活动场景的索引是0
            
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(activeSceneIndex);
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            Assert.IsNotNull(xmlDoc.DocumentElement);
            Assert.AreEqual("Hierarchy", xmlDoc.DocumentElement.Name);
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_WithInvalidSceneIndex_ShouldReturnNull()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(999);
            
            // Assert
            Assert.IsNull(xmlDoc);
        }
        
        [Test]
        public void GetLoadedSceneNames_ShouldReturnNonEmptyArray()
        {
            // Act
            string[] sceneNames = HierarchyToXML.GetLoadedSceneNames();
            
            // Assert
            Assert.IsNotNull(sceneNames);
            Assert.IsTrue(sceneNames.Length > 0, "Should have at least one loaded scene");
            
            // 验证包含当前活动场景
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Assert.Contains(currentSceneName, sceneNames);
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_WithOptions_ShouldRespectOptions()
        {
            // Arrange
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            var options = new ExportOptions
            {
                IncludeTransform = false,
                IncludeComponents = false,
                IncludeMaterials = false,
                IncludeInactiveObjects = false,
                IncludeChildObjects = true,
                MaxDepth = 1
            };
            
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(currentSceneName, options);
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            string xmlString = xmlDoc.OuterXml;
            
            // 验证不包含Transform信息（因为IncludeTransform = false）
            Assert.IsFalse(xmlString.Contains("<Position"), "Should not contain Position elements when IncludeTransform is false");
            Assert.IsFalse(xmlString.Contains("<Rotation"), "Should not contain Rotation elements when IncludeTransform is false");
            Assert.IsFalse(xmlString.Contains("<Scale"), "Should not contain Scale elements when IncludeTransform is false");
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXMLString_WithValidSceneName_ShouldReturnValidXMLString()
        {
            // Arrange
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Act
            string xmlString = HierarchyToXML.GetSpecificSceneHierarchyToXMLString(currentSceneName);
            
            // Assert
            Assert.IsNotNull(xmlString);
            Assert.IsTrue(xmlString.Contains("<Hierarchy"));
            Assert.IsTrue(xmlString.Contains("</Hierarchy>"));
            Assert.IsTrue(xmlString.Contains($"targetScene=\"{currentSceneName}\""));
            Assert.IsTrue(xmlString.Contains("DontDestroyOnLoad"), "Should always include DontDestroyOnLoad scene");
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXMLString_WithInvalidSceneName_ShouldReturnNull()
        {
            // Act
            string xmlString = HierarchyToXML.GetSpecificSceneHierarchyToXMLString("NonExistentScene");
            
            // Assert
            Assert.IsNull(xmlString);
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXMLString_WithDontDestroyOnLoad_ShouldReturnValidXMLString()
        {
            // Act
            string xmlString = HierarchyToXML.GetSpecificSceneHierarchyToXMLString("DontDestroyOnLoad");
            
            // Assert
            Assert.IsNotNull(xmlString);
            Assert.IsTrue(xmlString.Contains("<Hierarchy"));
            Assert.IsTrue(xmlString.Contains("</Hierarchy>"));
            Assert.IsTrue(xmlString.Contains("targetScene=\"DontDestroyOnLoad\""));
            Assert.IsTrue(xmlString.Contains("name=\"DontDestroyOnLoad\""));
        }
        
        [Test]
        public void GetSpecificSceneHierarchyToXML_ShouldAlwaysIncludeDontDestroyOnLoad()
        {
            // Arrange
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Act
            var xmlDoc = HierarchyToXML.GetSpecificSceneHierarchyToXML(currentSceneName);
            
            // Assert
            Assert.IsNotNull(xmlDoc);
            string xmlString = xmlDoc.OuterXml;
            
            // 验证包含指定场景和DontDestroyOnLoad场景
            Assert.IsTrue(xmlString.Contains($"name=\"{currentSceneName}\""), "Should contain the specified scene");
            Assert.IsTrue(xmlString.Contains("name=\"DontDestroyOnLoad\""), "Should always include DontDestroyOnLoad scene");
        }
    }
}
