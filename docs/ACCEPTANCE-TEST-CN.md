# KeyFlow Studio Lite 验收清单

## 1. 状态规则

每项只允许：`PASS`、`FAIL`、`BLOCKED`、`NOT RUN`。必须附运行环境和证据。CI PASS 不能代替真实 Windows 桌面测试。

## 2. 环境记录

- Windows edition / version / build：
- CPU 架构：
- 主屏分辨率、缩放比例：
- 副屏数量、分辨率、缩放比例、相对位置：
- .NET SDK / Runtime：
- 应用版本与 commit：
- 测试日期：

## 3. 迁移基线自动化结果

迁移源版本的旧仓库 Windows Actions run [43726550455](https://github.com/xiaobei206/anjian/actions/runs/43726550455)：
- Restore / Release Build：PASS，0 warnings / 0 errors
- Core tests：PASS，7/7
- Self-contained win-x64 publish：PASS
- ZIP / SHA256：PASS
- Release creation：PASS

该结果只证明旧源仓库 tag 的构建。新仓库自身 run 必须在迁移流程完成后补录，不能把旧仓库 run 当作新仓库 CI。

## 4. 自动化测试项目

- [ ] Lua runtime 确认为 Lua 5.4。
- [ ] Lua 可调用注册的全局函数。
- [ ] Lua 指令计数 Hook 可触发。
- [ ] 默认播放一次、轮间隔 2000 ms。
- [ ] 多轮播放只在轮次之间等待。
- [ ] RepeatCount=0 为无限循环。
- [ ] 超出次数/间隔边界被拒绝。
- [ ] 新增录制、Worker、输入释放测试后全部通过。

## 5. Windows 实机验收

- [ ] 启动默认悬浮窗与托盘。
- [ ] 打开主窗口时悬浮窗仍正常；主窗口关闭只隐藏，不退出。
- [ ] 托盘重新打开主窗口、显示/隐藏悬浮窗和明确退出。
- [ ] 新建、保存、重命名、复制、删除脚本；重启后仍存在。
- [ ] 普通文本编辑与 Ctrl+S。
- [ ] 录制鼠标移动和点击，检查坐标和操作顺序。
- [ ] click-only 模式不会写入鼠标移动轨迹。
- [ ] 键盘录制、组合键、滚轮。
- [ ] 暂停录制时不采集；续录后继续；结束后正确生成 Lua。
- [ ] F10 播放、F11 暂停/继续、F12 中止。
- [ ] Stop 后所有鼠标/键盘输入释放。
- [ ] Worker 异常退出或强制终止时输入清理。
- [ ] 0 次无限循环可以紧急停止。
- [ ] 多屏负坐标、混合 DPI、不同缩放比例。
- [ ] 非管理员/管理员目标进程权限差异提示。
- [ ] 至少 2 小时持续录制/播放/多次启停。

## 6. 当前未执行项目

在真实 Windows 10/11 用户设备提交证据之前，本清单第 5 节全部默认为 `NOT RUN`。禁止因代码阅读、CI 构建成功或在模拟器中创建窗口而标记实机 PASS。


## 7. 新仓库迁移 CI 验证

- 仓库：`xiaobei206/KeyFlowStudioLite`
- 提交：`7adc5bbe0553e02f89de74a72f1b7a7294a034a5`
- Windows Actions：[43727695106](https://github.com/xiaobei206/KeyFlowStudioLite/actions/runs/43727695106) — SUCCESS
- Restore：PASS
- Build：PASS，0 warnings / 0 errors
- Core tests：7/7 PASS
- Self-contained win-x64 publish：PASS
- ZIP / SHA256：PASS
- Actions artifact upload：PASS，artifact ID `14167530220`
- 此为自动化验证；第 5 节实机项目仍保持 NOT RUN。
