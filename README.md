# EZ SnapShoot

Unity场景层级快照工具，支持导出场景层级结构为XML格式。

## 🚀 功能特性

- **场景层级导出**: 一键导出当前场景的完整层级结构
- **DontDestroyOnLoad支持**: 专门处理持久化对象的导出
- **详细信息记录**: 记录GameObject的Transform、Component等详细信息
- **材质信息**: 包含Renderer组件的材质和Shader信息
- **编辑器集成**: 通过Unity菜单栏轻松访问功能
- **设置面板**: 可自定义导出路径和其他选项
- **完整测试**: 包含Runtime和Editor测试覆盖

## 📦 安装方式

### 通过Unity Package Manager安装

1. 打开Unity编辑器
2. 打开Window > Package Manager
3. 点击左上角的"+"按钮
4. 选择"Add package from git URL"
5. 输入：`https://github.com/summer198971/EZSnapShoot.git`

### 手动安装

1. 下载或克隆此仓库
2. 将`Assets/HierarchySnapShoot`文件夹复制到你的Unity项目的Assets目录下

## 🎯 使用方法

### 通过编辑器菜单

1. **导出当前场景**: `EzGame/SnapShoot/Export Current Scene Hierarchy`
2. **导出DontDestroyOnLoad对象**: `EzGame/SnapShoot/Export DontDestroyOnLoad Hierarchy`
3. **打开导出文件夹**: `EzGame/SnapShoot/Open Export Folder`
4. **设置配置**: `EzGame/SnapShoot/Settings`

### 通过代码调用

```csharp
using EzGame.SnapShoot;
using System.Xml;

// 获取当前场景层级的XML文档
XmlDocument sceneXml = HierarchyToXML.GetCurrentSceneHierarchyToXML();

// 获取DontDestroyOnLoad对象的XML文档
XmlDocument dontDestroyXml = HierarchyToXML.GetDontDestroyOnLoadHierarchyToXML();

// 获取字符串格式的层级信息
var stringWriter = HierarchyToXML.GetDontDestroyOnLoadHierarchyToStr();
string xmlString = stringWriter.ToString();
```

## 📋 XML输出格式

导出的XML包含以下信息：

```xml
<Hierarchy>
  <Scene name="SampleScene" active="true">
    <GameObject name="Main Camera" active="true" ChildCount="0">
      <Position x="0" y="1" z="-10" />
      <Rotation x="0" y="0" z="0" />
      <Scale x="1" y="1" z="1" />
      <Components scriptCount="0">
        <Renderer type="UnityEngine.Camera">
          <Materials>
            <Material name="Default-Material" shader="Standard" path="Default-Material" />
          </Materials>
        </Renderer>
        <Component type="UnityEngine.AudioListener" enabled="True" />
      </Components>
    </GameObject>
  </Scene>
  <Scene name="DontDestroyOnLoad" active="true">
    <!-- DontDestroyOnLoad对象 -->
  </Scene>
</Hierarchy>
```

## 🛠️ 系统要求

- Unity 2021.3 或更高版本
- .NET Standard 2.1

## 📁 项目结构

```
HierarchySnapShoot/
├── Runtime/                          # 运行时代码
│   ├── Scripts/Core/
│   │   └── HierarchyToXML.cs        # 核心功能类
│   └── EzGame.SnapShoot.Runtime.asmdef
├── Editor/                           # 编辑器代码
│   ├── Scripts/
│   │   ├── MenuItems/               # 菜单项
│   │   └── Windows/                 # 编辑器窗口
│   └── EzGame.SnapShoot.Editor.asmdef
├── Tests/                            # 测试代码
│   ├── Runtime/                     # 运行时测试
│   └── Editor/                      # 编辑器测试
├── Samples~/                         # 示例代码
│   └── BasicUsage/
├── Documentation~/                   # 文档
├── package.json                      # UPM包配置
└── CHANGELOG.md                      # 版本更新日志
```

## 🧪 运行测试

1. 打开Unity Test Runner窗口：`Window > General > Test Runner`
2. 选择PlayMode或EditMode标签页
3. 点击"Run All"运行所有测试

## 📝 示例

查看`Samples~/BasicUsage`目录下的示例代码和文档，了解如何在你的项目中使用EZ SnapShoot。

## 🤝 贡献

欢迎提交Issue和Pull Request来帮助改进这个项目！

## 📄 许可证

MIT License - 详见[LICENSE](LICENSE)文件

## 📞 联系方式

- 作者: EzGame
- 邮箱: support@ezgame.com
- 项目主页: https://github.com/summer198971/EZSnapShoot

## 🔄 更新日志

查看[CHANGELOG.md](Assets/HierarchySnapShoot/CHANGELOG.md)了解版本更新详情。
