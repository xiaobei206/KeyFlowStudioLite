# 1.0.0 源码迁移记录

## 来源

- 源仓库：<https://github.com/xiaobei206/anjian>
- 源 tag：`v1.0.0`
- 源 commit：`9e2f8d8528f34468dd0423262da6ebe56d572de6`
- 源 Release：旧仓库 Mouscript Lite v1.0.0
- 源 Windows Actions run：[43726550455](https://github.com/xiaobei206/anjian/actions/runs/43726550455)

## 目标

- 目标仓库：<https://github.com/xiaobei206/KeyFlowStudioLite>
- 默认分支：`main`
- 产品：KeyFlow Studio Lite
- 版本：1.0.0

## 迁移规则

- 源码以旧稳定 tag 为准，不从后来完整开发版 main 选取文件。
- 完整迁移解决方案、App/Core 项目、资源图标、Lua Worker、录制/播放服务、测试和文档。
- 只更换产品显示名称、PRODUCT_NAME/PRODUCT_ID、README、AI 接手文档和 CI/Release 元数据。
- 不重命名内部解决方案、程序集、EXE、历史用户数据目录，避免影响现有用户。
- 本仓库的 Actions/Release 必须重新执行和创建；旧仓库 CI 与 Release 只作为迁移来源证据。

## 来源包校验

旧仓库来源 ZIP：`MouscriptLite-1.0.0-win-x64.zip`，51,911,007 bytes，SHA256：

`ffab0dd7382a927949f405b5daf39b35a490c03539e17b507f4f5cbb286c0eb2`

该 SHA 是原仓库来源包摘要，不是本仓库新 Release 的 SHA。新仓库 Release 的 SHA 以本仓库 Actions 生成的 sidecar 和 GitHub asset digest 为准。

## 文件清单

本仓库迁移包括：
- 根目录解决方案、版本/产品元数据、AGENTS 与 Windows workflow。
- `src/Anjian.App`、`src/Anjian.Core` 全部 1.0.0 源码与 ICO。
- `tests/Anjian.Core.Tests` 测试项目。
- 原有架构、PRD、API、录制器、决策、用户指南文档，以及新的交接、功能矩阵、路线图、发布流程和迁移说明。
