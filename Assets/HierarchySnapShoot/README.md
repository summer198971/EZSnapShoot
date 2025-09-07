# HierarchySnapShoot - Unity场景层级快照工具

Unity场景层级快照工具，支持导出场景层级结构为XML格式，包含GameObject的详细信息。

## 🚀 功能特性

- **场景层级导出**: 一键导出当前场景的完整层级结构
- **指定场景导出**: 选择性导出特定场景的层级结构，支持场景名称和索引
- **批量场景导出**: 支持一次性导出所有已加载场景
- **DontDestroyOnLoad支持**: 专门处理持久化对象的导出
- **详细信息记录**: 记录GameObject的Transform、Component等详细信息
- **材质信息**: 包含Renderer组件的材质和Shader信息
- **灵活的导出选项**: 可自定义导出内容和深度限制
- **编辑器集成**: 通过Unity菜单栏轻松访问功能，提供直观的场景选择界面
- **设置面板**: 可自定义导出路径和其他选项
- **完整测试**: 包含Runtime和Editor测试覆盖

## 📦 安装方式

### 通过Unity Package Manager安装

1. 打开Unity编辑器
2. 打开Window > Package Manager
3. 点击左上角的"+"按钮
4. 选择"Add package from git URL"
5. 输入：`https://github.com/summer198971/EZSnapShoot.git?path=Assets/HierarchySnapShoot`

### 手动安装

1. 下载或克隆主项目仓库
2. 将`Assets/HierarchySnapShoot`文件夹复制到你的Unity项目的Assets目录下

## 🎯 使用方法

### 通过编辑器菜单

插件安装后，可以通过以下菜单访问功能：

1. **导出所有已加载场景 (Export All Loaded Scenes)**: 导出所有已加载场景和DontDestroyOnLoad对象
2. **导出指定场景 (Export Specific Scene)**: 选择性导出特定场景的层级结构
   - 支持输入场景名称
   - 支持从已加载场景列表中选择
   - 始终包含DontDestroyOnLoad对象
3. **导出DontDestroyOnLoad对象 (Export DontDestroyOnLoad)**: 仅导出DontDestroyOnLoad对象
4. **打开导出文件夹 (Open Export Folder)**: 打开导出文件所在目录
5. **设置 (Settings)**: 配置导出选项

### 通过代码调用

```csharp
using EzGame.SnapShoot;
using System.Xml;

// 获取当前已加载场景层级的XML文档（包括DontDestroyOnLoad）
XmlDocument sceneXml = HierarchyToXML.GetCurrentSceneLoadedHierarchyToXML();

// 获取指定场景的层级结构（包含DontDestroyOnLoad）
XmlDocument specificSceneXml = HierarchyToXML.GetSpecificSceneHierarchyToXML("SampleScene");

// 获取指定场景的XML字符串（包含DontDestroyOnLoad）
string xmlString = HierarchyToXML.GetSpecificSceneHierarchyToXMLString("SampleScene");

// 通过场景索引获取层级结构
XmlDocument sceneByIndexXml = HierarchyToXML.GetSpecificSceneHierarchyToXML(0);

// 获取所有已加载场景的名称
string[] loadedScenes = HierarchyToXML.GetLoadedSceneNames();

// 获取DontDestroyOnLoad对象的XML文档
XmlDocument dontDestroyXml = HierarchyToXML.GetDontDestroyOnLoadHierarchyToXML();

// 获取字符串格式的层级信息
var stringWriter = HierarchyToXML.GetDontDestroyOnLoadHierarchyToStr();
string xmlString = stringWriter.ToString();

// 使用自定义导出选项
var options = new ExportOptions
{
    IncludeTransform = true,
    IncludeComponents = true,
    IncludeMaterials = false,
    IncludeInactiveObjects = false,
    IncludeChildObjects = true,
    MaxDepth = 3
};
XmlDocument customXml = HierarchyToXML.GetSpecificSceneHierarchyToXML("SampleScene", options);
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

### XML结构说明

- **Hierarchy**: 根元素，包含所有场景
- **Scene**: 场景元素，包含场景名称和激活状态
- **GameObject**: 游戏对象元素，包含基本属性和子对象
- **Position/Rotation/Scale**: Transform信息
- **Components**: 组件信息，包含脚本数量
- **Renderer**: 渲染器信息（如果存在）
- **Materials**: 材质信息，包含材质名称、Shader和路径

## ⚙️ 设置选项

通过`EzGame/SnapShoot/Settings`菜单可以配置：

- **导出路径**: 自定义XML文件的保存位置
- **自动时间戳**: 是否在文件名中自动添加时间戳
- **包含非激活对象**: 是否导出非激活的GameObject

## 🛠️ 系统要求

- Unity 2021.3 或更高版本
- .NET Standard 2.1

## 📁 插件结构

```
HierarchySnapShoot/
├── Runtime/                          # 运行时代码
│   ├── Scripts/Core/
│   │   └── HierarchyToXML.cs        # 核心功能类
│   └── EzGame.SnapShoot.Runtime.asmdef
├── Editor/                           # 编辑器代码
│   ├── Scripts/
│   │   ├── MenuItems/               # 菜单项
│   │   │   └── SnapShootMenuItems.cs
│   │   └── Windows/                 # 编辑器窗口
│   │       └── SnapShootSettingsWindow.cs
│   └── EzGame.SnapShoot.Editor.asmdef
├── Tests/                            # 测试代码
│   ├── Runtime/                     # 运行时测试
│   │   └── Scripts/
│   │       └── HierarchyToXMLTests.cs
│   └── Editor/                      # 编辑器测试
│       └── Scripts/
│           └── SnapShootEditorTests.cs
├── Samples~/                         # 示例代码
│   └── BasicUsage/
│       ├── README.md                # 使用说明
│       └── SnapShootExample.cs      # 示例脚本
├── Documentation~/                   # 文档
├── package.json                      # UPM包配置
├── CHANGELOG.md                      # 版本更新日志
└── README.md                         # 本文件
```

## 🧪 运行测试

1. 打开Unity Test Runner窗口：`Window > General > Test Runner`
2. 选择PlayMode或EditMode标签页
3. 点击"Run All"运行所有测试

测试覆盖：
- **Runtime测试**: 核心XML生成功能
- **Editor测试**: 编辑器菜单和设置功能

## 📝 示例

查看`Samples~/BasicUsage`目录下的示例代码和文档：

- **SnapShootExample.cs**: 展示如何在代码中使用插件功能
- **README.md**: 详细的使用示例和说明

## 🔧 API参考

### HierarchyToXML类

#### 静态方法

- `GetCurrentSceneLoadedHierarchyToXML()`: 获取当前已加载场景层级的XML文档（包括所有已加载场景和DontDestroyOnLoad对象）
- `GetSpecificSceneHierarchyToXML(string sceneName)`: 获取指定场景的层级结构XML文档（包含DontDestroyOnLoad）
- `GetSpecificSceneHierarchyToXMLString(string sceneName)`: 获取指定场景的层级结构XML字符串（包含DontDestroyOnLoad）
- `GetSpecificSceneHierarchyToXML(int sceneIndex)`: 通过场景索引获取指定场景的层级结构XML文档
- `GetSpecificSceneHierarchyToXML(string sceneName, ExportOptions options)`: 使用自定义选项获取指定场景的层级结构
- `GetSpecificSceneHierarchyToXMLString(string sceneName, ExportOptions options)`: 使用自定义选项获取指定场景的XML字符串
- `GetLoadedSceneNames()`: 获取所有已加载场景的名称列表
- `GetDontDestroyOnLoadHierarchyToXML()`: 获取DontDestroyOnLoad对象的XML文档
- `GetDontDestroyOnLoadHierarchyToStr()`: 获取DontDestroyOnLoad对象的字符串格式

### 编辑器菜单

- `SnapShootMenuItems.ExportCurrentSceneHierarchy()`: 导出当前所有已加载场景
- `SnapShootMenuItems.ExportSpecificSceneHierarchy()`: 导出指定场景（提供选择界面）
- `SnapShootMenuItems.ExportDontDestroyOnLoadHierarchy()`: 导出DontDestroyOnLoad对象
- `SnapShootMenuItems.OpenExportFolder()`: 打开导出文件夹

## 📁 导出文件管理

### 默认导出路径
- **默认路径**: `Temp/SnapShoot_Exports/`
- **自定义路径**: 可通过设置面板或代码自定义

### 版本控制友好
导出的XML文件是临时生成的数据，**不应该**被版本控制系统跟踪：

- ✅ **Git**: 已在 `.gitignore` 中配置忽略规则
- ✅ **SVN**: 提供 `.svnignore` 文件模板
- ✅ **Unity**: 使用 `Temp/` 目录，Unity自动忽略

### 文件命名规则
- `Hierarchy_{场景名}_{时间戳}.xml` - 完整场景导出
- `Specific_{场景名}_{时间戳}.xml` - 指定场景导出
- `Batch_{场景名}_{时间戳}.xml` - 批量导出
- `DontDestroyOnLoad_{时间戳}.xml` - DontDestroyOnLoad对象

### 重要提示
⚠️ `Temp/` 目录下的文件在Unity重启时可能被清理，建议及时处理导出数据

## 🐛 已知问题

- 在运行时无法获取材质的完整资源路径，只能获取材质名称
- 非常大的场景可能导致XML文件过大

## 🔄 更新日志

查看[CHANGELOG.md](CHANGELOG.md)了解详细的版本更新信息。

## 🤝 贡献

欢迎提交Issue和Pull Request！请确保：

1. 代码遵循项目的编码规范
2. 添加适当的测试
3. 更新相关文档

## 📄 许可证

MIT License - 详见项目根目录的[LICENSE](../../LICENSE)文件

## 📞 支持

- **项目主页**: https://github.com/summer198971/EZSnapShoot
- **问题反馈**: [GitHub Issues](https://github.com/summer198971/EZSnapShoot/issues)
- **邮箱**: support@ezgame.com

---

*这是EzGame Unity插件集合的一部分。查看[项目主页](../../README.md)了解更多插件。*
