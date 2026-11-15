# 当前会话交接

## 项目身份
- 产品：**KeyFlow Studio Lite**
- 仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 当前目标版本：`1.0.1`
- 平台：Windows 10/11 x64
- 技术栈：WinForms、.NET 10、Lua 5.4、NLua/KeraLua
- 当前解决方案：`KeyFlowStudioLite.sln`
- App/Core/Tests：`KeyFlow.App`、`KeyFlow.Core`、`KeyFlow.Core.Tests`
- 应用程序集与可执行文件：`KeyFlowStudioLite.exe`

新会话只需读取仓库地址，再按根目录 `AGENTS.md` 指引读取本文件、路线图和相关技术文档，不要求用户重新粘贴提示词。

## 1.0.1 变更目标
1. 清除源码、命名空间、项目目录、解决方案、程序集、Worker IPC 命名中的旧内部开发代号，统一为 KeyFlow。
2. 悬浮窗从 640 px 收窄到 500 px；重新压缩头部、操作按钮、循环设置和坐标显示，不改变功能。
3. 将用户数据目录迁到 `%LOCALAPPDATA%\\KeyFlowStudioLite`。首次启动非破坏性导入旧目录中缺失的文件；不覆盖新目录已有文件，不删除旧目录。
4. 统一 `VERSION`、App ProductVersion、AssemblyVersion、FileVersion 与发布资产命名为 1.0.1。
5. 更新 README、协作规则、验收记录和累计开发日志。

## 兼容与安全
- Lua API、键鼠录制和播放语义维持 1.0.0。
- 首次启动的数据导入按文件复制、跳过已有目标文件，不执行覆盖或删除。
- 旧版目录只在数据迁移桥接代码中作为来源识别，不作为当前产品名、程序集名或新数据目录。
- Windows Actions 通过不代表悬浮窗、Hook、DPI、多屏和托盘已经在用户机器上完成实机验收。

## 当前提交与 CI
- 1.0.0 基线 HEAD：`b520fb3f99c5d524386b725b3a396589f55ec58d`
- 1.0.0 正式 Release：<https://github.com/xiaobei206/KeyFlowStudioLite/releases/tag/v1.0.0>
- 1.0.1 实现提交：待本轮提交后填入。
- 1.0.1 Windows Actions：待触发并核验。
- 1.0.1 Release 资产、ZIP SHA256：待 Windows Actions 通过后填入。
- 用户 Windows 实机验收：`NOT RUN`。

## 下一会话唯一任务
先核对最新 `main` HEAD、`v1.0.1` Release 和 Windows Actions。若发布验证通过，继续做 M1 稳定性补强：审查录制暂停/恢复、停止与输入释放、Worker 崩溃/IPC 断开、托盘生命周期和设置持久化，补充可确定性复现的自动化回归测试。不要擅自扩成完整 IDE、OCR、插件或云服务。
