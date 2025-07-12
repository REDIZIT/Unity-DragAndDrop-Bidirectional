# Bidirectional Files Drag and Drop
This plugin allows you to make your Game/application player able to **BI-DIRECTIONAL DRAG AND DROP**!<br/>
- Invoke **DoDragDrop** and send files from your built game/app into Windows (File explorer, browser, telegram and etc).</br>
- Subcribe for file Drop from outside (from Windows to Game/application)

Theoretically, this code can be adapted not only for Game/application, but also for any C# application and even more, since WinAPI calls are used here, it can be rewritten in Rust. The only thing you must have - ability to P/Invoke.

# Features
- Receive drop outside the Game/application
- Send drop outside from Game/application
- Send drop and receive drop inside same Game/application instance (UI thread will be locked, but onFilesDrop event will be invoked)

# How to setup
- For sending invoke: EditorDragAndDropHook.StartDragFiles
- For receiving use EditorDragAndDropHook.onFilesDropped and EditorDragAndDropHook.SubcribeOnDropHover

# Known Issues
- Thread block when WinAPI.DoDragDrop (blocks UI). Invoking WinAPI.DoDragDrop from another thread causes OleInitialize error [0x80010106 'RPC_E_CHANGED_MODE'](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/705fb797-2175-4a90-b5a3-3918024b10b8)
- Build Crash with ERROR: SymGetSymFromAddr64, GetLastError: 'Attempt to access invalid address.' (Address: 00007FF614D811F2). **Fix:** Player Settings -> Other Settings -> Managed Stripping Level set to 'Disabled'

# Sources used
- [UnityWindowsFileDrag-Drop](https://github.com/Bunny83/UnityWindowsFileDrag-Drop) - Editor only drop receive-part code
- [How to Implement Drag and Drop Between Your Program and Explorer](https://www.codeproject.com/Articles/840/How-to-Implement-Drag-and-Drop-Between-Your-Progra) - Chapter 'Initiating a drag and drop' is pretty usefull for DoDragDrop
- Gemini, MSDN - While WinAPI Vibe Coding is still too much for AI and reading MSDN is still too much for Me, combo of these two will produce something, that seems to work

# Links
- GitHub: https://github.com/REDIZIT/Unity-DragAndDrop-Bidirectional