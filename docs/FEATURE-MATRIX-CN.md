# 功能矩阵（KeyFlow Studio Lite）

状态定义：`IMPLEMENTED` = 源码已实现；`AUTOMATED` = 有自动化覆盖；`MANUAL NOT RUN` = 需要实机但未提供证据；`OUT OF SCOPE` = 不属于 Lite。

| 模块 | 功能 | 1.0.0 状态 | 说明 |
|---|---|---|---|
| 脚本 | 新建/打开/保存/重命名/复制/删除 | IMPLEMENTED | 本地 .lua 文件 |
| 编辑 | Lua 文本编辑、Ctrl+S | IMPLEMENTED | 当前非高级 IDE |
| Lua | Lua 5.4 runtime | AUTOMATED | 通过 NLua/KeraLua |
| Worker | 独立进程执行、Named Pipe IPC | IMPLEMENTED | Worker 与主进程分离 |
| 鼠标 | MoveTo、左/右/中键、Down/Up、Wheel | IMPLEMENTED | Windows SendInput |
| 键盘 | KeyPress、Down/Up、Hotkey | IMPLEMENTED | 虚拟键映射 |
| 文本 | TypeText Unicode | IMPLEMENTED | Unicode UTF-16 输入事件 |
| 时间 | Delay、RandomDelay、按下保持时长 | IMPLEMENTED | 毫秒 |
| 录制 | 全局鼠标/键盘 Hook | IMPLEMENTED | 需要 Windows 权限/环境验证 |
| 录制 | 暂停/续录、click-only、移动降噪 | IMPLEMENTED | 输出 Lua |
| 播放 | 播放/暂停/停止 | IMPLEMENTED | F10/F11/F12 |
| 循环 | 次数与轮间隔，0=无限 | AUTOMATED | Core 测试覆盖 |
| UI | 主窗、独立悬浮窗、托盘 | IMPLEMENTED | MANUAL NOT RUN |
| UI | 悬浮窗置顶、坐标显示 | IMPLEMENTED | MANUAL NOT RUN |
| 配置 | settings.json 持久化 | IMPLEMENTED | 损坏时回退默认值 |
| 输入安全 | Stop/异常路径释放输入 | IMPLEMENTED | 需补足自动化回归 |
| DPI/多屏 | 混合 DPI、负坐标、多显示器 | MANUAL NOT RUN | 不得标记实机 PASS |
| 稳定性 | 长时间录制/播放/重复启停 | MANUAL NOT RUN | 下一阶段优先验收 |

## 明确不包含

图像资源/找图/找色、OCR、高级 IDE/断点、动作列表、参数设计器、自定义 UI、小精灵、插件、定时任务、多实例和云服务均为 `OUT OF SCOPE`。


## 1.0.1 补充
- [x] KeyFlow 解决方案、项目、命名空间、程序集和 EXE 身份统一（CI PASS）。
- [x] 旧版用户数据首次启动非破坏性导入（代码已实现；实机数据迁移 NOT RUN）。
- [x] 悬浮窗固定宽度收窄至 500 px，压缩设置与按钮布局（编译通过；实机布局 NOT RUN）。
- [ ] Windows 实机确认不同 DPI、长脚本名、状态文本和坐标边界均不裁切。


## 1.1.0 — 固定点击节奏与悬浮窗

| 功能 | 状态 | 验收说明 |
|---|---|---|
| 固定连续点击总间隔 | IMPLEMENTED / CI PENDING | 默认关闭；在只录点击模式下按配置的总间隔规范化连续点击 |
| 点击后等待 | IMPLEMENTED / CI PENDING | 上一次点击结束后、下次移动前精确等待用户设置的毫秒数 |
| 移动后悬停等待 | IMPLEMENTED / CI PENDING | 移动到点击坐标后至少等待配置值；该等待计入固定点击总间隔 |
| 超出总间隔的必要等待 | IMPLEMENTED / CI PENDING | 不缩短点击后等待或悬停最低等待，允许实际间隔超过目标值 |
| 关闭功能后的兼容性 | TEST ADDED / CI PENDING | 应保留原录制间隔及原有生成顺序 |
| 浅色紧凑悬浮窗 | IMPLEMENTED / REAL-WORLD TEST PENDING | 目标尺寸 520 × 140；浅灰白底、无深色主题、大卡片或额外面板 |
| 实机 DPI、拖动和菜单时序 | NOT RUN | 需要在 Windows 10/11 实机进行视觉与目标软件验收 |
