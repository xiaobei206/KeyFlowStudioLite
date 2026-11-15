# 当前会话交接（唯一当前状态）

## 1. 项目身份

- 产品：**KeyFlow Studio Lite**
- 仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 当前版本：`1.0.0`（新仓库 Windows CI 已 PASS；下一提交触发正式 Release）
- 平台：Windows 10/11 x64
- 技术栈：WinForms、.NET 10、Lua 5.4、NLua/KeraLua
- 本文档是当前状态唯一入口。新 GPT 只需读取仓库 URL，再读 `AGENTS.md` 和本文件；不得要求用户重复提供项目提示词。

## 2. 迁移基线

本仓库完整源码取自旧项目 `xiaobei206/anjian` 的稳定 tag `v1.0.0`，tag 指向提交：

`9e2f8d8528f34468dd0423262da6ebe56d572de6`

迁移只重定向产品名称、产品包名、发布流程和项目交接文档；不把旧仓库 main 上后续增加的 IDE、图像资源、OCR、参数设计器、小精灵、插件、多实例等完整产品功能混入 Lite。

保留的兼容项：
- 解决方案：`Anjian2026.sln`
- 应用程序集/执行文件：`Anjian.App.exe`
- 用户脚本目录：`%LOCALAPPDATA%\\Anjian2026\\Scripts`
- 设置文件：`%LOCALAPPDATA%\\Anjian2026\\settings.json`
- Lua API 与 v1.0.0 行为语义

## 3. 当前已实现功能

- 独立悬浮窗、主窗口与系统托盘生命周期。
- 本地 Lua 脚本 CRUD 和文本编辑。
- 全局键盘/鼠标录制，录制暂停/续录、click-only 模式和移动事件降噪。
- Lua Worker 独立进程运行、IPC、播放/暂停/停止、循环与轮间隔。
- 键鼠事件发送、Unicode 输入和停止/异常时释放已按下输入。
- F10/F11/F12 全局快捷键。

详细状态以 `docs/FEATURE-MATRIX-CN.md` 为准；产品边界以 `docs/LITE-SCOPE-CN.md` 为准。

## 4. 自动化验证证据与限制

迁移源版本的 Windows Actions run [43726550455](https://github.com/xiaobei206/anjian/actions/runs/43726550455) 已 PASS：Restore、Build（0 warnings/0 errors）、7/7 Core tests、自包含 Windows x64 发布、ZIP/SHA、Release 创建成功。

源 ZIP：
- `MouscriptLite-1.0.0-win-x64.zip`
- 51,911,007 bytes
- SHA256 `ffab0dd7382a927949f405b5daf39b35a490c03539e17b507f4f5cbb286c0eb2`

这只是迁移输入包证据，不代替新仓库 CI，也不代替用户真实 Windows 桌面验收。当前新仓库的 CI/Release 结论必须由新仓库 Actions run 重新记录。所有真实 Hook、DPI、多屏、托盘、权限和长时稳定性项目在取得实机证据前为 `NOT RUN`。

## 5. 新会话的唯一下一任务

**M1 — 1.0.0 基线核验与稳定性补强**

接手时先核对新仓库最新 HEAD、`v1.0.0` Release 和最新 Actions。若迁移 CI 尚未完成，先修复并完成迁移发布，不开始功能扩张。其后：

1. 将 `docs/ACCEPTANCE-TEST-CN.md` 逐项与现有代码、测试和 workflow 对照。
2. 找出录制/暂停/停止、输入释放、Worker 崩溃、托盘生命周期、设置持久化等关键路径缺失的可自动化回归测试。
3. 优先补充可确定性复现的 Core/Windows 集成测试；不要用模拟测试冒充真实键鼠验收。
4. 对发现的 P0/P1 缺陷做最小修复；每项修复配套回归测试。
5. 更新本 handoff、路线图、验收记录和累计开发日志。

本阶段不要擅自加入图像识别、OCR、插件系统、云同步或大型 IDE；先把 Lite 1.0.0 的可靠性边界做扎实。新功能是否进入范围由用户明确决定。

## 6. 重要文档入口

- 协作规则：`/AGENTS.md`
- 产品需求：`/docs/PRD-CN.md`
- 架构：`/docs/ARCHITECTURE-CN.md`
- 功能矩阵：`/docs/FEATURE-MATRIX-CN.md`
- 路线图：`/docs/ROADMAP-CN.md`
- Lua API：`/docs/LUA-API-CN.md`
- 录制器设计：`/docs/RECORDER-DESIGN-CN.md`
- 用户手册：`/docs/USER-GUIDE-CN.md`
- 验收清单：`/docs/ACCEPTANCE-TEST-CN.md`
- 开发日志：`/docs/DEVELOPMENT-LOG-CN.md`
- 决策记录：`/docs/DECISIONS-CN.md`
- 发布规约：`/docs/RELEASE-PROCESS-CN.md`

## 7. 新仓库迁移 CI 结果（已核验）

- 迁移提交：`7adc5bbe0553e02f89de74a72f1b7a7294a034a5`
- Windows Actions run：[43727695106](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43727695106)，结论 SUCCESS。
- Restore：PASS；Release Build：PASS，0 warnings / 0 errors；Core tests：7/7 PASS；self-contained win-x64 publish：PASS；ZIP/SHA：PASS；artifact upload：PASS。
- Actions artifact ID：`14167530220`；artifact 名称 `KeyFlowStudioLite-release-win-x64`；大小 51,740,914 bytes；外层 artifact digest `sha256:b38bd54e38d85080bd396db81a1e23c539cb7d3562a7ad52fa686f1f23d999cd`。该摘要是 Actions artifact 外层 ZIP 摘要，不是产品 ZIP 自身 SHA。
- 正式 Release 由下一条 `release: v1.0.0 KeyFlow Studio Lite` 提交触发；新 Release 资产和 SHA 只有在 run 成功后才能记录。

## 8. 当前提交与下一步

- 本文档当前对应迁移提交 `7adc5bbe0553e02f89de74a72f1b7a7294a034a5`。
- 新仓库 Release 成功后，应将 release commit、tag、资产文件名/大小/SHA 和 Actions run 写入此处。
- 下一会话唯一任务仍是 M1 基线核验与稳定性补强；先核对最新 HEAD/Release/Actions，不要再次迁移源码。
