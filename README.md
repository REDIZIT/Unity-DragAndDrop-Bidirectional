# Description
This plugin allows you to make your Unity (but not only) player able to **BI-DIRECTIONAL DRAG AND DROP**!<br/>
- Invoke **DoDragDrop** and send files from your Unity built game/app into Windows (File explorer, browser, telegram and etc).</br>
- Subcribe for file Drop from outside (from Windows to Unity)

Theoretically, this code can be adapted not only for Unity, but also for any C# application and even more, since WinAPI calls are used here, it can be rewritten in Rust. The only thing you must have - ability to P/Invoke.

# Sources used
- [UnityWindowsFileDrag-Drop](https://github.com/Bunny83/UnityWindowsFileDrag-Drop) - Unity only drop receive-part code
- [How to Implement Drag and Drop Between Your Program and Explorer](https://www.codeproject.com/Articles/840/How-to-Implement-Drag-and-Drop-Between-Your-Progra) - Chapter 'Initiating a drag and drop' is pretty usefull for DoDragDrop
- [ClipSpy](https://www.codeproject.com/Articles/168/ClipSpy) - Program from 2001 year, that displays content of clipboard and drag-n-drop
