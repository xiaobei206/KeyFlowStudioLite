# KeyFlow Studio Lite 技术架构

## 1. 当前实现概览

应用采用单个 WinForms 主进程 + 独立 Lua Worker 子进程：

```text
Anjian.App.exe
  ├─ MainForm / FloatingForm / NotifyIcon
  ├─ ScriptRepository / AppSettingsStore
  ├─ GlobalRecorder / GlobalHotkeyManager
  └─ PlaybackRunner
        ├─ NamedPipeServerStream
        └─ WindowsInputController
              ↑ Named Pipe JSON IPC
Anjian.App.exe --worker <pipeName> <scriptPath>
  └─ WorkerProgram + NLua/KeraLua (Lua 5.4)
```

`Anjian.Core` 提供播放选项等轻量公共模型；`Anjian.Core.Tests` 验证 Lua runtime 与循环参数。

## 2. 目录职责

- `src/Anjian.App/MainForm.cs`：脚本列表、编辑区、录制/播放控制、日志、主窗体和托盘生命周期。
- `src/Anjian.App/FloatingForm.cs`：无边框独立悬浮控制窗，显示状态、实时坐标、录制/播放和循环设置。
- `src/Anjian.App/Services/GlobalRecorder.cs`：全局键鼠低级 Hook、录制会话、事件缓存、Down/Up 配对和 Lua 生成。
- `GlobalHotkeyManager.cs`：F10/F11/F12 全局快捷键分发。
- `PlaybackRunner.cs`：循环调度、Worker 生命周期、命令 IPC、暂停门和停止/异常清理。
- `WorkerProgram.cs`：Lua 5.4 VM 初始化、命令注册、沙箱限制和 Named Pipe 客户端。
- `WindowsInputController.cs`：SendInput/SetCursorPos、键鼠按下释放、Unicode 文本和输入清理。
- `ScriptRepository.cs`：本地 Lua 文件 CRUD 和安全文件名。
- `AppSettingsStore.cs`：本机设置 JSON 读写。
- `src/Anjian.Core`：播放循环配置与校验。
- `tests/Anjian.Core.Tests`：Lua 与播放选项的确定性测试。

## 3. 线程与进程边界

- WinForms 运行在主 UI 线程。
- Lua 在独立 Worker 进程运行，不允许直接占用主 UI 线程。
- Worker 通过命名管道逐条发送 JSON 命令；主进程负责校验/执行 Windows 输入。
- 播放暂停在命令边界生效；Up 命令在暂停时仍须可执行，避免按键或鼠标按钮被卡住。
- Stop 会取消 token、终止 Worker 并执行 ReleaseAll。
- 全局 Hook 回调必须快速返回，不允许将异常传播到系统消息链。

## 4. 输入和录制语义

- 鼠标坐标为屏幕物理像素，沿用 Windows 虚拟桌面坐标语义；副屏负坐标不能擅自裁剪。
- 默认 click-only 只记录点击时坐标，忽略鼠标移动轨迹；关闭后才记录降噪移动。
- 录制暂停时卸载 Hook，并封闭尚未释放的输入事件；恢复时重新安装 Hook。
- 鼠标/键盘录制忽略本程序自身窗口和注入输入。
- 录制生成 Lua 必须保持操作顺序、Down/Up 配对和合理 Delay。

## 5. 数据持久化

- 脚本：`%LOCALAPPDATA%\\Anjian2026\\Scripts`
- 设置：`%LOCALAPPDATA%\\Anjian2026\\settings.json`
- 继续沿用历史目录是兼容决策。任何改路径必须先提供迁移、备份和回滚方案。

## 6. 发布结构

- Framework：`net10.0-windows`
- RID：`win-x64`
- Self-contained，非 single-file；ZIP 必须保留所有 publish 输出文件。
- 入口 EXE：`Anjian.App.exe`
- Release 附带 ZIP 与 SHA256 sidecar。

## 7. 架构约束与待改进

- 将来拆分服务类或 UI 文件时必须保持职责边界和行为不变；避免大规模无测试重构。
- 当前主窗口代码集中度偏高，可在有测试保护的前提下逐步拆分。
- 当前单测覆盖面较窄；下一阶段优先补充录制器事件配对、Worker 取消、输入释放、设置持久化和生命周期测试。
- 真实键鼠 Hook、DPI/多屏、托盘和长时稳定性依然需要 Windows 实机验收。
