# 当前会话交接

## 项目身份
- 产品：**KeyFlow Studio Lite**
- 仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 当前稳定版本：**1.1.0**
- 平台：Windows 10/11 x64
- 技术栈：WinForms、.NET 10、Lua 5.4、NLua/KeraLua
- 本次发布提交：`84bd6b80746c43e1022d635eeccac535f6f1de50`
- 本次功能实现提交：`bf4987e6426e4ee9b15fd1eb9ca4dc474f709456`
- 新会话必须重新读取 `main` 最新 HEAD；不要只依赖历史记录里的 SHA。

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
先重新核对 main HEAD、v1.1.0 Release、ZIP/SHA 和 Actions。然后优先完成 Windows 实机验收：固定节奏设置持久化、悬浮窗布局/DPI/拖动、悬停菜单任务、暂停预览与最终脚本一致性、长时间录制与停止释放。实机没有完成前不要将其标为 PASS；如发现问题，按 SemVer 发布新补丁/次版本，不重用 1.1.0。
