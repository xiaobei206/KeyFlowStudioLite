# 自动化测试

当前测试项目：`tests/Anjian.Core.Tests`（xUnit，net10.0）。

现有测试：
- Lua runtime 版本及注册函数调用。
- Lua 指令计数 Hook。
- 播放默认值、轮次数、无限循环和参数边界。

执行：

```powershell
dotnet test tests/Anjian.Core.Tests/Anjian.Core.Tests.csproj -c Release
```

后续优先增加录制事件配对、录制暂停/续录、Worker 异常/取消、输入释放和设置持久化测试。Windows Hook、SendInput、托盘与混合 DPI 行为仍需 Windows 实机集成/手工测试；不得用普通单测冒充实机验收。
