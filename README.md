# scmod-cli

Survivalcraft（生存战争）模组脚手架 CLI 工具，用于快速创建模组项目。

## 功能

- **`scmod new <name>`** — 从模板创建新的模组项目，自动替换项目名称、命名空间和配置

## 环境要求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) 或更高版本
- Git（可选，用于初始化仓库）

## 安装

### 从源码构建并安装

```bash
# 克隆仓库
git clone https://github.com/nonameMaodie/scmod-cli.git
cd scmod-cli

# 构建并打包
dotnet pack

# 安装为全局工具
dotnet tool install -g --add-source ./src/ScmodCli/nupkg ScmodCli
```

### 直接运行（无需安装）

```bash
dotnet run --project src/ScmodCli -- new MyMod
```

## 使用

### 创建新项目

```bash
scmod new MyAwesomeMod
```

执行后会在当前目录创建 `MyAwesomeMod/` 文件夹，包含：
- 完整的 .NET 项目结构
- 已配置好的 `modinfo.json`
- 可选的 Git 仓库初始化

## 项目结构

```
scmod-cli/
├── src/ScmodCli/
│   ├── Program.cs              # CLI 入口
│   ├── Commands/
│   │   └── ProjectCreator.cs   # 项目创建逻辑
│   ├── TemplateExtractor.cs    # 模板解压
│   └── Template/
│       └── Template.zip        # 模组项目模板
└── test-output/                # 测试输出目录
```

## 开发

```bash
# 构建
dotnet build

# 清理
dotnet clean

# 卸载工具
dotnet tool uninstall -g ScmodCli
```

## 许可证

[MIT](LICENSE)

## 作者

nonameMaodie

## 其他

`typescript` 版本请切换至 `typescript-version` 分支
