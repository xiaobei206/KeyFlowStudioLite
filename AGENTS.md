# AGENTS.md — KeyFlow Studio Lite 自动化开发协作规则

本文件是所有 GPT 会话、代码代理和人工贡献者的强制入口。**只提供仓库地址也应能开始工作**：新会话必须先读取本文件，再按本文档导航读取 Git 中的交接材料，不得要求用户重复描述已写入仓库的需求。

## 1. 项目身份与事实来源

- 仓库：`xiaobei206/KeyFlowStudioLite`
- 产品：KeyFlow Studio Lite
- 默认分支：`main`
- 当前产品基线：`1.0.1`
- 平台：Windows 10/11 x64；WinForms + .NET 10 + Lua 5.4
- 仓库和当前分支文档是项目状态的唯一事实来源。聊天记录只能补充，不得覆盖仓库中的明确决定。
- 根 `README.md` 是文档导航；本文件定义协作约束；`docs/SESSION-HANDOFF.md` 定义当前状态和唯一下一任务；`docs/ROADMAP-CN.md` 是开发顺序；`docs/DEVELOPMENT-LOG-CN.md` 是不可覆盖的历史记录。

## 2. 每个新会话自动执行的接手流程

用户只给出仓库地址时，不要追问项目背景或版本。按顺序：

1. 读取当前仓库默认分支、最新 HEAD、最近提交和 Release；不要相信旧会话记忆里的 SHA。
2. 必读：`README.md`、`AGENTS.md`、`docs/SESSION-HANDOFF.md`、`docs/ROADMAP-CN.md`。
3. 按本轮任务读取 `PRD`、`ARCHITECTURE`、`FEATURE-MATRIX`、`LUA-API`、`RECORDER-DESIGN`、`DECISIONS` 和 `ACCEPTANCE-TEST` 中相关内容。
4. 检查当前 HEAD 上的源码与工作区状态。若用户没有点名其他事项，严格执行 handoff 的“下一会话唯一任务”。
5. 若 handoff 和较新的用户明确要求冲突，用户最新明确要求优先；完成后更新 handoff 解释冲突如何解决。

## 3. 开发与 Git 规则

- 默认直接在 `main` 进行个人项目的小步提交；除非任务明确需要隔离实验或用户要求，不要自行创建复杂分支/PR流程。
- 开始修改前核对 HEAD。每次提交前检查 diff，避免覆盖其他会话新提交。
- 一个提交只做一个逻辑目的；不创建含糊的 “FIX” 版本，不擅自跳版本。
- 不允许只改文档声称功能完成；实现、测试、文档与版本必须一致。
- 不允许为“看起来更现代”而整体重写 UI。优先解决可复现问题，控制改动面。
- 用户要求的范围优先；不得擅自将 Lite 扩成完整 IDE、图像识别平台或云服务。
- 保留历史内部路径和兼容行为，除非有明确迁移方案并经过验证。

## 4. 版本与发布规约

- `VERSION`、`PRODUCT_NAME`、`PRODUCT_ID`、`src/KeyFlow.App/KeyFlow.App.csproj` 的版本必须一致。
- 当前稳定基线为 `1.0.1`。下一次正式/预发布版本必须按 SemVer 递增，例如 `1.0.2` 或 `1.1.0-preview.1`；禁止重复覆盖同版本来伪装修复。
- 只有在 Windows Actions 构建、测试、打包和 SHA256 均通过后才允许发布。
- Release ZIP 必须包含所有 self-contained 伴随文件；同时发布 `.sha256` sidecar。
- Release 标签使用 `vX.Y.Z`；提交信息使用 `release: vX.Y.Z <purpose>` 触发发布 workflow。
- 发布完成后必须核对 Release 标签、正式/预发布属性、ZIP 名称、文件大小、SHA256 和下载 URL。
- CI 通过不是 Windows 桌面实机验收通过；两者必须分别记录。

## 5. 技术与安全边界

- 1.0.1 使用 KeyFlow 作为解决方案、项目目录、程序集、命名空间和应用程序身份。旧版用户数据通过 `AppDataPaths` 做非破坏性导入；不得删除旧数据目录。

- UI 不直接调用 Win32 输入 API；脚本运行必须在独立 Worker 中，不能阻塞 UI 线程。
- 输入按下与释放必须成对；取消、异常、停止和退出路径必须释放所有可能仍按下的鼠标/键盘输入。
- Lua API 的参数、默认值、时间单位、坐标语义必须以 `docs/LUA-API-CN.md` 为准；变更时同步测试和文档。
- 不要将 Hook 回调异常传播到系统输入链；不要吞掉用户可见的执行错误。
- 本地脚本和配置默认留在用户本机；不引入遥测、上传脚本、云同步或网络执行功能，除非用户明确批准。
- 不执行未授权的安全绕过、隐蔽持久化或对第三方系统的滥用自动化。

## 6. 验证规约

- 每次代码变更至少执行可行的构建和相关测试；Windows-only 行为必须由 Windows runner 或用户实机验证。
- 记录具体命令、结果、测试数量、Build warnings/errors、CI run URL 和未执行项目。
- 测试状态只允许：`PASS`、`FAIL`、`BLOCKED`、`NOT RUN`。没有证据不得 PASS。
- 对无法在 CI 验证的 Hook、物理键鼠、DPI、多显示器、托盘和长时稳定性，使用验收清单逐项记录。
- 不因没有实机环境而停止一切工作；能做的代码审查、单测、CI、文档和可重复烟测照常完成。

## 7. 每个会话结束时必须更新的文档

1. 更新 `docs/SESSION-HANDOFF.md`：当前 HEAD、版本、CI、已完成项、阻塞、唯一下一任务。
2. 追加 `docs/DEVELOPMENT-LOG-CN.md`，不得覆盖已有历史。
3. 按需更新 `docs/ROADMAP-CN.md`、`docs/FEATURE-MATRIX-CN.md`、`docs/ACCEPTANCE-TEST-CN.md` 和决策记录。
4. 保持 `README.md` 下载链接与最新稳定版一致。
5. 最终提交后再次读取 main HEAD，确保 handoff 指向最终提交。
