using System;

namespace Wireform.Utils
{
    /// <summary>
    /// Keyboard modifiers such as Shift, Ctrl, and Alt
    /// </summary>
    [Flags]
    public enum Modifier
    {
        None = 0,
        Shift = 1,
        Control = 2,
        Alt = 4,
    }
}
