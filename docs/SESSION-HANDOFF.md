# 当前会话交接

## 项目身份
- 产品：**KeyFlow Studio Lite**
- 仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 当前稳定版本：`1.0.1`
- 平台：Windows 10/11 x64
- 技术栈：WinForms、.NET 10、Lua 5.4、NLua/KeraLua
- 解决方案：`KeyFlowStudioLite.sln`
- 项目：`KeyFlow.App`、`KeyFlow.Core`、`KeyFlow.Core.Tests`
- 应用程序集与 EXE：`KeyFlowStudioLite.exe`

新会话只需读取仓库地址，再按根目录 `AGENTS.md` 指引读取本文件、路线图和相关技术文档；不要要求用户重复粘贴提示词。

## 1.0.1 已完成
1. 清理当前源码树中的旧项目目录、解决方案、项目名、命名空间、程序集、EXE、资源文件名、manifest 标识和 Worker IPC 前缀；统一为 KeyFlow。
2. 悬浮窗客户区固定为 **500 × 124 px**（原 640 × 124 px）；压缩标题区、按钮和循环设置区，保留完整功能与实时 X/Y 坐标。
3. 用户数据改用 `%LOCALAPPDATA%\\KeyFlowStudioLite`。首次启动把旧目录中目标目录尚不存在的文件复制到新目录；不覆盖新文件、不删除旧目录，保留回滚能力。
4. `VERSION`、App ProductVersion、AssemblyVersion、FileVersion 与资产版本统一为 `1.0.1`。
5. 更新 README、AGENTS、用户手册、路线图、功能矩阵、验收、决策和累计开发日志。

## 兼容边界
- Lua API、键鼠录制和播放语义保持 1.0.0 行为。
- 源码仅保留一个旧用户数据目录标识，专门用于首次启动的兼容导入；不是当前产品名/项目名/程序集名。
- 数据迁移逐文件复制并跳过已存在目标文件；不执行覆盖或删除。
- CI 成功不等于用户机器已完成悬浮窗、真实 Hook、DPI/多屏、托盘和长时稳定性验收。

## 1.0.1 构建与发布证据
- 代码发布提交：`e21080bc917e5a287b53a30e3d96a8707437b3c3`
- Windows Actions：[43729901294](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43729901294)，结论 SUCCESS。
- Restore、Release Build、Core tests、自包含 win-x64 publish、ZIP/SHA256、Artifact upload、GitHub Release：全部 PASS。
- Release Build：0 warnings / 0 errors。
- Core tests：7/7 PASS，0 failed，0 skipped。
- 正式稳定 Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.1>（prerelease=false）。
- ZIP：`KeyFlowStudioLite-1.0.1-win-x64.zip`，51,911,747 bytes。
- ZIP SHA256：`19becd2a22f896126e6dfad39ef38ee6e7a755d006a6ea3e5911292efd12af42`。
- SHA sidecar：`KeyFlowStudioLite-1.0.1-win-x64.zip.sha256`，已随 Release 上传。
- 实机验收：悬浮窗 500 px 布局、数据迁移、真实 Hook、DPI/多屏、托盘和长时稳定性均为 `NOT RUN`。

## 下一会话唯一任务
核对最新 `main` HEAD、`v1.0.1` Release 和 Actions 后，继续 M1 稳定性补强：审查录制暂停/恢复、停止与输入释放、Worker 崩溃/IPC 断开、托盘生命周期和设置持久化，补充可确定性复现的自动化回归测试。不要擅自扩成完整 IDE、OCR、插件或云服务。
