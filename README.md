# KeyFlow Studio Lite

**KeyFlow Studio Lite** 是一款面向 Windows 10/11 x64 的本地桌面键鼠自动化工具。用户可通过全局键鼠录制、Lua 5.4 脚本编辑、播放/暂停/停止和悬浮控制窗完成重复性桌面操作。

> 这个仓库本身就是项目交接入口。下一个 GPT / 代码代理只需要打开本仓库地址并读取根目录 `AGENTS.md`，无需用户再复制一段会话提示词。

## 项目状态

- 当前基线：**1.0.0**
- 代码：WinForms + .NET 10 + Lua 5.4（NLua/KeraLua）
- 平台：Windows 10/11 x64
- 默认分支：`main`
- 稳定版 Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.0>
- Windows ZIP：`KeyFlowStudioLite-1.0.0-win-x64.zip`（51,911,083 bytes）
- SHA256：`f9e33e319f725fbc94d85d73657de878f2f930f869276d2d0b0032936f9f7a90`
- 新仓库 Release Actions：[43727898613](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43727898613) SUCCESS，7/7 Core tests，0 warnings/errors。
- 项目交接：[`docs/SESSION-HANDOFF.md`](docs/SESSION-HANDOFF.md)
- 自动化开发规则：[`AGENTS.md`](AGENTS.md)

## 下载与运行

发布资产名称：

- `KeyFlowStudioLite-1.0.0-win-x64.zip`
- `KeyFlowStudioLite-1.0.0-win-x64.zip.sha256`

从 [Releases](https://github.com/xiaobei206/KeyFlowStudioLite/releases) 下载 ZIP，先用同目录 SHA256 文件核对完整性，再完整解压并运行 `Anjian.App.exe`。不要只提取 EXE 单独运行，Worker、Lua 和运行时伴随文件必须保留。

## 功能范围（1.0.0）

- 独立悬浮控制窗、完整主窗口和系统托盘生命周期。
- 本地 Lua 脚本新建、打开、保存、重命名、复制和删除。
- 全局鼠标/键盘录制；暂停/续录；只录点击坐标；鼠标移动降噪。
- 鼠标移动、左右中键、按下/释放、滚轮、键盘单键、组合键和 Unicode 文本输入。
- 独立 Worker 进程运行 Lua；播放、暂停、停止、输入释放和错误日志。
- 循环次数、轮间隔、F10 播放、F11 暂停/继续、F12 中止。
- 悬浮窗置顶、实时坐标、录制状态和播放设置同步。

完整边界见 [Lite 1.0.0 范围冻结](docs/LITE-SCOPE-CN.md) 和 [功能矩阵](docs/FEATURE-MATRIX-CN.md)。

## 开发环境与构建

需要 Windows 10/11 x64、.NET 10 SDK 和 Git。

在仓库根目录执行：

```powershell
dotnet restore Anjian2026.sln
dotnet build Anjian2026.sln -c Release --no-restore
dotnet test tests/Anjian.Core.Tests/Anjian.Core.Tests.csproj -c Release
dotnet publish src/Anjian.App/Anjian.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o artifacts/publish/win-x64
```

项目内部解决方案、程序集和 EXE 仍使用历史名称 `Anjian2026.sln` / `Anjian.App.exe`，这是迁移兼容决策，不代表仓库或产品仍叫 Anjian。

## 文档导航

| 文档 | 用途 |
|---|---|
| [AGENTS.md](AGENTS.md) | AI/人工协作的强制工作协议；新会话唯一必读入口 |
| [SESSION-HANDOFF](docs/SESSION-HANDOFF.md) | 当前版本、验证证据、唯一下一任务 |
| [PRD](docs/PRD-CN.md) | 产品定位、目标用户、需求和非目标 |
| [ARCHITECTURE](docs/ARCHITECTURE-CN.md) | 真实代码架构、进程与输入边界 |
| [FEATURE-MATRIX](docs/FEATURE-MATRIX-CN.md) | 已实现功能、限制和待办对照 |
| [ROADMAP](docs/ROADMAP-CN.md) | 里程碑和后续开发顺序 |
| [LUA-API](docs/LUA-API-CN.md) | Lua 命令和参数语义 |
| [RECORDER-DESIGN](docs/RECORDER-DESIGN-CN.md) | 录制、事件配对和代码生成 |
| [USER-GUIDE](docs/USER-GUIDE-CN.md) | 用户操作说明 |
| [ACCEPTANCE-TEST](docs/ACCEPTANCE-TEST-CN.md) | CI 与 Windows 实机验收 |
| [DEVELOPMENT-LOG](docs/DEVELOPMENT-LOG-CN.md) | 累计开发历史 |
| [DECISIONS](docs/DECISIONS-CN.md) | 不可随意推翻的架构/兼容决策 |
| [RELEASE-PROCESS](docs/RELEASE-PROCESS-CN.md) | 版本、构建、校验和发布规约 |

## 兼容性与数据位置

为避免从旧版迁移时丢失脚本和配置，1.0.0 沿用：

- 脚本：`%LOCALAPPDATA%\\Anjian2026\\Scripts`
- 设置：`%LOCALAPPDATA%\\Anjian2026\\settings.json`

不要擅自更改这两个路径、Lua API 语义或 `Anjian.App.exe` 文件名。任何迁移都必须提供兼容/回滚方案。

## 验收原则

GitHub Actions 的 PASS 只说明自动化构建与测试通过，不等同于用户 Windows 机器上的真实 Hook、DPI/多屏、托盘、权限和长时间稳定性验收。未经实际执行的项目必须标记 `NOT RUN`，不允许根据代码阅读或 CI 结果冒充实机 PASS。
