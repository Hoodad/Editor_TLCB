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
    public class RadialMenu
    {
        private List<RadialMenuItem> m_items;

        private Texture2D m_circleTexture;

        private Texture2D m_infoBox;

        private Texture2D m_arrow;

        private SpriteFont m_font;

        private float rot = 0.0f;
        private int arrowTarget = -1;

        bool m_leftDown = false;
        bool m_rightDown = false;

        int prevMouseX = 0;
        int prevMouseY = 0;

        private Texture2D m_symbol;

        private RadialMenu m_parent;

        private Keys m_hotKey;

        public RadialMenu(GraphicsDevice p_gd, ContentManager p_content,
                            List<RadialMenuItem> p_items, Texture2D p_symbol,
                                RadialMenu p_parent)
        {
            m_symbol = p_symbol;
            m_items = p_items;
            m_parent = p_parent;
            m_hotKey = Keys.None;
        }

        public void addItem(RadialMenuItem p_item)
        {
            m_items.Add(p_item);
        }

        public RadialMenu getParent()
        {
            return m_parent;
        }
        public void setParent(RadialMenu p_parent)
        {
            m_parent = p_parent;
        }

        public RadialMenuItem GetCurrent()
        {
            if (arrowTarget < 0)
                return null;
            return m_items[arrowTarget];
        }
        public void setTexturesAndFonts(Texture2D p_circleTexture, Texture2D p_infoBox, Texture2D p_arrow, SpriteFont p_font)
        {
            m_circleTexture = p_circleTexture;
            m_infoBox = p_infoBox;
            m_arrow = p_arrow;
            m_font = p_font;
        }

        public void update(float p_size, Vector2 p_position)
        {
            //Left
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && !m_leftDown)
            {
                m_leftDown = true;
                arrowTarget = arrowTarget-1;
                if (arrowTarget < -1)
                    arrowTarget = m_items.Count - 1;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                m_leftDown = false;
            }

            //Right
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && !m_rightDown)
            {
                m_rightDown = true;
                arrowTarget = arrowTarget+1;
                if (arrowTarget >= m_items.Count)
                    arrowTarget = -1;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                m_rightDown = false;
            }

            int mouseX = Mouse.GetState().X;
            int mouseY = Mouse.GetState().Y;

            if (mouseX != prevMouseX || mouseY != prevMouseY)
            {
                prevMouseX = mouseX;
                prevMouseY = mouseY;
                int col = checkCollision(p_size, p_position, Mouse.GetState().X, Mouse.GetState().Y);
                if (col >= -1)
                {
                    arrowTarget = col;
                }
            }

            //Control
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Keys[] pressed = Keyboard.GetState().GetPressedKeys();
                for (int i = 0; i < pressed.Length; i++)
                {
                    if (pressed[i] >= Keys.A && pressed[i] <= Keys.Z)
                    {
                        if (m_items[arrowTarget].activateEvent != null)
                        {
                            m_items[arrowTarget].activateEvent.hotkey = pressed[i];
                            break;
                        }
                        else if (m_items[arrowTarget].submenu != null)
                        {
                            m_items[arrowTarget].submenu.m_hotKey = pressed[i];
                        }
                    }
                }
            }
        }
        public void preSetMouse()
        {
            prevMouseX = Mouse.GetState().X;
            prevMouseY = Mouse.GetState().Y;
        }
        public Keys getHotkey()
        {
            return m_hotKey;
        }
        public void setCurrentWithTarget(RadialMenu p_menu)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                if (m_items[i].submenu == p_menu)
                {
                    arrowTarget = i;
                }
            }
        }

        public Vector2 getSelectedPosition(Vector2 m_position, float size)
        {
            float distance = 1.8f * size;

            Vector2 drawPos = m_position - new Vector2(size * 0.5f, size * 0.5f);

            float spawnArea = (float)(5 * Math.PI / 4);

            float startRot = ((float)(Math.PI * 2) - spawnArea) * 0.5f;

            float arrowRot = startRot;

            float sin = (float)Math.Sin(startRot);
            float cos = (float)Math.Cos(startRot);

            Vector2 dir = new Vector2(0, 1);

            float x = cos * dir.X - sin * dir.Y;
            float y = sin * dir.X + cos * dir.Y;
            dir.X = x;
            dir.Y = y;

            float rotPerObj = spawnArea / (m_items.Count - 1);

            sin = (float)Math.Sin(rotPerObj);
            cos = (float)Math.Cos(rotPerObj);

            rot += 0.05f;

            Vector2 arrowDir = new Vector2(0, 1);

            for (int i = 0; i < m_items.Count; i++)
            {
                Vector2 IconPos = m_position + dir * distance;

                if (i == arrowTarget)
                    return IconPos;

                x = cos * dir.X - sin * dir.Y;
                y = sin * dir.X + cos * dir.Y;
                dir.X = x;
                dir.Y = y;
            }
            return Vector2.Zero;
        }

        public int checkCollision(float size, Vector2 m_position, int p_x, int p_y)
        {
            float distance = 1.8f * size;

            Vector2 drawPos = m_position - new Vector2(size * 0.5f, size * 0.5f);

            float spawnArea = (float)(5 * Math.PI / 4);

            float startRot = ((float)(Math.PI * 2) - spawnArea) * 0.5f;

            float sin = (float)Math.Sin(startRot);
            float cos = (float)Math.Cos(startRot);

            Vector2 dir = new Vector2(0, 1);

            float x = cos * dir.X - sin * dir.Y;
            float y = sin * dir.X + cos * dir.Y;
            dir.X = x;
            dir.Y = y;

            float rotPerObj = spawnArea / (m_items.Count - 1);

            sin = (float)Math.Sin(rotPerObj);
            cos = (float)Math.Cos(rotPerObj);

            rot += 0.05f;

            Vector2 arrowDir = new Vector2(0, 1);

            Vector2 mousePos = new Vector2(p_x, p_y);

            float minDist = float.MaxValue;
            int ind = -2;

            for (int i = 0; i < m_items.Count; i++)
            {
                Vector2 IconPos = m_position + dir * distance;

                if ((IconPos - mousePos).LengthSquared() < minDist)
                {
                    minDist = (IconPos - mousePos).LengthSquared();
                    ind = i;
                }

                /*int rectSize = (int)(size / (float)Math.Sqrt(2.0));

                Rectangle drawRect;
                drawRect.X = (int)(IconPos.X - rectSize * 0.5f);
                drawRect.Y = (int)(IconPos.Y - rectSize * 0.5f);

                drawRect.Width = rectSize;
                drawRect.Height = rectSize;

                //Check Collision Here
                if (drawRect.Contains(p_x, p_y))
                    return i;*/

                x = cos * dir.X - sin * dir.Y;
                y = sin * dir.X + cos * dir.Y;
                dir.X = x;
                dir.Y = y;
            }

            if ((m_position - mousePos).LengthSquared() < minDist)
            {
                minDist = (m_position - mousePos).LengthSquared();
                ind = -1;
            }

            return ind;
        }

        public void draw(SpriteBatch sp, Vector2 m_position, float size, float p_opacity = 1.0f, bool p_centerOpaque = false, bool p_selectedOpaque = false, bool p_infoBoxOpaque = false,
                                bool p_shrink = false)
        {
            float sizeFraction = 1.0f;
            if (p_shrink)
                sizeFraction = p_opacity;

            Color drawColor = Color.White * p_opacity;

            float distance = 1.8f * size;

            Vector2 drawPos = m_position - new Vector2(size * 0.5f, size * 0.5f);

            float spawnArea = (float)(5 * Math.PI / 4);

            float startRot = ((float)(Math.PI * 2) - spawnArea) * 0.5f;

            float arrowRot = startRot;

            if (arrowTarget < 0)
                arrowRot = (float)Math.PI;

            float sin = (float)Math.Sin(startRot);
            float cos = (float)Math.Cos(startRot);

            Vector2 dir = new Vector2(0, 1);

            float x = cos * dir.X - sin * dir.Y;
            float y = sin * dir.X + cos * dir.Y;
            dir.X = x;
            dir.Y = y;

            float rotPerObj = spawnArea / (m_items.Count - 1);
            if (m_items.Count < 2)
                rotPerObj = 0;

            sin = (float)Math.Sin(rotPerObj);
            cos = (float)Math.Cos(rotPerObj);

            rot += 0.05f;

            Vector2 arrowDir = new Vector2(0, 1);

            for (int i = 0; i < m_items.Count; i++)
            {
                if (i == arrowTarget)
                {
                    arrowDir = dir;
                    arrowRot += rotPerObj * i;
                }

                Rectangle drawRect;
                Vector2 IconPos = m_position + dir * distance;

                int rectSize = (int)(m_items[i].scale * size / (float)Math.Sqrt(2.0));

                if (i == arrowTarget && p_selectedOpaque)
                {
                    drawRect.X = (int)(IconPos.X - rectSize * 0.5f);
                    drawRect.Y = (int)(IconPos.Y - rectSize * 0.5f);

                    drawRect.Width = rectSize;
                    drawRect.Height = rectSize;

                    sp.Draw(m_items[i].texture, drawRect, m_items[i].sourceRect, Color.White);
                }
                else
                {
                    float rectSize2 = rectSize * sizeFraction;
                    drawRect.X = (int)(IconPos.X - rectSize2 * 0.5f);
                    drawRect.Y = (int)(IconPos.Y - rectSize2 * 0.5f);

                    drawRect.Width = (int)rectSize2;
                    drawRect.Height = (int)rectSize2;

                    Color c = drawColor;
                    if (i != arrowTarget)
                    {
                        byte alpha = drawColor.A;
                        c *= 0.5f;
                        c.A = alpha;
                    }

                    sp.Draw(m_items[i].texture, drawRect, m_items[i].sourceRect, c);
                }
                //if (i == arrowTarget)
                    //sp.Draw(m_circleTexture, IconPos - new Vector2(size, size)*0.5f, Color.White);

                x = cos * dir.X - sin * dir.Y;
                y = sin * dir.X + cos * dir.Y;
                dir.X = x;
                dir.Y = y;
            }

            Rectangle dest;
            dest.X = (int)drawPos.X;
            dest.Y = (int)drawPos.Y;
            dest.Width = (int)size;
            dest.Height = (int)size;

            //sp.Draw(m_circleTexture, dest, drawColor);
            if (m_symbol != null)
            {
                Color c = drawColor;
                if (p_centerOpaque)
                    c = Color.White;

                if (arrowTarget >= 0)
                {
                    byte alpha = c.A;
                    c *= 0.7f;
                    c.A = alpha;
                }

                if (p_centerOpaque)
                    sp.Draw(m_symbol, dest, c);
                else
                    sp.Draw(m_symbol, dest, c);
            }

            Vector2 drawBoxPos = m_position - new Vector2(size * 2 * 0.5f, size * 0.5f) + distance * new Vector2(0, 1);
            dest.X = (int)drawBoxPos.X;
            dest.Y = (int)drawBoxPos.Y;
            dest.Width = (int)(size*2);
            dest.Height = (int)size;

            if (p_infoBoxOpaque)
                sp.Draw(m_infoBox, dest, Color.White);
            else
                sp.Draw(m_infoBox, dest, drawColor);


            string text = "Go Back";
            if (m_parent == null)
                text = "Close";
            if (arrowTarget >= 0)
                text = m_items[arrowTarget].text;

            float textSize = size / 50.0f;

            Vector2 wordSize = m_font.MeasureString(text) * textSize;

            Vector2 drawTextPos = m_position - wordSize * 0.5f + distance * new Vector2(0, 1) + new Vector2(-size+wordSize.X*0.5f, -size*0.5f+wordSize.Y*0.5f);

            drawTextPos += new Vector2(size / 10.0f, size / 10.0f);

            sp.DrawString(m_font, text, drawTextPos, drawColor, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            if (arrowTarget >= 0)
            {
                if (m_items[arrowTarget].activateEvent != null &&
                    m_items[arrowTarget].activateEvent.hotkey != Keys.None)
                {
                    string hotKey = Enum.GetName(typeof(Keys), m_items[arrowTarget].activateEvent.hotkey);
                    text = "Hotkey(" + hotKey + ")";
                    sp.DrawString(m_font, text, drawTextPos + new Vector2(0, wordSize.Y), drawColor, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);
                }
                else
                {
                    string hotKey = Enum.GetName(typeof(Keys), Keys.None);
                    if (m_items[arrowTarget].submenu != null)
                        hotKey = Enum.GetName(typeof(Keys), m_items[arrowTarget].submenu.getHotkey());
                    text = "Hotkey(" + hotKey + ")";
                    sp.DrawString(m_font, text, drawTextPos + new Vector2(0, wordSize.Y), drawColor, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);
                }
            }

            arrowDir *= distance * 0.5f;

            Vector2 origin = new Vector2(m_arrow.Width * 0.5f, m_arrow.Height * 0.5f);
            dest.X = (int)(m_position.X + arrowDir.X);
            dest.Y = (int)(m_position.Y + arrowDir.Y);
            dest.Width = (int)(size * 0.6f);
            dest.Height = (int)(size * 0.6f);

            sp.Draw(m_arrow, dest, null, drawColor, arrowRot, origin, SpriteEffects.None, 0.0f);
        }
        public void drawSome(SpriteBatch sp, Vector2 m_position, float size, float p_fraction, bool p_fade = true, float p_midIconScale = 1.0f)
        {
            float opacity = 1.0f;
            if (p_fade)
                opacity = p_fraction;
            Color drawColor = Color.White;

            float distance = 1.8f * size;

            float minSize = (size / (float)Math.Sqrt(2.0));
            float midIconSize = minSize * (1.0f - p_midIconScale) + p_midIconScale * size;

            Vector2 drawPos = m_position - new Vector2(midIconSize * 0.5f, midIconSize * 0.5f);

            float spawnArea = (float)(5 * Math.PI / 4);

            float startRot = ((float)(Math.PI * 2) - spawnArea) * 0.5f;

            float arrowRot = startRot;

            if (arrowTarget < 0)
                arrowRot = (float)Math.PI;

            float sin = (float)Math.Sin(startRot);
            float cos = (float)Math.Cos(startRot);

            Vector2 dir = new Vector2(0, 1);

            float x = cos * dir.X - sin * dir.Y;
            float y = sin * dir.X + cos * dir.Y;
            dir.X = x;
            dir.Y = y;

            float rotPerObj = spawnArea / (m_items.Count - 1);

            sin = (float)Math.Sin(rotPerObj);
            cos = (float)Math.Cos(rotPerObj);

            rot += 0.05f;

            Vector2 arrowDir = new Vector2(0, 1);

            for (int i = 0; i < m_items.Count*p_fraction; i++)
            {
                if (i == arrowTarget)
                {
                    arrowDir = dir;
                    arrowRot += rotPerObj * i;
                }

                Rectangle drawRect;
                Vector2 IconPos = m_position + dir * distance;

                int rectSize = (int)(m_items[i].scale * size / (float)Math.Sqrt(2.0));

                drawRect.X = (int)(IconPos.X - rectSize * 0.5f);
                drawRect.Y = (int)(IconPos.Y - rectSize * 0.5f);

                drawRect.Width = rectSize;
                drawRect.Height = rectSize;

                if (i == arrowTarget)
                    sp.Draw(m_items[i].texture, drawRect, m_items[i].sourceRect, Color.White);
                else
                {
                    Color c = drawColor;
                        byte alpha = drawColor.A;
                        c *= 0.5f;
                        c.A = alpha;
                        sp.Draw(m_items[i].texture, drawRect, m_items[i].sourceRect, c);
                }
                //if (i == arrowTarget)
                //sp.Draw(m_circleTexture, IconPos - new Vector2(size, size)*0.5f, Color.White);

                x = cos * dir.X - sin * dir.Y;
                y = sin * dir.X + cos * dir.Y;
                dir.X = x;
                dir.Y = y;
            }

            Rectangle dest;
            dest.X = (int)drawPos.X;
            dest.Y = (int)drawPos.Y;
            dest.Width = (int)(midIconSize);
            dest.Height = (int)(midIconSize);

            //sp.Draw(m_circleTexture, dest, drawColor);
            if (m_symbol != null)
            {
                Color c = drawColor;
                if (arrowTarget >= 0)
                {
                    byte alpha = c.A;
                    c *= 0.7f;
                    c.A = alpha;
                }
                sp.Draw(m_symbol, dest, c * opacity);
            }

            Vector2 drawBoxPos = m_position - new Vector2(size * 2 * 0.5f, size * 0.5f) + distance * new Vector2(0, 1);
            dest.X = (int)drawBoxPos.X;
            dest.Y = (int)drawBoxPos.Y;
            dest.Width = (int)(size * 2);
            dest.Height = (int)size;

            sp.Draw(m_infoBox, dest, drawColor);


            string text = "Go Back";
            if (m_parent == null)
                text = "Close";
            if (arrowTarget >= 0)
                text = m_items[arrowTarget].text;

            float textSize = size / 50.0f;

            Vector2 wordSize = m_font.MeasureString(text) * textSize;

            Vector2 drawTextPos = m_position - wordSize * 0.5f + distance * new Vector2(0, 1) + new Vector2(-size + wordSize.X * 0.5f, -size * 0.5f + wordSize.Y * 0.5f);

            drawTextPos += new Vector2(size / 10.0f, size / 10.0f);

            sp.DrawString(m_font, text, drawTextPos, drawColor * opacity, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            if (arrowTarget >= 0 && m_items[arrowTarget].activateEvent != null && m_items[arrowTarget].activateEvent.hotkey != Keys.None)
            {
                string hotKey = Enum.GetName(typeof(Keys), m_items[arrowTarget].activateEvent.hotkey);
                text = "Hotkey(" + hotKey + ")";
                sp.DrawString(m_font, text, drawTextPos + new Vector2(0, wordSize.Y), drawColor * opacity, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);
            }
            else
            {
                string hotKey = Enum.GetName(typeof(Keys), Keys.None);
                if (m_items[arrowTarget].submenu != null)
                    hotKey = Enum.GetName(typeof(Keys), m_items[arrowTarget].submenu.getHotkey());
                text = "Hotkey(" + hotKey + ")";
                sp.DrawString(m_font, text, drawTextPos + new Vector2(0, wordSize.Y), drawColor, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);
            }

            arrowDir *= distance * 0.5f;

            Vector2 origin = new Vector2(m_arrow.Width * 0.5f, m_arrow.Height * 0.5f);
            dest.X = (int)(m_position.X + arrowDir.X);
            dest.Y = (int)(m_position.Y + arrowDir.Y);
            dest.Width = (int)(size * 0.6f);
            dest.Height = (int)(size * 0.6f);

            if (p_fraction * m_items.Count > arrowTarget)
                sp.Draw(m_arrow, dest, null, drawColor * opacity, arrowRot, origin, SpriteEffects.None, 0.0f);
        }
        public void drawOptionOnly(SpriteBatch sp, Vector2 m_position, float size, float p_moveFraction, bool p_infoBoxOpaque = false)
        {
            Color drawColor = Color.White;

            float distance = 1.8f * size;

            Vector2 drawPos = m_position - new Vector2(size * 0.5f, size * 0.5f);

            float spawnArea = (float)(5 * Math.PI / 4);

            float startRot = ((float)(Math.PI * 2) - spawnArea) * 0.5f;

            float arrowRot = startRot;

            float sin = (float)Math.Sin(startRot);
            float cos = (float)Math.Cos(startRot);

            Vector2 dir = new Vector2(0, 1);

            float x = cos * dir.X - sin * dir.Y;
            float y = sin * dir.X + cos * dir.Y;
            dir.X = x;
            dir.Y = y;

            float rotPerObj = spawnArea / (m_items.Count - 1);

            sin = (float)Math.Sin(rotPerObj);
            cos = (float)Math.Cos(rotPerObj);

            rot += 0.05f;

            Vector2 arrowDir = new Vector2(0, 1);

            for (int i = 0; i < m_items.Count; i++)
            {
                if (i == arrowTarget)
                {
                    Rectangle drawRect;
                    Vector2 IconPos = m_position + dir * distance * (1.0f - p_moveFraction);

                    int rectSize = (int)(m_items[i].scale * size / (float)Math.Sqrt(2.0));

                    rectSize = (int)((1.0f - p_moveFraction) * rectSize + (p_moveFraction * size)); 

                    drawRect.X = (int)(IconPos.X - rectSize * 0.5f);
                    drawRect.Y = (int)(IconPos.Y - rectSize * 0.5f);

                    drawRect.Width = rectSize;
                    drawRect.Height = rectSize;

                    sp.Draw(m_items[i].texture, drawRect, m_items[i].sourceRect, Color.White);
                }
                x = cos * dir.X - sin * dir.Y;
                y = sin * dir.X + cos * dir.Y;
                dir.X = x;
                dir.Y = y;
            }

            if (p_infoBoxOpaque)
            {
                Rectangle dest;

                Vector2 drawBoxPos = m_position - new Vector2(size * 2 * 0.5f, size * 0.5f) + distance * new Vector2(0, 1);
                dest.X = (int)drawBoxPos.X;
                dest.Y = (int)drawBoxPos.Y;
                dest.Width = (int)(size * 2);
                dest.Height = (int)size;

                sp.Draw(m_infoBox, dest, drawColor);
            }
        }
    }
}
