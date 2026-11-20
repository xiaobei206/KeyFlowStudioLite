# KeyFlow Studio Lite

**KeyFlow Studio Lite** 是一款面向 Windows 10/11 x64 的本地桌面键鼠自动化工具。通过全局键鼠录制、Lua 5.4 脚本、独立 Worker 和紧凑悬浮控制窗，完成可重复的桌面操作。

> 本仓库是唯一项目交接入口。新会话只需读取仓库地址、根目录 `AGENTS.md` 和 `docs/SESSION-HANDOFF.md`，无需用户重复提供提示词。

## 当前状态

- 当前稳定版本：**1.0.1**
- 开发中版本：**1.1.0**（固定点击节奏、点击后/悬停等待与浅色紧凑悬浮窗；Windows CI 验证中）
- 技术栈：WinForms + .NET 10 + Lua 5.4（NLua/KeraLua）
- 平台：Windows 10/11 x64
- 默认分支：`main`
- 稳定版 Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.1>
- Windows ZIP：`KeyFlowStudioLite-1.0.1-win-x64.zip`（51,911,747 bytes）
- SHA256：`19becd2a22f896126e6dfad39ef38ee6e7a755d006a6ea3e5911292efd12af42`
- Windows Actions：[43729901294](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43729901294) SUCCESS；7/7 tests，0 warnings/errors，自包含发布与 ZIP/SHA 校验通过。
- 下载入口：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/latest>
- 项目交接：[`docs/SESSION-HANDOFF.md`](docs/SESSION-HANDOFF.md)
- 自动化开发规则：[`AGENTS.md`](AGENTS.md)

## 下载与运行

从 [Releases](https://github.com/xiaobei206/KeyFlowStudioLite/releases) 下载 `KeyFlowStudioLite-1.0.1-win-x64.zip` 和同目录的 `.sha256` 文件。先校验 SHA256，再完整解压并运行 `KeyFlowStudioLite.exe`。不要单独提取 EXE；Worker、Lua 和 .NET 运行时伴随文件必须完整保留。

## 功能范围

- 独立悬浮控制窗、完整主窗口与系统托盘生命周期。
- 本地 Lua 脚本新建、打开、保存、重命名、复制和删除。
- 全局鼠标/键盘录制、暂停/续录、只录点击坐标与移动降噪。
- 鼠标移动、左右中键、按下/释放、滚轮、键盘单键、组合键和 Unicode 文本输入。
- 独立 Worker 运行 Lua；播放、暂停、停止、循环次数、轮间隔和异常日志。
- F10 播放、F11 暂停/继续、F12 中止。
- 约 500 px 紧凑悬浮窗，脚本名、状态、录制/播放操作、循环设置与实时坐标均保持可见。

完整边界见 [Lite 范围](docs/LITE-SCOPE-CN.md) 和 [功能矩阵](docs/FEATURE-MATRIX-CN.md)。

## 开发环境与构建

需要 Windows 10/11 x64、.NET 10 SDK 和 Git。在仓库根目录执行：

```powershell
dotnet restore KeyFlowStudioLite.sln
dotnet build KeyFlowStudioLite.sln -c Release --no-restore
dotnet test tests/KeyFlow.Core.Tests/KeyFlow.Core.Tests.csproj -c Release
dotnet publish src/KeyFlow.App/KeyFlow.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o artifacts/publish/win-x64
```

源码使用 `KeyFlow.App` / `KeyFlow.Core` 命名空间；解决方案、项目目录、程序集和 EXE 均使用 KeyFlow 名称。

## 文档导航

| 文档 | 用途 |
|---|---|
| [AGENTS.md](AGENTS.md) | AI/人工协作强制协议；新会话唯一必读入口 |
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
| [DECISIONS](docs/DECISIONS-CN.md) | 架构、兼容与迁移决策 |
| [RELEASE-PROCESS](docs/RELEASE-PROCESS-CN.md) | 版本、构建、校验和发布规约 |

## 用户数据与升级

当前数据目录为 `%LOCALAPPDATA%\\KeyFlowStudioLite`，其中包含 `Scripts` 和 `settings.json`。1.0.1 首次启动时会把旧版数据中尚不存在于新目录的文件复制过来；不会覆盖新目录已有文件，也不会删除旧目录，便于回滚。

## 验收原则

GitHub Actions 的 PASS 只证明自动化构建、测试、自包含发布和包校验通过，不等同于用户 Windows 机器上的真实 Hook、DPI/多屏、托盘、权限和长时间稳定性验收。未实际执行的项目必须标记 `NOT RUN`。
