using System;

public delegate IntPtr HookProc(int code, IntPtr wParam, ref MSG lParam);
public delegate bool EnumThreadDelegate(IntPtr Hwnd, IntPtr lParam);