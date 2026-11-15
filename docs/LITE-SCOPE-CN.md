# KeyFlow Studio Lite 1.0.1 范围

- 产品：KeyFlow Studio Lite
- 版本：1.0.1
- 平台：Windows 10/11 x64
- 技术栈：WinForms + .NET 10 + Lua 5.4
- 源码迁移基线：原仓库 `xiaobei206/anjian` tag `v1.0.0`，commit `9e2f8d8528f34468dd0423262da6ebe56d572de6`
- 当前仓库：`xiaobei206/KeyFlowStudioLite`
- 正式发布类型：GitHub stable Release（非 prerelease）

## 冻结原则

当前仓库只承接 Lite 1.0.0 已交付功能。源码迁移时允许调整品牌显示、产品包名、文档、CI/Release 和不影响用户数据的整理；不得把原完整开发线的高级 IDE、图像资源、OCR、参数设计器、小精灵、插件和多实例代码带入 Lite。

## 包含

- 主窗口、独立悬浮窗、系统托盘。
- 本地 Lua 文件 CRUD 与基础文本编辑。
- 全局鼠标/键盘录制、暂停/续录、click-only、移动降噪。
- Lua Worker、Named Pipe IPC、播放/暂停/停止。
- 鼠标、键盘、滚轮、Unicode 文本和 Delay API。
- 循环次数、轮间隔、F10/F11/F12 快捷键。
- 设置持久化和停止输入释放。

## 不包含

- 图像资源、找图/找色、OCR。
- 高级 IDE、语法补全、断点调试、动作列表。
- 脚本参数设计器、复杂自定义 UI、小精灵打包。
- 插件、受控文件/网络 API、定时任务、多实例管理。
- 云同步、账号、遥测和远程执行。

## 兼容性

- 继续使用 `KeyFlowStudioLite.exe` 和历史目录 `%LOCALAPPDATA%\\KeyFlowStudioLite`。
- 不静默改变 Lua 已发布 API 的函数名、默认值、时间单位或坐标语义。
- 任何破坏性变更必须有版本提升、迁移说明和回滚方案。

## 验证边界

Windows Actions 只证明构建、单测、自包含发布和包校验。真实桌面 Hook、托盘、DPI/多屏、目标进程权限和长时运行必须在 Windows 实机逐项验收。


## 1.0.1 内部身份与布局修正

1.0.1 不扩展产品功能范围；仅统一内部工程命名为 KeyFlow、迁移应用程序身份，并将悬浮控制窗宽度收窄至 500 px。Lua API、键鼠录制/播放语义保持兼容。用户数据首次启动时从旧版目录非破坏性导入，旧目录保留。
