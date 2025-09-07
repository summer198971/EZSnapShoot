# EzGame Unity插件集合

这是一个包含多个Unity编辑器插件的项目，每个插件都遵循Unity UPM (Unity Package Manager) 标准，可以独立使用或组合使用。

## 🔧 插件列表

### 1. [HierarchySnapShoot](Assets/HierarchySnapShoot/) - 场景层级快照工具
Unity场景层级快照工具，支持导出场景层级结构为XML格式。

**主要功能:**
- 导出当前场景的完整层级结构
- 支持DontDestroyOnLoad对象导出
- 详细的GameObject信息记录（Transform、Component、Material等）
- 编辑器菜单集成和设置面板

**安装:** `https://github.com/summer198971/EZSnapShoot.git?path=Assets/HierarchySnapShoot`

---

### 2. [即将推出] AssetManager - 资源管理工具
Unity项目资源管理和优化工具。

**计划功能:**
- 资源依赖分析
- 未使用资源检测
- 资源重复检查
- 批量资源处理

---

### 3. [即将推出] SceneTools - 场景工具集
Unity场景编辑和管理工具集合。

**计划功能:**
- 场景快速切换
- 场景对比工具
- 批量场景处理
- 场景统计分析

---

## 📦 安装方式

### 方式一：通过Unity Package Manager安装单个插件

1. 打开Unity编辑器
2. 打开Window > Package Manager
3. 点击左上角的"+"按钮
4. 选择"Add package from git URL"
5. 输入对应插件的Git URL（见上方插件列表）

### 方式二：克隆整个项目

```bash
git clone https://github.com/summer198971/EZSnapShoot.git
```

然后将需要的插件文件夹复制到你的Unity项目中。

### 方式三：安装所有插件

输入主仓库URL：`https://github.com/summer198971/EZSnapShoot.git`

## 🏗️ 项目架构

```
EZSnapShoot/
├── Assets/
│   ├── HierarchySnapShoot/          # 场景层级快照插件
│   ├── Shared/                      # 共享代码库（计划中）
│   └── Scenes/                      # 示例场景
├── Packages/                        # UPM包输出目录（构建时生成）
├── Build/                           # 构建脚本和配置（计划中）
├── README.md                        # 项目总览（本文件）
└── LICENSE                          # MIT许可证
```

## 🛠️ 系统要求

- Unity 2021.3 或更高版本
- .NET Standard 2.1

## 🧪 测试

每个插件都包含完整的测试覆盖：

1. 打开Unity Test Runner窗口：`Window > General > Test Runner`
2. 选择PlayMode或EditMode标签页
3. 点击"Run All"运行所有测试

## 📚 文档

每个插件都有独立的文档：
- 查看各插件目录下的README.md文件
- 查看Samples~/目录下的示例代码
- 查看Documentation~/目录下的详细文档

## 🤝 贡献指南

我们欢迎社区贡献！请遵循以下步骤：

1. Fork本项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建Pull Request

### 开发规范

- 遵循[Unity UPM插件开发规范](.cursor/rules/unity-ump-development.mdc)
- 每个插件必须包含完整的测试
- 代码必须通过所有测试
- 提交信息使用中文，格式清晰

## 📄 许可证

本项目采用MIT许可证 - 详见[LICENSE](LICENSE)文件

## 📞 联系方式

- **作者**: EzGame
- **邮箱**: support@ezgame.com
- **项目主页**: https://github.com/summer198971/EZSnapShoot
- **问题反馈**: [GitHub Issues](https://github.com/summer198971/EZSnapShoot/issues)

## 🔄 版本历史

- **v1.0.0** - 初始版本，包含HierarchySnapShoot插件
- 更多版本信息请查看各插件的CHANGELOG.md文件

## 🎯 路线图

- [ ] 完善HierarchySnapShoot插件功能
- [ ] 添加AssetManager资源管理插件
- [ ] 添加SceneTools场景工具插件
- [ ] 创建Shared共享代码库
- [ ] 添加自动化构建和发布流程
- [ ] 完善文档和示例
