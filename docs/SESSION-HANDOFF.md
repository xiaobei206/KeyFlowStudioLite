# 当前会话交接

## 项目身份
- 产品：**KeyFlow Studio Lite**
- 仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 当前稳定版本：**1.1.1**
- 平台：Windows 10/11 x64
- 技术栈：WinForms、.NET 10、Lua 5.4、NLua/KeraLua
- 本次发布/功能提交：`7d8ff7b5999fc9379a52ca4f4de4a7efa55ac1aa`
- 新会话必须重新读取 `main` 最新 HEAD；不要只依赖历史记录里的 SHA。

## 1.1.1 正式交付
- Release：https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.1.1（正式版，prerelease=false）
- Windows ZIP：`KeyFlowStudioLite-1.1.1-win-x64.zip`，51,916,404 bytes。
- ZIP SHA256：`803109a87168f9e6ce25a8037ff2901d81d70594d01857ef33e10e1f591dbbf1`。
- SHA256 sidecar：`KeyFlowStudioLite-1.1.1-win-x64.zip.sha256`。
- Release Actions：[44737292744](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44737292744)，SUCCESS；Restore、Build、Test、Publish、ZIP/SHA、Artifact 上传和 Release 创建均 PASS。
- Release Build：0 warnings / 0 errors；Core tests：10/10 PASS，0 failed，0 skipped。

## 1.1.1 悬浮窗视觉更新
- 根据用户实机截图，将独立悬浮窗改为 600 × 190 px；标题/脚本/状态与窗口按钮、主操作、循环和实时坐标、点击节奏分别分区。
- 播放按钮作为主要操作突出显示；录制使用低饱和红色，状态以低饱和颜色区分录制、播放、暂停和就绪。
- 完整主界面没有重做；本次没有改动 PlaybackRunner 的命名管道生命周期。
- Windows 真机 DPI、混合 DPI、多显示器、负坐标、拖动与控件无截断验收仍为 NOT RUN。

## 1.1.0 正式交付
- Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.1.0>（正式版，prerelease=false）
- Windows ZIP：`KeyFlowStudioLite-1.1.0-win-x64.zip`
- ZIP 大小：51,915,111 bytes
- ZIP SHA256：`b8af447c184807ce59516e1a90e3dd0cf7ebbcc31414185a530b4c62b06fd207`
- SHA256 sidecar：`KeyFlowStudioLite-1.1.0-win-x64.zip.sha256`
- Release 工作流：[44733253454](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44733253454)，结论 SUCCESS。
- CI：Restore、Release Build、Test、自包含发布、ZIP 打包、SHA256、Artifact 上传和 Release 创建均 PASS。
- Release Build：0 warnings / 0 errors。
- Core tests：10/10 PASS，0 failed，0 skipped；新增录制时序回归测试 3 项。
- 源码实现提交 Actions：[44732976826](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44732976826)，SUCCESS。
- 最新功能文档提交 Actions：[44733122799](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44733122799)，SUCCESS。

## 1.1.0 功能与时序规则
- 悬浮窗约 520 × 140 px，紧凑浅灰白布局、四行、无深色主题和大型卡片区；录制淡红、播放淡绿，按钮使用克制的扁平样式。
- 新增“固定间隔”勾选框及三个值：总间隔默认 400 ms、点击后等待默认 50 ms、移动到点击坐标后悬停最低等待默认 150 ms；范围 0–60000 ms。
- 固定节奏默认关闭，只在“只录点击坐标”模式生效；关闭功能或使用完整鼠标轨迹录制时沿用原本生成时间顺序。
- 连续点击：上一点击结束后先精确等待点击后时长 A，再移动到下一坐标，最后等待 `max(悬停最低等待 B, 总间隔 T - A)`。T=400、A=50、B=150 时点击间隔为 50+350=400 ms；如 A+B>T，则不缩短必要等待，实际间隔可更长。
- 第一次点击无前置点击后等待，但移动到目标后仍应用悬停等待。
- `LeftClick(holdMs)` 仍只表示 Down/Up 的按下持续时间，不包含动作间隔或悬停等待。
- 设置保存在 `%LOCALAPPDATA%\KeyFlowStudioLite\settings.json`。开始录制时冻结参数快照，暂停预览和最终 Lua 使用同一组参数。
- 新增自动化回归测试覆盖总间隔预算、必要等待超额、关闭功能后维持旧输出。
- 用户 Windows 实机验收仍为 `NOT RUN`：DPI、拖动、视觉截断、多显示器、真实 Hook、目标软件悬停菜单和长时录制尚未亲自验收。

## 发布与维护规则
- 保持 `VERSION`、App ProductVersion、AssemblyVersion、FileVersion 同步。
- 按 SemVer 递增，不覆盖正式版本或创建含糊的 FIX 版号。
- Release ZIP 必须完整包含所有自包含运行时与 Worker 文件，并发布 SHA256 sidecar。
- 自动化 CI PASS 不等于 Windows 桌面实机验收 PASS；未实测项目继续标记 NOT RUN。

## 下一会话唯一任务
先核对 main 最新 HEAD、v1.1.1 Release、ZIP/SHA 和 Actions，再按用户此前的“先讨论”要求，继续讨论但暂不改动命名管道故障。用户提供的长循环日志在第 6 轮报 `System.IO.IOException: Pipe is broken`，堆栈落在 `PlaybackRunner.RunOneAsync` 的 `StreamWriter.Dispose/Flush` 收尾阶段；这提示可能是 Worker 关闭管道后的清理异常，但根因尚未确认。该故障没有包含在 v1.1.1，也未被宣称已修复。除非用户明确同意进入修复，不改 IPC 代码。
悬浮窗实际 Windows 桌面验收同样保持 NOT RUN：需要用户确认 100/125/150% DPI、拖动、状态显示、坐标截断和实际观感。
