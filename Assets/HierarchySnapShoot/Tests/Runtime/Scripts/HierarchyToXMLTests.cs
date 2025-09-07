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
        public void GetCurrentSceneHierarchyToXML_ShouldReturnValidXML()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneHierarchyToXML();
            
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
            var xmlDoc = HierarchyToXML.GetCurrentSceneHierarchyToXML();
            string xmlString = xmlDoc.OuterXml;
            
            // Assert
            Assert.IsTrue(xmlString.Contains("TestObject"), "XML should contain the test object");
        }
        
        [Test]
        public void XMLDocument_ShouldContainSceneElement()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneHierarchyToXML();
            var sceneElements = xmlDoc.GetElementsByTagName("Scene");
            
            // Assert
            Assert.IsTrue(sceneElements.Count > 0, "XML should contain at least one Scene element");
        }
        
        [Test]
        public void XMLDocument_ShouldContainDontDestroyOnLoadScene()
        {
            // Act
            var xmlDoc = HierarchyToXML.GetCurrentSceneHierarchyToXML();
            string xmlString = xmlDoc.OuterXml;
            
            // Assert
            Assert.IsTrue(xmlString.Contains("DontDestroyOnLoad"), "XML should contain DontDestroyOnLoad scene");
        }
    }
}
