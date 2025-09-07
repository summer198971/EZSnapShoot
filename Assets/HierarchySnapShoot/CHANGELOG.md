# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-01-XX

### Added
- 初始版本发布
- 支持导出当前已加载场景层级结构为XML格式（包括所有已加载场景和DontDestroyOnLoad对象）
- 支持导出DontDestroyOnLoad对象层级结构
- 包含GameObject的详细信息：
  - 名称和激活状态
  - 位置、旋转、缩放信息
  - 组件信息（类型、启用状态）
  - 渲染器和材质信息
- Unity编辑器菜单集成
- 可配置的导出设置窗口
- 自动时间戳功能
- 完整的单元测试覆盖

### Features
- **场景层级导出**: 一键导出当前场景的完整层级结构
- **DontDestroyOnLoad支持**: 专门处理持久化对象的导出
- **详细信息记录**: 记录GameObject的Transform、Component等详细信息
- **材质信息**: 包含Renderer组件的材质和Shader信息
- **编辑器集成**: 通过Unity菜单栏轻松访问功能
- **设置面板**: 可自定义导出路径和其他选项
- **测试覆盖**: 包含Runtime和Editor测试

### Technical Details
- 基于Unity 2021.3+
- 使用标准UPM包结构
- 独立的Assembly Definition文件
- 符合Unity编码规范
- 支持.NET Standard 2.1

## [Unreleased]

### Added
- **指定场景导出功能**: 新增 `GetSpecificSceneHierarchyToXML()` 方法支持导出指定场景
  - 支持通过场景名称导出: `GetSpecificSceneHierarchyToXML(string sceneName)`
  - 支持通过场景索引导出: `GetSpecificSceneHierarchyToXML(int sceneIndex)`
  - 支持自定义导出选项: `GetSpecificSceneHierarchyToXML(string sceneName, ExportOptions options)`
- **场景名称查询**: 新增 `GetLoadedSceneNames()` 方法获取所有已加载场景名称
- **编辑器菜单增强**: 新增"Export Specific Scene Hierarchy"菜单项
  - 提供直观的场景选择界面
  - 支持单个场景导出和批量导出
  - 显示导出结果摘要
- **完整的单元测试**: 为所有新方法添加了完整的测试覆盖

### Improved
- 优化了菜单项的组织结构和用户体验
- 增强了错误处理和用户反馈
- 完善了API文档和使用示例

### Changed
- **🔄 重要变更**: 默认导出路径从 `Assets/SnapShoot_Exports/` 改为 `Temp/SnapShoot_Exports/`
  - 更加版本控制友好，导出文件不会被Git/SVN跟踪
  - Unity会自动忽略 `Temp/` 目录
  - 避免了导出文件污染项目资源
- 更新了 `.gitignore` 规则以覆盖各种导出路径
- 添加了 `.svnignore` 文件模板支持SVN用户

### Planned
- 支持更多导出格式（JSON、CSV等）
- 层级比较和差异分析
- 自定义过滤器和导出规则
- 性能优化和大场景支持
