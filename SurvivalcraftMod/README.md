# Survivalcraft Mod 项目

## 环境要求

- [.NET SDK](https://dotnet.microsoft.com/download) (建议 10.0 或更高版本)

## 项目构建

### 编译命令

```bash
dotnet build
```

### 编译产物输出路径

编译完成后，`.scmod` 文件将根据以下环境变量配置输出到对应目录：

| 环境变量 | 说明 |
|---------|------|
| `SURVIVALCRAFT_MOD_DEBUG_PATH` | 调试模式输出路径（优先级最高） |
| `SURVIVALCRAFT_MOD_PATH` | 常规构建输出路径 |

若未配置任何环境变量，产物将默认输出至 `./src/bin/DEBUG/` 目录。

## 环境变量配置示例

**Windows (PowerShell)**
```powershell
$env:SURVIVALCRAFT_MOD_PATH = "D:\Survivalcraft\Mods"
```

**Linux / macOS**
```bash
export SURVIVALCRAFT_MOD_PATH="/path/to/Survivalcraft/Mods"
```

## 运行调试（仅适用于 Windows）

### 前置准备

1. **克隆生存战争插件版项目**

   ```bash
   git clone https://gitee.com/SC-SPM/SurvivalcraftApi.git
   ```

2. **构建调试应用**

   ```bash
   cd Survivalcraft.Windows
   dotnet build
   ```

### 配置调试环境

3. **设置调试输出路径**

   将以下路径配置为环境变量 `SURVIVALCRAFT_MOD_DEBUG_PATH` 的值：

   ```
   SurvivalcraftApi\Survivalcraft.Windows\bin\Debug\Mods
   ```

   请使用该目录的**绝对路径**。

   **PowerShell 示例：**
   ```powershell
   $env:SURVIVALCRAFT_MOD_DEBUG_PATH = "D:\path\to\SurvivalcraftApi\Survivalcraft.Windows\bin\Debug\Mods"
   ```

### 运行调试

4. **编译模组**

   在模组项目目录执行：

   ```bash
   dotnet build
   ```

   编译产物 `.scmod` 将自动输出到 `SURVIVALCRAFT_MOD_DEBUG_PATH` 指定的目录。

5. **启动游戏**

   在生存战争插件版项目目录执行：

   ```bash
   cd Survivalcraft.Windows
   dotnet watch run --configuration Debug
   ```

6. **验证模组加载**

   进入游戏后，观察控制台输出以确认模组是否正常加载。