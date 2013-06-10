using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace GuildWars2.Spidy.Prospects
{
    public class HotKeyHandler
    {
        private IList<Keys> m_KeyHook_KEYS = new List<Keys> { Keys.F5 };
        private IList<Keys> m_KeyHook_KEYS_SKIP = new List<Keys> { Keys.F6 };
        private IList<int> m_KeyHook_IDS = new List<int>();
        private HandlerState m_CurrentState = HandlerState.SEARCH;
        private SpinLock m_SpinLock = new SpinLock();

        public int CopperValue { get; set; }

        private ItemInfo m_CurrentItem = null;
        public ItemInfo CurrentItem
        {
            get { return m_CurrentItem; }
            set
            {
                bool lockTaken = false;
                try
                {
                    m_SpinLock.Enter(ref lockTaken);

                    m_CurrentItem = value;
                    m_CurrentState = HandlerState.SEARCH;
                }
                finally
                {
                    if (lockTaken)
                        m_SpinLock.Exit();
                }
            }
        }

        public HotKeyHandler()
        {
            CurrentItem = null;
            CopperValue = -1;
        }

        public void RegisterHotKeys()
        {
            if (m_KeyHook_IDS.Count == 0)
            {
                HotKeyManager.HotKeyPressed += HotKeyPressed;

                foreach (Keys key in m_KeyHook_KEYS)
                    m_KeyHook_IDS.Add(HotKeyManager.RegisterHotKey(key, HotKeyManager.KeyModifiers.NoRepeat));

                foreach (Keys key in m_KeyHook_KEYS_SKIP)
                    m_KeyHook_IDS.Add(HotKeyManager.RegisterHotKey(key, HotKeyManager.KeyModifiers.NoRepeat));
            }
        }

        public void UnregisterHotKeys()
        {
            if (m_KeyHook_IDS.Count > 0)
            {
                foreach (int id in m_KeyHook_IDS)
                    HotKeyManager.UnregisterHotKey(id);

                HotKeyManager.HotKeyPressed -= HotKeyPressed;
                m_KeyHook_IDS.Clear();
            }
        }

        private void HotKeyPressed(object sender, HotKeyManager.HotKeyEventArgs e)
        {
            bool lockTaken = false;
            try
            {
                m_SpinLock.Enter(ref lockTaken);

                if (e.Modifiers == 0 && m_CurrentItem != null)
                {
                    SpinWait.SpinUntil(() => !HotKeyManager.IsKeyPressed(e.Key));

                    if (m_KeyHook_KEYS.Contains(e.Key))
                    {
                        long buyPrice = m_CurrentItem.BuyPrice + 1;

                        if (CopperValue >= 0)
                        {
                            buyPrice = (long)((Math.Floor(m_CurrentItem.BuyPrice / 100.0) * 100) + CopperValue);

                            if (buyPrice <= m_CurrentItem.BuyPrice + 1)
                                buyPrice += 100;
                        }

                        long buyPriceC = (buyPrice % 100);
                        long buyPriceS = (long)(Math.Floor(buyPrice / 100.0) % 100);
                        long buyPriceG = (long)(Math.Floor(buyPrice / 10000.0));

                        switch (m_CurrentState)
                        {
                            case HandlerState.SEARCH:
                                SendKeys.Send(m_CurrentItem.Name);
                                m_CurrentState = HandlerState.ENTERG;
                                break;
                            case HandlerState.ENTERG:
                                SendKeys.Send(buyPriceG.ToString());
                                m_CurrentState = HandlerState.ENTERS;
                                break;
                            case HandlerState.ENTERS:
                                SendKeys.Send(buyPriceS.ToString());
                                m_CurrentState = HandlerState.ENTERC;
                                break;
                            case HandlerState.ENTERC:
                                SendKeys.Send(buyPriceC.ToString());
                                m_CurrentState = HandlerState.NEXT;
                                break;
                            case HandlerState.NEXT:
                                m_CurrentItem = null;
                                m_CurrentState = HandlerState.SEARCH;
                                break;
                        }
                    }
                    else if (m_KeyHook_KEYS_SKIP.Contains(e.Key))
                    {
                        m_CurrentItem = null;
                        m_CurrentState = HandlerState.SEARCH;
                    }
                }
            }
            finally
            {
                if (lockTaken)
                    m_SpinLock.Exit();
            }
        }

        private enum HandlerState
        {
            SEARCH,
            ENTERG,
            ENTERS,
            ENTERC,
            NEXT
        }
    }
}
