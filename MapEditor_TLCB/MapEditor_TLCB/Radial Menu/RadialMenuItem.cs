using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MapEditor_TLCB
{
    public class EventData
    {
        public delegate void EventCallback(Object p_data);
        public EventCallback callback;
        public Object data;
        public Keys hotkey;

        public EventData(EventCallback p_callback, Object p_data)
        {
            callback = p_callback;
            data = p_data;
            hotkey = Keys.None;
        }
    }

    public class RadialMenuItem
    {
        public delegate void RadialMenuItemCallback(Object p_data);

        public string text;
        public Texture2D texture;

        public EventData activateEvent;

        public RadialMenu submenu;

        public Rectangle sourceRect;

        public float scale;

        public RadialMenuItem(string p_text, Texture2D p_texture, EventData p_event, float p_scale = 1.0f)
        {
            text = p_text;
            texture = p_texture;
            activateEvent = p_event;

            sourceRect.X = 0;
            sourceRect.Y = 0;
            sourceRect.Width = p_texture.Width;
            sourceRect.Height = p_texture.Height;
            scale = p_scale;
        }
        public RadialMenuItem(string p_text, Texture2D p_texture, EventData p_event, Rectangle p_sourceRect, float p_scale = 1.0f)
        {
            text = p_text;
            texture = p_texture;
            activateEvent = p_event;
            sourceRect = p_sourceRect;
            scale = p_scale;
        }
        public RadialMenuItem(string p_text, Texture2D p_texture, RadialMenu p_submenu, float p_scale = 1.0f)
        {
            text = p_text;
            texture = p_texture;
            submenu = p_submenu;
            activateEvent = null;

            sourceRect.X = 0;
            sourceRect.Y = 0;
            sourceRect.Width = p_texture.Width;
            sourceRect.Height = p_texture.Height;
            scale = p_scale;
        }
        public RadialMenuItem(string p_text, Texture2D p_texture, RadialMenu p_submenu, Rectangle p_sourceRect, float p_scale = 1.0f)
        {
            text = p_text;
            texture = p_texture;
            submenu = p_submenu;
            activateEvent = null;

            sourceRect = p_sourceRect;
            scale = p_scale;
        }
    }
}
