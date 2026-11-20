# 累计开发日志

本文件按时间追加记录，不覆盖历史。当前状态以 SESSION-HANDOFF.md 为准。

## 2026-11-12 — S000 — 仓库初始化与文档框架

### 目标
在 xiaobei206/KeyFlowStudioLite 建立KeyFlow Studio Lite 的唯一长期项目资料库和 GPT 会话接力机制；不开始功能编码。

### 已完成
- 核实仓库为 public，默认分支 main，连接权限为 admin/push。
- 创建 README.md，作为文档导航和项目入口。
- 建立 AGENTS.md，约定 GPT 会话开始、Git、验证、范围控制和强制 handoff 流程。
- 建立 PRD：首版产品范围、P0/P1/P2、用户流程和完成定义。
- 建立 ARCHITECTURE：项目拆分、依赖方向、线程模型、Lua 取消和输入边界。
- 建立 LUA-API：Lua 5.4 与 Mouse / Keyboard / Delay / Log API 草案。
- 建立 RECORDER-DESIGN：Raw Input/Hook PoC、动作模型、双模式同步和数据保存规则。
- 建立 ACCEPTANCE-TEST：P0 功能、录制、停止、稳定性、DPI 和发布验收。
- 建立 TECHNICAL-VALIDATION：Lua 绑定、编辑器、输入采集和便携发布 PoC。
- 建立 ROADMAP、DECISIONS、SESSION-PROTOCOL、SESSION-HANDOFF、最短接手提示词及 src/tests 目录说明。

### 技术核对记录
- Microsoft 生命周期资料将 .NET 10 列为 LTS，生命周期截至 2028-11-14。
- Microsoft LowLevelKeyboardProc 文档提醒钩子回调必须迅速返回；Windows 可能静默移除超时的低级钩子，并建议考虑 Raw Input。
- 因此录制器不预先锁定低级钩子，先做 Raw Input 和 Hook 对比 PoC。
- Lua 绑定与编辑器组件均未锁定具体 NuGet 版本；必须先实测 .NET 10、Lua 5.4、原生依赖、许可证和 self-contained 发布。

### Git 与验证
- 初始 README 提交：c6b4cf436e3de87accb95650a5e7d52321336161
- 文档规格批次提交：a595e1dd9e937e2e63b697526a5944497b7c13a1
- S000 最终提交：306ed5dc85c36576c52452a499551e4a8f9abc70
- 目录/文件落库：通过 GitHub Contents API 复读确认。
- 应用构建/测试：NOT RUN（本轮没有应用代码，符合范围）。

## 2026-11-12 — S001 — 首版交互与 API 语义冻结

### 目标
根据用户对首版交互的确认，统一脚本语法、录制移动合并、坐标语义和点击按下持续时间，避免规格分叉。

### 已确认
- Lua 用户 API 采用全局函数形式：`MoveTo(122, 100)`，不是自创 DSL，也不强制命名空间。
- 录制鼠标移动合并/降噪，保留必要路径采样点和终点；点击、按下、释放、滚轮顺序完整。
- 坐标使用虚拟桌面绝对物理像素，允许负坐标；首版不做窗口相对坐标。
- Click / KeyPress 默认按下 50 ms 后释放；显式参数指定持续时间，0 表示不增加等待。
- 长按使用 Down / Delay / Up。Click / KeyPress Stop 时立即释放，Pause 在当前原子动作结束后生效。
- 已记录 ADR-014 至 ADR-017，统一 API、架构、录制器文档。

### 外部依赖初步核对（尚未等于采用）
- NuGet 页面显示 NLua 1.7.9（2026-05-01 更新）为当前可见版本候选；KeraLua 上游 releases 显示 v1.4.11，仍需核对与 NLua 的版本依赖关系、许可证和原生资产。
- 不据此直接锁定依赖；TV-01 必须构建和执行实际 PoC。
- 参考：https://www.nuget.org/packages/NLua
- 参考：https://www.nuget.org/packages/KeraLua
- 上游：https://github.com/NLua/NLua
- 上游：https://github.com/NLua/KeraLua

### Git / 测试
- 本次为规格文档更新；提交 SHA 以本条记录所在的 GitHub commit 为准。
- 应用构建和测试：NOT RUN（仍没有应用代码或 PoC）。

### 下一步
TV-01：Lua 5.4 / .NET 10 绑定与中断机制 PoC。


## 2026-11-12 — S002 — 首个可用 Windows 预览版

### 实现内容
- .NET 10 WinForms 主程序、Core 项目和自动化测试项目。
- 脚本本地存储、编辑器、新建/打开/保存/重命名/复制/删除及右键菜单。
- NLua 1.7.9 / Lua 5.4 worker 子进程；脚本通过 IPC 请求主进程执行键鼠输入、延时和日志。
- Windows SendInput / SetCursorPos 鼠标键盘输入，输入状态跟踪和 Stop 清理。
- 全局低级鼠标/键盘 Hook 录制；过滤注入事件和本程序窗口操作，移动合并，Down/Up 配对，键盘自动重复过滤。
- Lua 点击/按键默认 50ms，显式按下时长，双击快捷 API，常见标点键和数字小键盘键。
- 播放次数默认 1，0=无限；每轮间隔默认 2000ms，只在两轮之间等待，末轮后不等待；设置持久化。
- 长文本 Unicode 注入可在字符边界响应 Stop；代理对按 UTF-16 序列注入。
- GitHub Actions Windows build/test/self-contained ZIP/SHA256 与 release 流程。

### 最新 CI 结果（测试提交 fbf208f）
- Workflow run：43222878724，SUCCESS。
- Release build：0 warning / 0 error。
- Tests：7 passed / 0 failed / 0 skipped。
- Lua runtime 实测：Lua 5.4、Lua 调用 C# 注册函数、instruction count hook 触发。
- win-x64 self-contained publish 成功。
- ZIP 上传成功，Actions artifact ID 13947200664。

### 产物复核
- ZIP：Anjian2026-0.1.0-preview.1-win-x64.zip
- 大小：51,879,713 bytes。
- SHA256：baceea2b0c0b2a63783851ff942e049a6d59a8a0a5a23e957a952966a78f4135。
- 已下载 Actions artifact 并检查内部 ZIP，276 个文件，主程序 EXE 与 NLua.dll 存在。
- sidecar SHA256 与实际 ZIP 计算结果一致。

### 限制与验收
- 本次 CI 和包结构验证通过，但尚无用户 Windows 实机验收证据。
- 不将 2 小时稳定性、复杂拖拽、混合 DPI、多显示器和所有权限目标标记 PASS。
- 当前不是完整 IDE，也没有可视化动作列表；录制结果直接生成 Lua。

### 当前任务
发布 GitHub prerelease v0.1.0-preview.1，并核验正式 Release 的 ZIP 和 SHA256 资产。


### Release 工作流修复（13:05 UTC）
- 首次 Release 工作流的构建、7 项测试、自包含发布、打包与上传全部成功；仅 PowerShell 输出文件步骤因误用 Set-Content -Append 失败。
- 已改用 Add-Content 写入 GITHUB_OUTPUT。
- 修复提交：21441470ca6f8de57df5cc04ba46359d2cb2b828。
- 修复提交 Actions run：43223670738，结论 SUCCESS；Release 条件步骤按预期跳过（该提交不是 release: 前缀）。
- 本次 release: 提交将触发 v0.1.0-preview.1 tag 和 prerelease 创建。


## 2026-11-12 — S002 — v0.1.0-preview.1 GitHub prerelease 正式发布

### 最终发布信息
- Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v0.1.0-preview.1>
- Tag：`v0.1.0-preview.1`
- 发布提交：`cf24ac9292ea21dd1063ea9e9d65f18e3637442a`
- Release workflow run：`43224009158`，结论 SUCCESS。
- Assets：Windows x64 ZIP + SHA256 sidecar。
- ZIP 大小：51,879,710 bytes。
- SHA256：`80bfbf3ddbebafaf6e835bc8fb29148760662418b2c57de79f7ab7dbe88e6164`。
- GitHub Release API 中 ZIP asset digest 与 Actions artifact 内实际 ZIP 文件 sha256、sidecar 三方一致。
- Release ID：442961207，prerelease=true。

### PowerShell 发布修复
- 首次发布尝试中 PowerShell Set-Content 没有 -Append 参数，导致 output 变量步骤失败；构建、7 项测试和 ZIP 上传本身已成功。
- 改为 Add-Content 写入 GITHUB_OUTPUT，修复提交 Actions SUCCESS。
- 后续 release: 提交重新执行全流程，Read release version 和 Create GitHub Release 均 SUCCESS。

### 验收边界
- CI 编译和 7 项自动测试 PASS。
- 用户 Windows 实机手工验收、2 小时稳定性、多屏/DPI 与具体目标程序兼容仍为 NOT RUN。


## 2026-11-12 — S003 — 经典全局快捷键

### 用户需求
用户明确要求沿用按键精灵的熟悉快捷键，尤其需要能够在脚本运行时直接中止，不必切回主窗口。

### 实现
- 默认 F10 启动当前脚本、F11 暂停/继续、F12 中止当前脚本。
- 新增全局低级键盘钩子，并将控制消息投递到 WinForms 窗口消息队列。
- 仅截获物理按键；过滤 SendInput 等模拟注入事件，避免脚本自行发送功能键时误触发。
- 对按住功能键产生的自动重复进行抑制。
- 空闲时只接管 F10；运行或暂停时才接管 F11/F12；无对应动作时功能键正常传递给前台程序。
- 主工具栏、README、PRD、验收清单和用户手册补充快捷键说明。

### 提交
- 实现：`5ce20ee50542bed34dad86aae89c5dd045767803`
- 状态按需截获修正：`e8e96cb26df1a254d2db45dd2a72b2c7ef11cd9f`
- 文档：`55afa24e54d3614b9e25088fd65b0dc864ac117b`
- 最终代码提交 Windows Actions：待完成。

### 验证状态
- 初版代码 Windows CI：`43226021330`，SUCCESS（编译、7 项测试、自包含发布及 ZIP 打包均通过）。
- 状态按需截获修正版 Windows CI：待完成。
- Windows 10/11 实机全局快捷键、最小化状态、键鼠释放与冲突测试：NOT RUN，必须由用户实测。

### 版本安排
- 作为 v0.1.0-preview.2 的功能增量；preview.1 既有资产保持不变。


## 2026-11-12 — S003 — 全局热键与 v0.1.0-preview.2 发布完成

### 用户需求
用户明确提出脚本运行时必须有按键精灵熟悉的快捷键，避免无法快速中止。

### 实现
- F10：应用空闲且未录制时启动当前脚本。
- F11：脚本运行/暂停时切换暂停与继续。
- F12：脚本运行/暂停时立即中止并清理本程序持有的按键和鼠标按钮。
- 使用全局低级键盘钩子；过滤模拟注入按键，避免脚本自身发送 F10/F11/F12 导致误触发。
- 只在有对应动作时截获按键：空闲时 F12 仍可交给浏览器打开开发者工具；运行期间 F12 被用作中止键。
- 主界面、README、PRD、用户手册、验收清单、handoff 和 roadmap 已同步说明。

### Git 提交
- 快捷键实现：`5ce20ee50542bed34dad86aae89c5dd045767803`
- 状态按需截获修正：`e8e96cb26df1a254d2db45dd2a72b2c7ef11cd9f`
- 文档更新：`0bcff7373319a47cc4253309238f3603c7eab81d`
- preview.2 release：`3e63ff38d8111181fa1a402547350c80e6ceb882`

### CI 与发布
- 最终代码提交 Actions run：`43226322656`，SUCCESS。
- Release Actions run：`43226666516`，SUCCESS；Build、Test、Publish、Package、Artifact Upload、Read Release Version、Create GitHub Release 全部成功。
- 自动化测试：7 passed / 0 failed / 0 skipped。
- Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v0.1.0-preview.2>
- ZIP：`Anjian2026-0.1.0-preview.2-win-x64.zip`，51,881,918 bytes。
- SHA256：`41139ffeefd695bce3e1f00f232fc8d2fc1cb7b34b67f8bfb53d68019c0bd089`。
- Sidecar 与实际 ZIP SHA256 已通过本地校验；Release asset ZIP digest 与文件 SHA 一致。

### 验收边界与下一步
- CI 编译、自动化测试、自包含发布及 Release 资产核验：PASS。
- 用户 Windows 10/11 实机 F10/F11/F12 热键测试：NOT RUN。
- 2 小时稳定性、混合 DPI、多显示器、复杂拖拽和不同权限级别目标程序兼容：仍 NOT RUN。
- 下一步仅优先做用户实机验收与反馈修复，不要擅自扩大首版范围。


## 2026-11-12 — S004 — 点击坐标录制、续录和紧凑悬浮窗（待 CI）

### 用户需求
- 可选只保留点击位置，忽略手部抖动带来的鼠标移动轨迹。
- 录制过程中可以暂停并继续，之前录制内容不清空。
- 默认紧凑悬浮窗，显示脚本名和实时鼠标坐标，支持完整模式切换与始终置顶。

### 本轮代码变更
- GlobalRecorder 增加 click-only 采集模式，忽略移动事件；生成代码按 MoveTo -> 可选 Delay -> Click 的顺序执行。
- 增加 Pause/Resume 录制会话，暂停时卸载全局钩子、闭合已按住输入，恢复时排除暂停时长。
- MainForm 增加紧凑/完整两种模式、拖动标题区、实时坐标、置顶切换、状态化录制按钮。
- 新增 AppSettings 持久化录制模式、置顶和窗口模式；默认只记录点击坐标、紧凑模式启动。
- 用户手册和验收清单增加对应说明与 B16-B18 测试项。

### 验证状态
- 本地工具链未执行 Windows 编译；等待 GitHub Actions Windows runner 验证。
- Windows 实机交互和 UI 布局：NOT RUN。
- 若 CI 通过，再将版本递增至 v0.1.0-preview.3 并发布；不覆盖 preview.2。


## 2026-11-12 — S004.1 — Windows CI 第一轮通过

- 功能提交 `659db9836c1ab51c7e8812958eb1fee7166fa9ad` 对应 Actions run `43231551625`：SUCCESS。
- Build、Test、self-contained publish、ZIP/SHA256 package、artifact upload 全部通过。
- 随后在紧凑悬浮窗顶部增加“只录点击”勾选，commit `3d79665418869487405e74c12d0f4308f404bbf1`，其 CI run `43231710764` 已通过 Build/Test/Publish/Package。
- 正在递增版本至 v0.1.0-preview.3，并由 Release workflow 对最终代码再次完整验证。


## 2026-11-12 — S004.2 — v0.1.0-preview.3 正式发布

### CI 与发布证据
- 首轮 release run `43232299395` 因 `MainForm.cs(492,55)` nullable warning-as-error 失败，未发布资产。
- 修复：录制期间恢复脚本列表选择时，对 `_current` 做 null 检查。修复提交 `2eacde0c12f9616e4f37d209c929c1b473edab43` 的 run `43232594517` SUCCESS。
- 最终 release commit：`3938b1550f32cb98511b67cfb5642e6eaf78f8fe`。
- Release run `43232898372` SUCCESS；Build/Test/Publish/Package/Artifact Upload/Read Release Version/Create GitHub Release 全部成功。
- 自动化测试 7/7；构建 0 warnings / 0 errors。

### 发布资产
- Release：https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v0.1.0-preview.3
- ZIP：`Anjian2026-0.1.0-preview.3-win-x64.zip`，51,887,704 bytes。
- SHA256：`b18071d5ecfb05435198e5ee900e59cc861ef7706328b413bee2412650acf188`。
- 已从 Actions artifact 提取 ZIP 和 sidecar；`sha256sum -c` 输出 OK，GitHub asset digest 与实际文件 SHA256 一致。

### 验收边界
- CI / Release 资产：PASS。
- 用户 Windows 实机点击坐标录制、暂停续录、紧凑 UI、置顶与坐标刷新：NOT RUN。


## 2026-11-12 — S005 — 独立悬浮窗、系统托盘与应用图标（preview.4 开发中）

### 用户反馈与产品决策
- preview.3 的“悬浮模式/完整模式”其实是同一个 MainForm 中切换面板，悬浮窗不独立；完整模式会让悬浮面板消失。
- 确认悬浮窗与完整主窗口必须是两个独立顶层窗口，可同时显示并共享同一状态。
- 主窗口关闭或点“返回悬浮窗”时应隐藏主窗口、移除任务栏按钮，应用继续由托盘图标维持。
- 托盘菜单可打开主窗口、显示/隐藏悬浮窗和退出程序；悬浮窗关闭只隐藏窗口。
- 悬浮窗直接提供循环次数和轮间隔输入，与主窗口实时同步。
- 用户要求更新应用图标，且图标必须应用于 EXE 和托盘。

### 已落地代码（等待 CI）
- 新增独立 `FloatingForm`；主窗口保留脚本列表、Lua 编辑器、日志和完整设置。
- 启动时隐藏主窗口（ShowInTaskbar=false、Opacity=0），默认显示独立悬浮窗。
- 完整界面按钮仅显示主窗口，不关闭悬浮窗；主窗口“返回悬浮窗”和右上角关闭都只隐藏主窗口。
- 新增 NotifyIcon 和托盘菜单；显式“退出KeyFlow Studio Lite”才关闭两个窗口、释放 Hook/播放资源。
- 悬浮窗增加循环次数和轮间隔控件，并与主窗口 NumericUpDown 双向同步。
- 自定义多尺寸 ICO 已提交到 `src/Anjian.App/Resources/Anjian2026.ico`；`ApplicationIcon` 已嵌入 EXE，主窗体、悬浮窗和 NotifyIcon 使用同一图标。
- Windows CI run `43252971971` 和最新代码 run `43253075450` 均 SUCCESS；Restore/Build/7 项测试/Publish/ZIP/SHA256/Artifact upload 全部完成。
- 正式 prerelease 尚待 release: commit；Windows 10/11 实机窗口、托盘和图标验收仍为 NOT RUN。

### 验证
- 源码结构检查：已完成。
- Windows 构建、测试、打包：待执行。
- Windows 10/11 实机窗口/托盘/图标验收：NOT RUN。

- CI 通过后的发布触发提交：`release: publish v0.1.0-preview.4`；Release workflow 再次从该提交构建并创建 prerelease。


## 2026-11-15 — Lite 1.0.0 独立迁入 KeyFlowStudioLite

- 源码来源：xiaobei206/anjian tag v1.0.0，commit 9e2f8d8528f34468dd0423262da6ebe56d572de6。
- 迁移目标：xiaobei206/KeyFlowStudioLite，默认分支 main。
- 保留 Lite 1.0.0 既有实现与 Lua API；更新产品显示名、PRODUCT_NAME/PRODUCT_ID、README、AGENTS、CI/Release 工作流和项目交接文档。
- 保留 Anjian2026.sln、Anjian.App.exe、%LOCALAPPDATA%\\Anjian2026\\Scripts、settings.json，避免已有用户脚本/配置迁移风险。
- 新仓库 CI 和 Release 以新仓库 Actions 与 GitHub Release 为准；旧仓库 CI 仅证明迁移输入源版本可构建，不替代新仓库验证。


## 2026-11-15 — KeyFlow Studio Lite 1.0.0 独立仓库迁移验证

- 新仓库：`xiaobei206/KeyFlowStudioLite`，默认分支 `main`。
- 源码来自旧仓库 tag `v1.0.0`，源 commit `9e2f8d8528f34468dd0423262da6ebe56d572de6`；完整迁移源码、ICO、测试、解决方案和文档。
- 迁移提交：`7adc5bbe0553e02f89de74a72f1b7a7294a034a5`。
- 产品品牌改为 KeyFlow Studio Lite，包 ID 为 `KeyFlowStudioLite`；旧的 Anjian 解决方案/EXE 名和本机用户数据目录保持兼容。
- 新仓库 Windows Actions run [43727695106](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43727695106) SUCCESS：0 Build warnings、0 errors、7/7 Core tests、self-contained win-x64 publish、ZIP/SHA 与 artifact upload 全 PASS。
- Actions artifact ID `14167530220`，外层归档 51,740,914 bytes，digest `sha256:b38bd54e38d85080bd396db81a1e23c539cb7d3562a7ad52fa686f1f23d999cd`。
- 新仓库正式 `v1.0.0` Release 将由独立的 release commit 触发；未经核对 Release API 与资产清单前，不得声称已发布。


## 2026-11-15 — KeyFlow Studio Lite v1.0.0 新仓库正式发布

- Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.0>，Release ID `446142271`，正式稳定版，prerelease=false。
- Release commit：`909559341cf7d749f4a151219a175c716db37af5`。
- Windows Actions run [43727898613](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43727898613) SUCCESS：Restore、Build（0 warnings / 0 errors）、7/7 Core tests、self-contained win-x64 publish、ZIP/SHA、artifact upload、Release creation 全 PASS。
- 发布 ZIP：`KeyFlowStudioLite-1.0.0-win-x64.zip`，51,911,083 bytes；SHA256 `f9e33e319f725fbc94d85d73657de878f2f930f869276d2d0b0032936f9f7a90`。
- SHA sidecar：`KeyFlowStudioLite-1.0.0-win-x64.zip.sha256`。
- 新仓库完成源码迁移与 1.0.0 发布；下一阶段按 handoff 执行 M1 基线核验和稳定性补强。


## 2026-11-15 — 1.0.1 — KeyFlow 内部命名统一与紧凑悬浮窗

### 变更
- 统一解决方案、项目目录/项目名、命名空间、程序集、EXE、应用清单和 Worker IPC 前缀为 KeyFlow。
- 将用户数据目录更新为 `%LOCALAPPDATA%\\KeyFlowStudioLite`，首次启动非破坏性导入旧目录缺失文件；新文件优先、旧目录保留。
- 悬浮窗客户区宽度从 640 px 调整到 500 px；缩短标题区、动作按钮和循环设置控件，保留实时 X/Y 坐标。
- 更新 README、协作入口、交接、路线图、发布流程、用户手册、功能矩阵和验收项目。

### 验证
- Windows Actions [43729901294](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43729901294)：SUCCESS。
- Restore / Build：PASS，0 warnings / 0 errors；Core tests：7/7 PASS。
- self-contained win-x64 publish、ZIP/SHA256、Artifact upload、Release creation：全部 PASS。
- 正式 Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.1>，prerelease=false。
- ZIP：`KeyFlowStudioLite-1.0.1-win-x64.zip`，51,911,747 bytes；SHA256：`19becd2a22f896126e6dfad39ef38ee6e7a755d006a6ea3e5911292efd12af42`。
- Windows 10/11 实机悬浮窗布局、数据迁移、Hook、DPI/多屏、托盘验收：NOT RUN。


## 2026-11-20 — 1.1.0 — 固定点击节奏与紧凑浅色悬浮窗（实现提交，等待 CI）

### 用户需求与设计决定
- 在悬浮窗增加固定点击间隔选项，解决录制中出现 900/1200 ms 等不希望保留的非固定点击空隙。
- 同时提供总间隔、点击完成后开始移动前的等待、移动到目标后悬停等待三个毫秒值。
- 三项等待采用组合预算：点击后等待固定为用户填写的数值；连续点击之间的移动后等待为 `max(悬停等待, 总间隔 - 点击后等待)`。如果点击后与悬停的必要等待已经超过总间隔，不压缩必要等待，允许实际间隔更长。
- 固定节奏仅在勾选“只录点击”时启用；关闭固定节奏或录制完整鼠标轨迹时沿用旧生成顺序。
- 悬浮窗定位为浅色、轻量、小幅增高的四行布局，目标尺寸 520 × 140 px；不采用深色主题或大型卡片。

### 实现
- 新增 settings 字段并设置兼容默认值：FixedClickInterval=false、FixedClickIntervalMilliseconds=400、PostClickDelayMilliseconds=50、HoverDelayMilliseconds=150。
- 录制会话开始时复制节奏参数；暂停预览和最终 Lua 使用同一参数快照。
- 延迟值限制在 0–60000 ms；点击 holdMs 仍仅来自 Down/Up 配对时间。
- 新增 `RecorderTimingTests.cs` 三项回归测试，覆盖固定间隔、必要等待超额及开关关闭时的兼容行为。
- 将测试项目目标框架调整为 `net10.0-windows` 并引用 App 项目，以直接验证 Lua 生成器。

### Git / 验证
- 实现提交：`bf4987e6426e4ee9b15fd1eb9ca4dc474f709456`
- Actions：<https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44732976826>，本记录追加时为 QUEUED/PENDING，最终结果待更新。
- Windows 真机布局、DPI、真实 Hook、悬浮菜单和长时间录制：NOT RUN。


### 1.1.0 发布后验证补记（2026-11-20）

- 正式 Release：[v1.1.0](https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.1.0)，`prerelease=false`。
- 发布提交：`84bd6b80746c43e1022d635eeccac535f6f1de50`。
- Release Actions：[44733253454](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/44733253454)，SUCCESS。
- Restore、Build、Test、自包含发布、打包、SHA256、Artifact 上传和 Release 创建全部 PASS；Release Build 0 warnings / 0 errors。
- 测试：10 passed / 0 failed / 0 skipped；新增录制时序回归测试 3 项。
- ZIP：`KeyFlowStudioLite-1.1.0-win-x64.zip`，51,915,111 bytes。
- ZIP SHA256：`b8af447c184807ce59516e1a90e3dd0cf7ebbcc31414185a530b4c62b06fd207`；sidecar 随 Release 一同上传。
- 用户 Windows 实机视觉与目标软件验收仍为 NOT RUN；CI 通过不能替代 DPI、拖动、真实 Hook、菜单悬停和长时稳定性验收。


## 2026-11-20 — 1.1.1 — 悬浮控制窗第二轮视觉与布局重整（发布候选）

### 用户反馈与设计判断
- 根据用户提供的 Windows 实机截图，原悬浮窗将标题、5 个动作按钮、循环/轮间隔、实时坐标和 4 个点击节奏控件挤在约 520×140 px 的空间中；问题不仅是配色，也包括层级、控件间距和信息分组。
- 用户认可完整主界面，本次只调整独立悬浮窗，不擅自重做完整主界面，也不把先前的命名管道异常混入本次变更。

### UI 调整
- 悬浮窗调整为 600×190 px，外围边框采用低对比度细线，统一浅色表面和对齐规则。
- 标题区分别呈现产品名、脚本名、状态和窗口操作；状态根据录制、播放、暂停、就绪使用不同的低饱和提示色。
- 操作区突出播放主按钮，录制使用轻红色，其他操作使用中性按钮；全局快捷键提示独立对齐。
- 循环次数、轮间隔、只录点击和实时坐标集中在同一参数行；点击节奏控件拆为独立一行，避免与循环参数挤在一起。
- 增加底部说明，标注固定间隔仅在“只录点击”模式下生效；未增加大型卡片、深色主题或额外功能。

### 验证状态
- 当前记录为发布候选；Windows Actions、ZIP/SHA 和 Release 资产结果在正式发布后补记。
- Windows 实机 100/125/150% DPI、多显示器、拖动、按钮截断和持续运行：NOT RUN，不能由源码改动或 CI 通过代替。
