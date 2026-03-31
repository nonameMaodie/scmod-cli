# scmod

Survivalcraft 模组创建 CLI 工具

## 安装

```bash
npm install
npm install -g . #全局安装
```

## 使用

```bash
# 开发模式
npm run dev new <project-name>

# 构建后运行
npm run build
node dist/index.js new <project-name>

# 全局安装后
scmod new <project-name>
```

## 命令

- `new <project-name>` - 创建新的 Survivalcraft 模组项目

示例:

```bash
scmod new my-awesome-mod
```

这将复制 `../SurvivalcraftMod` 模板到当前目录下的 `my-awesome-mod` 文件夹，并自动重命名相关文件。

## 项目结构

```
scmod/
  src/
    index.ts      # 主入口
  tsconfig.json  # TypeScript 配置
  package.json   # 项目配置
```

## 开发

```bash
# 安装依赖
npm install

# 构建
npm run build

# 开发模式（构建后直接运行）
npm run dev new my-mod
```

## 技术栈

- TypeScript
- Node.js
- inquirer (命令行交互)
