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
    public class RadialMenuContext
    {
        List<RadialMenu> m_menus;
        int m_current;
        int m_root;

        bool m_altDown = false;
        bool m_active = false;
        bool m_enterDown = false;

        private Texture2D m_circleTexture;
        private Texture2D m_infoBox;
        private Texture2D m_arrow;
        private float size;
        private SpriteFont m_font;
        private int prevMouseWheel;

        Vector2 m_position = new Vector2(640, 360);
        Vector2 m_originalPosition = new Vector2(640, 360);

        bool m_transitionPhase = false;
        float m_transitionDT = 0.0f;
        float m_transitionTime = 1.0f;
        int m_queued = -1;

        bool useTempPos = false;


        public bool isActive()
        {
            return m_active;
        }
        public Vector2 getPosition()
        {
            return m_position;
        }

        public RadialMenuContext(GraphicsDevice p_gd, ContentManager p_content)
        {
            m_originalPosition = m_position = new Vector2(p_gd.Viewport.Width * 0.5f, p_gd.Viewport.Height * 0.5f);


            m_menus = new List<RadialMenu>();
            m_current = -1;


            size = 100;
            int thickness = (int)(size * 0.1f);
            m_circleTexture = new Texture2D(p_gd, (int)size, (int)size);
            Color[] data = new Color[(int)(size * size)];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int x = (int)Math.Abs(size * 0.5f - i);
                    int y = (int)Math.Abs(size * 0.5f - j);

                    float l = (float)Math.Sqrt((double)(x * x + y * y));

                    if (l > size * 0.5f - thickness && l < size * 0.5f)
                    {
                        data[j * (int)size + i] = Color.Black;
                    }
                    else
                    {
                        data[j * (int)size + i] = Color.Transparent;
                    }
                }
            }
            m_circleTexture.SetData<Color>(data);


            m_infoBox = new Texture2D(p_gd, (int)size * 2, (int)size);
            data = new Color[(int)(size * size * 2)];
            for (int i = 0; i < size * 2; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i < thickness || i > size * 2 - thickness ||
                        j < thickness || j > size - thickness)
                        data[j * (int)size * 2 + i] = Color.Black;
                    else
                        data[j * (int)size * 2 + i] = Color.Transparent;
                }
            }
            m_infoBox.SetData<Color>(data);

            m_font = p_content.Load<SpriteFont>("Arial8");

            m_arrow = p_content.Load<Texture2D>("arrow2");

            m_infoBox = p_content.Load<Texture2D>("textbox");

            prevMouseWheel = Mouse.GetState().ScrollWheelValue;
        }
        public void addRadialMenu(RadialMenu p_menu)
        {
            p_menu.setTexturesAndFonts(m_circleTexture, m_infoBox, m_arrow, m_font);
            m_menus.Add(p_menu);
            if (m_current == -1)
            {
                m_current = m_root = 0;
            }
        }
        public int getIndex(RadialMenu p_menu)
        {
            for (int i = 0; i < m_menus.Count; i++)
            {
                if (m_menus[i] == p_menu)
                    return i;
            }
            return -1;
        }
        private void setCurrent(int p_index)
        {
            //m_current = p_index;
            if (m_current != p_index)
            {
                if (m_active)
                {
                    m_transitionPhase = true;
                    m_queued = p_index;
                }
                else
                {
                    m_transitionPhase = false;
                    m_current = p_index;
                }
                m_active = true;
            }
        }
        public void update(float p_dt)
        {
            if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Mouse.GetState().MiddleButton == ButtonState.Pressed) && !m_altDown)
            {
                m_altDown = true;
                m_active = !m_active;
                if (!m_active)
                    m_current = m_root;

                m_position.X = m_originalPosition.X = Mouse.GetState().X;
                m_position.Y = m_originalPosition.Y = Mouse.GetState().Y;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Up) && Mouse.GetState().MiddleButton != ButtonState.Pressed)
            {
                m_altDown = false;
            }

            if (m_current >= 0 && m_active)
            {
                if (!m_transitionPhase)
                    m_menus[m_current].update(size, m_position);

                //Enter
                if ((Keyboard.GetState().IsKeyDown(Keys.Enter) || Mouse.GetState().LeftButton == ButtonState.Pressed) && !m_enterDown)
                {
                    m_enterDown = true;
                    RadialMenuItem curr = m_menus[m_current].GetCurrent();

                    if (curr != null)
                    {
                        if (curr.activateEvent != null && curr.activateEvent.callback != null)
                        {
                            curr.activateEvent.callback(curr.activateEvent.data);
                            m_active = false;
                            m_current = m_root;
                            return;
                        }
                        else
                        {
                            int index = -1;
                            for (int i = 0; i < m_menus.Count; i++)
                            {
                                if (m_menus[i] == curr.submenu)
                                    index = i;
                            }
                            if (index >= 0)
                            {
                                setCurrent(index);
                                return;
                            }
                        }
                    }
                    else
                    {
                        int index = -1;
                        for (int i = 0; i < m_menus.Count; i++)
                        {
                            if (m_menus[i] == m_menus[m_current].getParent())
                            {
                                index = i;
                            }
                        }
                        if (index >= 0)
                        {
                            setCurrent(index);
                        }
                        else
                        {
                            m_active = false;
                        }
                    }
                }
                else if (!Keyboard.GetState().IsKeyDown(Keys.Enter) && Mouse.GetState().LeftButton != ButtonState.Pressed)
                {
                    m_enterDown = false;
                }

                //Scroll Wheel
                if (Mouse.GetState().ScrollWheelValue > prevMouseWheel)
                {
                    size *= 1.2f;
                }
                else if (Mouse.GetState().ScrollWheelValue < prevMouseWheel)
                {
                    size /= 1.2f;
                    if (size < 50)
                        size = 50;
                }
                prevMouseWheel = Mouse.GetState().ScrollWheelValue;

                //+/- Keys
                if (Keyboard.GetState().IsKeyDown(Keys.Add))
                {
                    size *= (1.0f + 0.75f*p_dt);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                {
                    size *= (1 - 0.75f*p_dt);
                    if (size < 50)
                        size = 50;
                }

                if (m_transitionPhase)
                {
                    m_transitionDT += p_dt;
                    if (m_transitionDT > m_transitionTime)
                    {
                        m_transitionPhase = false;
                        m_transitionDT = 0;
                        m_current = m_queued;
                        m_menus[m_current].preSetMouse();
                        useTempPos = false;
                        m_position = m_originalPosition;
                    }
                }
            }
            for (int i = 0; i < m_menus.Count; i++)
            {
                if (KeyDelta.getDelta(m_menus[i].getHotkey()) > 0.0f &&
                    !Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    if (m_current == i && m_active)
                    {
                        m_active = false;
                        m_current = m_root;
                    }
                    else
                    {
                        setCurrent(i);
                        m_active = true;
                    }
                }
            }
        }
        public RadialMenu getRadialMenu(int p_id)
        {
            return m_menus[p_id];
        }
        public void draw1(SpriteBatch p_sb)
        {
            m_transitionTime = 0.5f;
            if (m_current >= 0 && m_active)
            {
                if (m_transitionPhase)
                {
                    m_menus[m_queued].setCurrentWithTarget(m_menus[m_current]);
                    if (m_transitionDT < m_transitionTime * 0.25f)
                    {
                        float opacity = 1.0f - m_transitionDT / (m_transitionTime * 0.25f);
                        if (m_menus[m_current].getParent() == m_menus[m_queued])
                            m_menus[m_current].draw(p_sb, m_position, size, opacity, true, false, true, false);
                        else
                            m_menus[m_current].draw(p_sb, m_position, size, opacity, false, true, true, false);
                    }
                    else if (m_transitionDT < m_transitionTime * 0.75f)
                    {
                        float moveFraction = (m_transitionDT - m_transitionTime * 0.25f) / (m_transitionTime * 0.5f);
                        if (m_menus[m_current].getParent() == m_menus[m_queued])
                        {
                            moveFraction = 1.0f - moveFraction;
                            m_menus[m_queued].drawOptionOnly(p_sb, m_position, size, moveFraction, true);
                        }
                        else
                            m_menus[m_current].drawOptionOnly(p_sb, m_position, size, moveFraction, true);
                    }
                    else
                    {
                        float opacity = (m_transitionDT - m_transitionTime * 0.75f) / (m_transitionTime * 0.25f);
                        if (m_menus[m_current].getParent() == m_menus[m_queued])
                        {
                            m_menus[m_queued].draw(p_sb, m_position, size, opacity, false, true, true, false);
                        }
                        else
                        {
                            m_menus[m_queued].draw(p_sb, m_position, size, opacity, true, false, true, false);
                        }
                    }
                }
                else
                    m_menus[m_current].draw(p_sb, m_position, size);
            }
        }
        public void draw2(SpriteBatch p_sb)
        {
            m_transitionTime = 0.5f;
            if (m_current >= 0 && m_active)
            {
                if (m_transitionPhase)
                {
                    if (m_transitionDT < m_transitionTime * 0.4f)
                    {
                        float frac = (m_transitionTime * 0.4f - m_transitionDT) / (m_transitionTime * 0.4f);
                        m_menus[m_current].drawSome(p_sb, m_position, size, frac);
                    }
                    else if (m_transitionDT < m_transitionTime * 0.6f)
                    {
                        m_menus[m_current].drawSome(p_sb, m_position, size, 0);
                    }
                    else
                    {
                        float frac = 1.0f - (m_transitionTime - m_transitionDT) / (m_transitionTime * 0.4f);
                        m_menus[m_queued].drawSome(p_sb, m_position, size, frac);
                    }
                }
                else
                    m_menus[m_current].draw(p_sb, m_position, size);
            }
        }
        public void draw3(SpriteBatch p_sb)
        {
            m_transitionTime = 1.0f;
            if (m_current >= 0 && m_active)
            {
                if (m_transitionPhase)
                {
                    if (m_transitionDT < m_transitionTime * 0.4f)
                    {
                        float opacity = 1.0f - m_transitionDT / (m_transitionTime * 0.4f);
                        m_menus[m_current].draw(p_sb, m_position, size, opacity);
                    }
                    else if (m_transitionDT < m_transitionTime * 0.6)
                    {
                        m_menus[m_current].draw(p_sb, m_position, size, 0.0f);
                    }
                    else
                    {
                        float opacity = (m_transitionDT - m_transitionTime * 0.6f) / (m_transitionTime * 0.4f);
                        m_menus[m_queued].draw(p_sb, m_position, size, opacity);
                    }
                }
                else
                    m_menus[m_current].draw(p_sb, m_position, size);
            }
        }
        public void draw4(SpriteBatch p_sb)
        {
            m_transitionTime = 0.5f;
            if (m_current >= 0 && m_active)
            {
                if (m_transitionPhase)
                {
                    if (m_transitionDT < m_transitionTime * 0.125f) //0.125
                    {
                        float opacity = 1.0f - m_transitionDT / (m_transitionTime * 0.125f);
                        m_menus[m_current].draw(p_sb, m_position, size, opacity, false, true, true);
                    }
                    else if (m_transitionDT < m_transitionTime * 0.5f) //0.375
                    {
                        float moveFraction = (m_transitionDT - m_transitionTime * 0.125f) / (m_transitionTime * 0.375f);
                        m_menus[m_current].drawOptionOnly(p_sb, m_position, size, moveFraction, true);
                    }
                    else //0.5f
                    {
                        float frac = (m_transitionDT - m_transitionTime * 0.5f) / (m_transitionTime * 0.5f);
                        m_menus[m_queued].drawSome(p_sb, m_position, size, frac, false);
                    }
                }
                else
                    m_menus[m_current].draw(p_sb, m_position, size);
            }
        }
        public void draw5(SpriteBatch p_sb)
        {
            m_transitionTime = 0.5f;
            if (m_current >= 0 && m_active)
            {
                if (m_transitionPhase)
                {
                    m_menus[m_queued].setCurrentWithTarget(m_menus[m_current]);
                    if (m_transitionDT < m_transitionTime * 0.25f)
                    {
                        float opacity = 1.0f - m_transitionDT / (m_transitionTime * 0.25f);
                        if (m_menus[m_current].getParent() == m_menus[m_queued])
                            m_menus[m_current].draw(p_sb, m_position, size, opacity, true, false, true, true);
                        else
                            m_menus[m_current].draw(p_sb, m_position, size, opacity, false, true, true, true);
                    }
                    else if (m_transitionDT < m_transitionTime * 0.75f)
                    {
                        float moveFraction = (m_transitionDT - m_transitionTime * 0.25f) / (m_transitionTime * 0.5f);
                        if (m_menus[m_current].getParent() == m_menus[m_queued])
                        {
                            moveFraction = 1.0f - moveFraction;
                            m_menus[m_queued].drawOptionOnly(p_sb, m_position, size, moveFraction, true);
                        }
                        else
                            m_menus[m_current].drawOptionOnly(p_sb, m_position, size, moveFraction, true);
                    }
                    else
                    {
                        float opacity = (m_transitionDT - m_transitionTime * 0.75f) / (m_transitionTime * 0.25f);                       
                        if (m_menus[m_current].getParent() == m_menus[m_queued])
                        {
                            m_menus[m_queued].draw(p_sb, m_position, size, opacity, false, true, true, true);
                        }
                        else
                        {
                            m_menus[m_queued].draw(p_sb, m_position, size, opacity, true, false, true, true);
                        }
                    }
                }
                else
                    m_menus[m_current].draw(p_sb, m_position, size);
            }
        }
        public void draw6(SpriteBatch p_sb)
        {
            m_transitionTime = 0.5f;
            if (m_current >= 0 && m_active)
            {
                if (m_transitionPhase)
                {
                    if (m_transitionDT < m_transitionTime * 0.125f) //0.125
                    {
                        float opacity = 1.0f - m_transitionDT / (m_transitionTime * 0.125f);
                        m_menus[m_current].draw(p_sb, m_position, size, opacity, false, true, true, true);
                    }
                    else if (m_transitionDT < m_transitionTime * 0.5f) //0.375
                    {
                        float moveFraction = (m_transitionDT - m_transitionTime * 0.125f) / (m_transitionTime * 0.375f);
                        m_menus[m_current].drawOptionOnly(p_sb, m_position, size, moveFraction, true);
                    }
                    else //0.5f
                    {
                        float frac = (m_transitionDT - m_transitionTime * 0.5f) / (m_transitionTime * 0.5f);
                        m_menus[m_queued].drawSome(p_sb, m_position, size, frac, false);
                    }
                }
                else
                    m_menus[m_current].draw(p_sb, m_position, size);
            }
        }
        public void draw7(SpriteBatch p_sb)
        {
            m_transitionTime = 0.0f;
            if (m_current >= 0 && m_active)
            {
                m_menus[m_current].draw(p_sb, m_position, size);
            }
        }

    }
}
