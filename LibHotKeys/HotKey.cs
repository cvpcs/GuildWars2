using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace LibHotKeys
{
    public class HotKey : IEquatable<HotKey>
    {
        public Key Key { get; private set; }
        public ModifierKeys Modifiers { get; private set; }

        public Keys Keys => (Keys)KeyInterop.VirtualKeyFromKey(Key);

        public HotKey(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public HotKey(Keys key, ModifierKeys modifiers = ModifierKeys.None)
            : this(KeyInterop.KeyFromVirtualKey((int)key), modifiers)
        { }

        public HotKey(uint hotKeyParam)
            : this((Keys)((hotKeyParam & 0xffff0000) >> 16), (ModifierKeys)(hotKeyParam & 0x0000ffff))
        { }

        public HotKey(IntPtr hotKeyParam)
            : this((uint)hotKeyParam.ToInt64())
        { }

        public bool Equals(HotKey other)
            => Key == other?.Key &&
               Modifiers == other?.Modifiers;

        public override bool Equals(object obj)
            => !ReferenceEquals(null, obj) &&
               (ReferenceEquals(this, obj) || Equals(obj as HotKey));

        public override int GetHashCode()
            => new Tuple<Key, ModifierKeys>(Key, Modifiers).GetHashCode();

        public override string ToString()
            => new[] { Modifiers.ToString(), Key.ToString() }
                .SelectMany(k => k.Split(','))
                .Select(k => k.Trim())
                .Aggregate((a, b) => $"{a} + {b}");
    }
}
