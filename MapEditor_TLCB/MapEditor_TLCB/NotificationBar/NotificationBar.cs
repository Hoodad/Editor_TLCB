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
    class NotificationBar
    {
        Texture2D m_barTexture;
        Texture2D m_warning;
        Texture2D m_error;
        Texture2D m_info;
        Texture2D m_success;

        int m_border;

        int m_width;
        int m_height;

        SpriteFont m_font;

        List<Notification> m_notifications;

        bool m_transition;
        float m_transitionDT;
        int m_transitionCurrent;

        int m_addType = 3;

        float m_transitionTime = 0.25f;

        Notification m_awaitingAdd;

        int m_barWidth;
        int m_barHeight;
        int m_barBorder;

        Texture2D m_borderTexture;

        Vector2 m_position = new Vector2(7, 28);//new Vector2(0, 50);

        bool m_aging = true;
        bool m_openOnHover = false;

        float m_timeNotOver = 100;
        float m_timeOver = 0;

        float m_maxAge = 10;

        int m_unseen = 0;

        Texture2D m_pin;
        Texture2D m_unpin;

        bool m_pressed = false;
        bool m_active = true;

        float m_randomAddTimer = 0;

        public NotificationBar(GraphicsDevice p_gd, ContentManager p_content, float p_width, float p_height)
        {
            m_width = (int)p_width;
            m_height = (int)p_height;
            m_border = 2;
            m_barTexture = new Texture2D(p_gd, m_width, m_height);
            m_transition = false;
            m_transitionDT = 0;
            m_transitionCurrent = 0;

            Color[] data = new Color[m_width * m_height];

            for (int i = 0; i < m_width; i++)
            {
                for (int j = 0; j < m_height; j++)
                {
                    if (i < m_border || j < m_border || m_width - i < m_border || m_height - j < m_border)
                        data[j * m_width + i] = Color.LightGray;
                    else
                        data[j * m_width + i] = Color.White;
                }
            }
            m_barTexture.SetData<Color>(data);

            m_success = p_content.Load<Texture2D>("ok");
            m_info = p_content.Load<Texture2D>("info");
            m_error = p_content.Load<Texture2D>("error");
            m_warning = p_content.Load<Texture2D>("warning");

            m_notifications = new List<Notification>();

            m_notifications.Add(new Notification("This is an information message.", NotificationType.INFO));
            m_notifications.Add(new Notification("This is a warning message", NotificationType.WARNING));
            m_notifications.Add(new Notification("This is an error message", NotificationType.ERROR));
            m_notifications.Add(new Notification("This is a success message", NotificationType.SUCCESS));

            m_font = p_content.Load<SpriteFont>("Arial10");


            //Initialize surrounding border
            m_barBorder = 10;
            m_barWidth = m_width + m_barBorder*2;
            m_barHeight = m_height*5 + m_barBorder*2;
            m_borderTexture = new Texture2D(p_gd, m_barWidth, m_barHeight);
            Color[] borderData = new Color[m_barWidth * m_barHeight];
            for (int i = 0; i < m_barWidth * m_barHeight; i++)
            {
                borderData[i] = Color.White;
            }
            m_borderTexture.SetData<Color>(borderData);

            m_pin = p_content.Load<Texture2D>("pinOn");
            m_unpin = p_content.Load<Texture2D>("pinOff");

        }
        public void addNotification(Notification p_notification)
        {
            if (m_addType == 0 || m_addType == 2)
            {
                int ind = m_notifications.Count - Math.Min(m_notifications.Count, 5);
                m_notifications.Insert(ind, p_notification);
                m_transition = true;
                m_transitionCurrent = ind;
                m_transitionDT = 0;
            }
            else if (m_addType == 3)
            {
                m_awaitingAdd = p_notification;
                m_transition = true;
                m_transitionDT = 0;
            }
            else
            {
                m_notifications.Add(p_notification);
            }

            if (m_openOnHover)
                m_unseen++;
        }
        public void setAddType(int p_addType)
        {
            m_addType = p_addType;
            if (m_addType == 2)
                m_transitionTime = 0.1f;
            else if (m_addType == 3)
                m_transitionTime = 0.25f;
            else
                m_transitionTime = 0.1f;
        }

        public void setAging(bool p_value)
        {
            m_aging = p_value;
        }
        public bool getAging()
        {
            return m_aging;
        }
        public void setOpenOnHover(bool p_value)
        {
            m_openOnHover = p_value;
        }
        public bool getOpenOnHover()
        {
            return m_openOnHover;
        }

        public void update(float p_dt)
        {
            if (m_transition)
            {
                m_transitionDT += p_dt;
                if (m_transitionDT > m_transitionTime)
                {
                    if (m_addType < 3)
                    {
                        m_transitionDT -= m_transitionTime;
                        if (m_transitionCurrent + 1 < m_notifications.Count)
                        {
                            Notification temp = m_notifications[m_transitionCurrent];
                            m_notifications[m_transitionCurrent] = m_notifications[m_transitionCurrent + 1];
                            m_notifications[m_transitionCurrent + 1] = temp;
                            m_transitionCurrent++;
                        }
                        else
                        {
                            m_transition = false;
                        }
                    }
                    else
                    {
                        m_transition = false;
                        m_transitionDT = 0;
                        m_notifications.Add(m_awaitingAdd);
                    }
                }
            }

            for (int i = 0; i < m_notifications.Count; i++)
            {
                if (m_aging)
                    m_notifications[i].age += p_dt;
                else
                    m_notifications[i].age = 0;
            }

            //Pin
            Rectangle dest;
            dest.X = (int)m_position.X;
            dest.Y = (int)m_position.Y - 25;
            dest.Width = 150;
            dest.Height = 25;

            //Border
            Rectangle borderRect;
            borderRect.X = (int)m_position.X;
            borderRect.Y = (int)m_position.Y;
            borderRect.Width = m_barWidth;
            borderRect.Height = m_barHeight;

            if (m_openOnHover)
            {
                if (dest.Contains(Mouse.GetState().X, Mouse.GetState().Y) ||
                    borderRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    m_timeOver += p_dt;
                    m_timeNotOver = 0.0f;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        m_timeOver = 0.51f;

                    if (m_timeOver > 0.5f)
                    {
                        m_position.Y = 623;
                        m_unseen = 0;
                        m_active = true;
                    }
                }
                else
                {
                    m_timeOver = 0;
                    m_timeNotOver += p_dt;
                    if (m_timeNotOver > 1.0f)
                    {
                        m_position.Y = 768;
                        m_active = false;
                    }
                }

                //Check for collision against the pin
            }
            else
            {
                m_active = true;
            }

            dest.X = (int)m_position.X + 125;
            dest.Width = 25;
            if (dest.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!m_pressed)
                    {
                        m_openOnHover = !m_openOnHover;
                        if (m_openOnHover)
                        {
                            m_active = false;
                            m_position.Y = 768;
                        }
                    }
                    m_pressed = true;
                }
                else
                    m_pressed = false;
            }

            m_randomAddTimer += p_dt;
            if (m_randomAddTimer > 2.0f)
            {
                m_randomAddTimer = 0;

                Random random = new Random();
                int rnd = random.Next(4);
                Notification note = null;
                if (rnd == 0)
                {
                    note = new Notification("Anton pwns Robin", NotificationType.ERROR);
                }
                else if (rnd == 1)
                {
                    note = new Notification("Anton l33t, Johan N00b", NotificationType.WARNING);
                }
                else if (rnd == 2)
                {
                    note = new Notification("Anton handles Jarl like a small potatoe", NotificationType.SUCCESS);
                }
                else
                {
                    note = new Notification("Anton kills all with cool rocket launcher", NotificationType.INFO);
                }
                addNotification(note);
            }
        }
        public void draw(SpriteBatch p_sb)
        {

            //p_sb.Draw(m_borderTexture, m_position, Color.Black);

            if (m_addType == 0 || m_addType == 1)
            {
            }
            else if (m_addType == 2)
            {
            }
            else
            {
                draw3(p_sb);
            }

            //Pin
            /*Rectangle dest;
            dest.X = (int)m_position.X;
            dest.Y = (int)m_position.Y - 25;
            dest.Width = 150;
            dest.Height = 25;

            Color pinColor = Color.Black;
            Color textColor = Color.White;
            if (dest.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                textColor = Color.LightGreen;

            p_sb.Draw(m_borderTexture, dest, pinColor);

            //Pin icon
            dest.X = (int)m_position.X + 125;
            dest.Y = (int)m_position.Y - 25;
            dest.Width = 25;
            dest.Height = 25;
            Color c = new Color(255, 100, 100, 255);
            if (dest.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                textColor = Color.White;
                c = Color.LightGreen;
            }

            if (m_active)
            {
                if (m_openOnHover)
                    p_sb.Draw(m_pin, dest, c);
                else
                    p_sb.Draw(m_unpin, dest, c);
            }

            string text = "Notification Bar";

            Vector2 textSize = m_font.MeasureString(text);

            if (m_unseen > 0)
            {
                text += " (" + m_unseen.ToString() + ")";
            }

            p_sb.DrawString(m_font, text, m_position - new Vector2(-50 + textSize.X*0.5f, 12.5f + textSize.Y*0.5f), textColor);*/

        }
        public void draw3(SpriteBatch p_sb)
        {
            Color warningColor = new Color(255, 242, 184, 255);
            Color successColor = new Color(185, 255, 182, 255);
            Color errorColor = new Color(255, 202, 202, 255);
            Color infoColor = new Color(176, 224, 230, 255);

            float offset = 0;
            if (m_transition)
            {
                offset = m_transitionDT / m_transitionTime;
                offset *= m_height;
            }

            Vector2 startPos = m_position + new Vector2(0, offset);// +new Vector2(m_barBorder, m_barBorder + offset);
            for (int i = 0; i < 5; i++)
            {
                if (m_transition && i == 4)
                    break;
                Vector2 barPos = startPos + new Vector2(0, m_height * i);
                p_sb.Draw(m_barTexture, barPos, Color.White);

                if (m_notifications.Count - i - 1 >= 0)
                {
                    Notification n = m_notifications[m_notifications.Count - i - 1];
                    Texture2D icon = m_info;
                    Color color = infoColor;
                    if (n.type == NotificationType.ERROR)
                    {
                        color = errorColor;
                        icon = m_error;
                    }
                    else if (n.type == NotificationType.SUCCESS)
                    {
                        color = successColor;
                        icon = m_success;
                    }
                    else if (n.type == NotificationType.WARNING)
                    {
                        color = warningColor;
                        icon = m_warning;
                    }

                    float grayness = Math.Min(n.age / m_maxAge, 1.0f);

                    Color barColor = Color.White;
                    barColor.R = (byte)(color.R * (1 - grayness) + Color.White.R * grayness);
                    barColor.G = (byte)(color.G * (1 - grayness) + Color.White.G * grayness);
                    barColor.B = (byte)(color.B * (1 - grayness) + Color.White.B * grayness);
                    barColor.A = (byte)(color.A * (1 - grayness) + Color.White.A * grayness);

                    Rectangle rect;
                    rect.X = (int)barPos.X;
                    rect.Y = (int)barPos.Y;
                    rect.Width = m_width;
                    rect.Height = m_height;

                    if (rect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                        barColor = color;

                    p_sb.Draw(m_barTexture, barPos, barColor);
                    rect.X = (int)(barPos.X + m_border);
                    rect.Y = (int)(barPos.Y + m_border);
                    rect.Width = m_height - m_border * 2;
                    rect.Height = m_height - m_border * 2;

                    p_sb.Draw(icon, rect, barColor);

                    Vector2 textSize = m_font.MeasureString(n.message);

                    Vector2 textPos = barPos + new Vector2(0, 0.5f * (m_height - textSize.Y));

                    textPos.X += m_border + m_height;

                    p_sb.DrawString(m_font, n.message, textPos, Color.Black);
                }
            }
            if (m_transition) //Fade out last
            {
                float p_opacity = 1.0f - m_transitionDT / m_transitionTime;

                Vector2 barPos = startPos + new Vector2(0, m_height * (4));

                Rectangle sourceRect;
                sourceRect.X = 0;
                sourceRect.Y = 0;
                sourceRect.Width = m_barTexture.Width;
                sourceRect.Height = (int)(p_opacity * m_barTexture.Height);

                p_sb.Draw(m_barTexture, barPos, sourceRect, Color.White);

                if (m_notifications.Count - 4 - 1 >= 0)
                {

                    Notification n = m_notifications[m_notifications.Count - 4 - 1];
                    Texture2D icon = m_info;
                    Color color = infoColor;
                    if (n.type == NotificationType.ERROR)
                    {
                        color = errorColor;
                        icon = m_error;
                    }
                    else if (n.type == NotificationType.SUCCESS)
                    {
                        color = successColor;
                        icon = m_success;
                    }
                    else if (n.type == NotificationType.WARNING)
                    {
                        color = warningColor;
                        icon = m_warning;
                    }

                    float grayness = Math.Min(n.age / m_maxAge, 1.0f);

                    Color barColor = Color.White;
                    barColor.R = (byte)(color.R * (1 - grayness) + Color.White.R * grayness);
                    barColor.G = (byte)(color.G * (1 - grayness) + Color.White.G * grayness);
                    barColor.B = (byte)(color.B * (1 - grayness) + Color.White.B * grayness);
                    barColor.A = (byte)(color.A * (1 - grayness) + Color.White.A * grayness);

                    Rectangle rect;
                    rect.X = (int)barPos.X;
                    rect.Y = (int)barPos.Y;
                    rect.Width = m_width;
                    rect.Height = m_height;

                    if (rect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                        barColor = color;

                    barColor *= p_opacity;

                    p_sb.Draw(m_barTexture, barPos, sourceRect, barColor);



                    rect.X = (int)barPos.X + m_border;
                    rect.Y = (int)barPos.Y + m_border;
                    rect.Width = m_height - m_border * 2;
                    rect.Height = (int)((m_height - m_border * 2) * p_opacity);

                    sourceRect.X = 0;
                    sourceRect.Y = 0;
                    sourceRect.Width = icon.Width;
                    sourceRect.Height = (int)(p_opacity * icon.Height);

                    p_sb.Draw(icon, rect, sourceRect, barColor);

                    Vector2 textSize = m_font.MeasureString(n.message);

                    Vector2 textPos = barPos + new Vector2(0, 0.5f * (m_height - textSize.Y));

                    textPos.X += m_border + m_height;

                    //p_sb.DrawString(m_font, n.message, textPos, Color.Black);
                }
            }
            if (m_transition) //Fade in new
            {
                float p_opacity = m_transitionDT / m_transitionTime;

                //p_sb.Draw(m_barTexture, new Vector2(0, 480 - m_height * (5 - (-1)) + offset), Color.White);

                Notification n = m_awaitingAdd;
                Texture2D icon = m_info;
                Color color = infoColor;
                if (n.type == NotificationType.ERROR)
                {
                    color = errorColor;
                    icon = m_error;
                }
                else if (n.type == NotificationType.SUCCESS)
                {
                    color = successColor;
                    icon = m_success;
                }
                else if (n.type == NotificationType.WARNING)
                {
                    color = warningColor;
                    icon = m_warning;
                }

                float grayness = Math.Min(n.age / m_maxAge, 1.0f);

                Vector2 barPos = startPos + new Vector2(0, m_height * (-1));

                Color barColor = Color.White;
                barColor.R = (byte)(color.R * (1 - grayness) + Color.White.R * grayness);
                barColor.G = (byte)(color.G * (1 - grayness) + Color.White.G * grayness);
                barColor.B = (byte)(color.B * (1 - grayness) + Color.White.B * grayness);
                barColor.A = (byte)(color.A * (1 - grayness) + Color.White.A * grayness);

                Rectangle rect;
                rect.X = (int)barPos.X;
                rect.Y = (int)barPos.Y;
                rect.Width = m_width;
                rect.Height = m_height;

                if (rect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    barColor = color;

                barColor *= p_opacity;

                Rectangle sourceRect;
                sourceRect.X = 0;
                sourceRect.Y = (int)((1 - p_opacity) * m_barTexture.Height);
                sourceRect.Width = m_barTexture.Width;
                sourceRect.Height = (int)(p_opacity*m_barTexture.Height);

                p_sb.Draw(m_barTexture, barPos + new Vector2(0, m_barTexture.Height*(1-p_opacity)), sourceRect, barColor);



                rect.X = (int)barPos.X + m_border;
                rect.Y = (int)barPos.Y + m_border + (int)(m_barTexture.Height * (1 - p_opacity));
                rect.Width = m_height - m_border * 2;
                rect.Height = (int)((m_height - m_border * 2) * p_opacity);

                sourceRect.X = 0;
                sourceRect.Y = (int)((1 - p_opacity) * icon.Height);
                sourceRect.Width = icon.Width;
                sourceRect.Height = (int)(p_opacity * icon.Height);

                p_sb.Draw(icon, rect, sourceRect, barColor);

                Vector2 textSize = m_font.MeasureString(n.message);

                Vector2 textPos = barPos + new Vector2(0, 0.5f * (m_height - textSize.Y));

                textPos.X += m_border + m_height;

                //p_sb.DrawString(m_font, n.message, textPos, Color.Black);
            }
        }
    }
}
