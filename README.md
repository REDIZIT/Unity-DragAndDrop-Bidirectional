https://github.com/user-attachments/assets/3f481c83-2691-4357-bd07-ed0146e4045c

# Bidirectional Files Drag and Drop
This plugin allows you to make your Unity (but not only) player able to **BI-DIRECTIONAL DRAG AND DROP**!<br/>
- Invoke **DoDragDrop** and send files from your Unity built game/app into Windows (File explorer, browser, telegram and etc).</br>
- Subcribe for file Drop from outside (from Windows to Unity)

Theoretically, this code can be adapted not only for Unity, but also for any C# application and even more, since WinAPI calls are used here, it can be rewritten in Rust. The only thing you must have - ability to P/Invoke.

# Features
- Receive drop outside the Unity application
- Send drop outside from Unity application
- Send drop and receive drop inside same Untiy application (UI thread will be locked, but onFilesDrop event will be invoked)

# Known Issues
- Thread block when WinAPI.DoDragDrop (blocks UI). Invoking WinAPI.DoDragDrop from another thread causes OleInitialize error [0x80010106 'RPC_E_CHANGED_MODE'](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/705fb797-2175-4a90-b5a3-3918024b10b8)

# Sources used
- [UnityWindowsFileDrag-Drop](https://github.com/Bunny83/UnityWindowsFileDrag-Drop) - Unity only drop receive-part code
- [How to Implement Drag and Drop Between Your Program and Explorer](https://www.codeproject.com/Articles/840/How-to-Implement-Drag-and-Drop-Between-Your-Progra) - Chapter 'Initiating a drag and drop' is pretty usefull for DoDragDrop
- [ClipSpy](https://www.codeproject.com/Articles/168/ClipSpy) - Program from 2001 year, that displays content of clipboard and drag-n-drop
- Gemini, MSDN - While WinAPI Vibe Coding is still too much for AI and reading MSDN is still too much for Me, combo of these two will produce something, that seems to work
