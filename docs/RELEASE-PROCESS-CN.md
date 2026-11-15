# 发布流程与版本规约

## 1. 版本源

以下内容必须一致：
- `VERSION`
- `PRODUCT_NAME`
- `PRODUCT_ID`
- `src/KeyFlow.App/KeyFlow.App.csproj` 的 ProductVersion

## 2. 本地验证

```powershell
dotnet restore KeyFlowStudioLite.sln
dotnet build KeyFlowStudioLite.sln -c Release --no-restore
dotnet test tests/KeyFlow.Core.Tests/KeyFlow.Core.Tests.csproj -c Release
dotnet publish src/KeyFlow.App/KeyFlow.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o artifacts/publish/win-x64
```

## 3. GitHub Actions

`.github/workflows/windows-build.yml` 在 main push / PR 时执行 Restore、Build、Core tests、self-contained win-x64 publish、ZIP 和 SHA256、artifact 上传。

提交信息以 `release:` 开头时，在完整构建通过后创建 GitHub Release：
- Tag：`vX.Y.Z`
- ZIP：`KeyFlowStudioLite-X.Y.Z-win-x64.zip`
- Sidecar：`KeyFlowStudioLite-X.Y.Z-win-x64.zip.sha256`

## 4. 发布门禁

- 任何 Build error 或测试失败：停止发布。
- ProductVersion 与 VERSION 不匹配：停止发布。
- ZIP 或 sidecar 不存在：停止发布。
- 发布后核验 tag、release 是否 prerelease、资产名称、大小、SHA256。
- 稳定 Release 禁止覆盖已有版本；修复必须提升 patch 或 minor 版本。
- 每次发布更新 README、SESSION-HANDOFF、DEVELOPMENT-LOG 和 ACCEPTANCE。

## 5. 用户数据迁移

1.0.1 将用户数据存放于 `%LOCALAPPDATA%\\KeyFlowStudioLite`。首次启动会从旧版目录复制目标目录中缺失的文件，不覆盖新数据、不删除源目录。回滚到旧版仍可使用原目录；新版本新增的数据不会自动反向同步。

## 6. 回滚

保留 Git tag 和上一版 Release。发现问题时先撤销代码提交或发布新 patch 版本；不要删除用户本地脚本/设置目录，不要在升级时清理 `%LOCALAPPDATA%\\KeyFlowStudioLite`。
