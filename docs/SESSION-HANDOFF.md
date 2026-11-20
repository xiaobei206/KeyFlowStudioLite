# 当前会话交接

## 项目身份
- 产品：**KeyFlow Studio Lite**
- 仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 当前稳定版：`1.0.1`
- 开发目标版本：`1.1.0`
- 平台：Windows 10/11 x64
- 技术栈：WinForms、.NET 10、Lua 5.4、NLua/KeraLua
- 源码实现提交：`bf4987e6426e4ee9b15fd1eb9ca4dc474f709456`
- 新会话开始时以 `main` 最新 HEAD 为准，并读取本文件与根目录 `AGENTS.md`。

## 1.1.0 本轮目标
增加一个轻量的固定点击节奏选项，并改善悬浮窗空间利用率。保持原始录制坐标、Down/Up 配对、点击 holdMs 和默认播放语义兼容。

### 已实现（等待 Windows CI 验证）
- 悬浮窗改为约 520 × 140 的浅灰白色布局：四个紧凑横行，扁平浅色按钮，录制按钮淡红、播放按钮淡绿；没有深色皮肤或大型卡片区。
- 新增“固定间隔”勾选框和三个毫秒输入项：总间隔默认 400 ms、点击后默认 50 ms、移动到目标后悬停默认 150 ms。数值范围 0–60000 ms。
- 固定节奏只在勾选“只录点击”模式时生效；全鼠标轨迹录制与开关关闭时沿用原输出时间顺序。
- 连续点击的延迟安排：前一次点击抬起后先等待设置的“点击后”时长，再移动到下个点击坐标；移动完成后等待 `max(悬停时长, 总间隔 - 点击后时长)`。因此目标总间隔为 400、点击后 50、悬停最低 150 时，点击间延迟为 50 + 350 = 400 ms；若两个必要等待合计超过总间隔，则保留必要等待，实际间隔可更长。
- 第一次点击没有“上一次点击后等待”，但仍在移动到目标后应用悬停等待。
- 配置保存在 `%LOCALAPPDATA%\\KeyFlowStudioLite\\settings.json`；录制会话开始时冻结设置，暂停预览与最终脚本使用同一参数快照。
- 新增三个 Lua 生成回归测试：固定间隔分配、必要等待超过目标总间隔、关闭开关保留旧行为。
- 测试项目改为 Windows TFM，并通过 friend assembly 访问录制器内部模型。

## 验证状态
- 源码实现提交：`bf4987e6426e4ee9b15fd1eb9ca4dc474f709456`
- Windows Actions run：<https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44732976826>
- 当前状态：`QUEUED / PENDING`（尚未取得本轮 Build/Test/Package 结果，不得视为 PASS）。
- 用户 Windows 实机验收：`NOT RUN`；当前没有截图或 DPI/拖动/托盘实测证据。
- 稳定版 Release 仍为 [v1.0.1](https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.1)。通过验证后再正式发布 v1.1.0，并核实 ZIP 与 SHA256。

## 重要兼容规则
- 不勾选固定间隔时，生成器保留旧版录制间隔行为。
- 点击时长从鼠标 Down/Up 时间戳计算，绝不包含动作间隔或新增悬停等待。
- 只把连续的录制点击动作规范化为固定总间隔；穿插键盘/滚轮等其他动作时，保留其记录时间差，不能把它们伪装成纯点击序列。
- “点击后”是上一次点击结束后、开始移动前的精确等待；“悬停”是移动结束后、下一次点击前的最低等待。固定总间隔优先满足这两个设置。
- 必须保持 `VERSION`、`KeyFlow.App.csproj` 的 ProductVersion/AssemblyVersion/FileVersion 一致。
- Actions 通过不等于桌面实机验收通过。

## 下一会话唯一任务
首先核对 Actions run `44732976826`。若失败，定位并修复构建/测试错误，保持版本 1.1.0，不创建重复版本；若通过，完成文档最终核对并触发 v1.1.0 正式 Release。发布后更新本交接，记录 Release URL、ZIP 大小、SHA256、测试数量与剩余 NOT RUN 实机验收项目。
