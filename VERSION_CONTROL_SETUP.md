# EZ SnapShoot - 版本控制设置指南

EZ SnapShoot 导出的XML文件是临时生成的数据文件，不应该被版本控制系统跟踪。本指南将帮助你正确配置版本控制系统。

## 🎯 为什么要忽略导出文件？

- **临时性**: 导出的XML文件是运行时生成的临时数据
- **体积大**: 大型场景的XML文件可能很大，会增加仓库体积
- **频繁变化**: 每次导出都会生成新的时间戳文件
- **无需共享**: 这些文件通常只用于本地分析，不需要团队共享

## 📁 默认导出路径

EZ SnapShoot 使用以下默认路径，这些路径已经被Unity和版本控制系统自动忽略：

- **主要导出路径**: `Temp/SnapShoot_Exports/`
- **示例脚本路径**: `Temp/MyCustomExports/`
- **测试路径**: `Temp/Test_SnapShoot_Exports/`

## 🔧 Git 配置

### 自动配置
项目已包含 `.gitignore` 文件，包含了完整的忽略规则。无需额外配置。

### 手动配置
如果你需要手动添加规则，在项目根目录的 `.gitignore` 文件中添加：

```gitignore
# EZ SnapShoot 导出文件
/Temp/SnapShoot_Exports/
/Temp/MyCustomExports/
**/SnapShoot_Exports*/
Hierarchy_*.xml
Scene_*.xml
Specific_*.xml
Batch_*.xml
DontDestroyOnLoad_*.xml
```

## 🔧 SVN 配置

### 使用提供的模板
1. 将项目根目录的 `.svnignore` 文件内容复制
2. 设置SVN忽略属性：
   ```bash
   svn propset svn:ignore -F .svnignore .
   ```
3. 提交属性更改：
   ```bash
   svn commit -m "Add SnapShoot ignore rules"
   ```

### 手动配置
使用SVN命令行工具设置忽略规则：

```bash
# 忽略导出目录
svn propset svn:ignore "SnapShoot_Exports" Temp/
svn propset svn:ignore "MyCustomExports" Temp/

# 忽略XML文件模式
svn propset svn:ignore "Hierarchy_*.xml" .
svn propset svn:ignore "Scene_*.xml" .
```

## 🔧 其他版本控制系统

### Mercurial (.hgignore)
```
# EZ SnapShoot 导出文件
syntax: glob
Temp/SnapShoot_Exports/**
Temp/MyCustomExports/**
Hierarchy_*.xml
Scene_*.xml
Specific_*.xml
Batch_*.xml
DontDestroyOnLoad_*.xml
```

### Perforce (.p4ignore)
```
# EZ SnapShoot 导出文件
Temp/SnapShoot_Exports/...
Temp/MyCustomExports/...
Hierarchy_*.xml
Scene_*.xml
Specific_*.xml
Batch_*.xml
DontDestroyOnLoad_*.xml
```

## ⚠️ 重要注意事项

1. **Temp目录特性**: Unity的 `Temp/` 目录在Unity重启时可能被清理
2. **及时处理**: 建议及时处理或备份重要的导出数据
3. **自定义路径**: 如果你自定义了导出路径到 `Assets/` 下，请手动添加忽略规则
4. **团队协作**: 确保团队成员都正确配置了版本控制忽略规则

## 🛠️ 故障排除

### 如果导出文件已经被跟踪
如果导出文件已经被Git跟踪，需要先移除：

```bash
# 移除已跟踪的文件（保留本地文件）
git rm --cached -r Assets/SnapShoot_Exports/
git rm --cached Hierarchy_*.xml

# 提交更改
git commit -m "Remove SnapShoot export files from tracking"
```

### 检查忽略规则是否生效
```bash
# Git: 检查文件是否被忽略
git check-ignore Temp/SnapShoot_Exports/test.xml

# SVN: 查看忽略属性
svn propget svn:ignore .
```

## 📞 需要帮助？

如果在配置版本控制时遇到问题，请：

1. 查看项目的 [GitHub Issues](https://github.com/summer198971/EZSnapShoot/issues)
2. 发送邮件至 support@ezgame.com
3. 确保使用的是最新版本的插件
