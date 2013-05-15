using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.Components;
using MapEditor_TLCB.CustomControls;
using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB.Systems
{
    public class EventSystem : EntitySystem
    {
        List<EventData> m_events;
        public EventSystem()
		{
            m_events = new List<EventData>();
		}

		public override void Initialize()
		{
		}

		public override void Process()
		{
            for (int i = 0; i < m_events.Count; i++)
            {
                if (KeyDelta.getDelta(m_events[i].hotkey) > 0.0f && !Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    if (m_events[i].callback != null)
                        m_events[i].callback(m_events[i].data);
                }
            }
		}

        public void addEvent(EventData p_event)
        {
            p_event.system = this;
            for (int i = 0; i < m_events.Count; i++)
            {
                if (m_events[i].hotkey == p_event.hotkey)
                    m_events[i].hotkey = Keys.None;
            }
            m_events.Add(p_event);
        }
        public void setHotKey(EventData p_event, Keys p_hotkey)
        {
            for (int i = 0; i < m_events.Count; i++)
            {
                if (m_events[i].hotkey == p_hotkey)
                    m_events[i].hotkey = Keys.None;
            }
            p_event.hotkey = p_hotkey;
        }
    }
}
