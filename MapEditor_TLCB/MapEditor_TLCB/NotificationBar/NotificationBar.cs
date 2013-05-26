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
        SpriteFont m_font2;

        List<Notification> m_notifications;
        Queue<Notification> m_queued;

        bool m_transition;
        float m_transitionDT;

        float m_transitionTime = 0.5f;

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

        float m_randomAddTimer = 0;

        int m_showAdditionalInformation = -1;
        float m_additionalInformationHeight = 100;

        public NotificationBar(GraphicsDevice p_gd, ContentManager p_content, float p_width, float p_height)
        {
            m_width = (int)p_width;
            m_height = (int)p_height;
            m_border = 2;
            m_barTexture = new Texture2D(p_gd, m_width, m_height);
            m_transition = false;
            m_transitionDT = 0;

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
            m_queued = new Queue<Notification>();

            /*List<Paragraph> paragraphs = new List<Paragraph>();
            paragraphs.Add(new Paragraph("Information messages appear to inform you of some of the functionality of the editor."));
            paragraphs.Add(new Paragraph("Information messages' use the following symbol.", m_info, 50, 50));
            m_notifications.Add(new Notification("This is an information message.", NotificationType.INFO, paragraphs));
            m_notifications.Add(new Notification("This is a warning message", NotificationType.WARNING));
            m_notifications.Add(new Notification("This is an error message", NotificationType.ERROR));
            m_notifications.Add(new Notification("This is a success message", NotificationType.SUCCESS));*/

            m_notifications.Add(new Notification("WELCOME TO THE LITTLE CHEESE BOY EDITOR!", NotificationType.INFO));

            m_font = p_content.Load<SpriteFont>("Arial10");
            m_font2 = p_content.Load<SpriteFont>("Arial12");


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
            if (!m_transition)
            {
                m_notifications.Add(p_notification);
                m_transition = true;
                m_transitionDT = 0;

                if (m_openOnHover)
                    m_unseen++;
            }
            else
            {
                m_queued.Enqueue(p_notification);
            }
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
        public float getTotalHeight()
        {
            return m_height * m_notifications.Count;
        }

        public void update(float p_dt, Vector2 p_topLeft, int p_height, bool p_hasFocus, float p_scroll)
        {
            if (m_transition)
            {
                m_transitionDT += p_dt;
                if (m_transitionDT > m_transitionTime)
                {
                    m_transition = false;
                    m_transitionDT = 0;
                    if (m_queued.Count > 0)
                    {
                        addNotification(m_queued.Dequeue());
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
            checkCollision(p_height, p_topLeft, p_hasFocus, p_scroll);
        }
        public void checkCollision(int p_height, Vector2 p_topLeft, bool p_hasFocus, float p_scroll)
        {
            if (m_showAdditionalInformation == -1)
            {
                if (Mouse.GetState().LeftButton != ButtonState.Pressed)
                    return;

                float offset = 0;
                if (m_transition)
                {
                    offset = (m_transitionTime - m_transitionDT) / m_transitionTime;
                    offset *= m_height;
                }

                Vector2 startPos = m_position - new Vector2(0, offset);
                startPos.Y -= p_scroll;

                int count = p_height / m_height;
                if (m_transition)
                    count++;

                for (int i = 0; i < count; i++)
                {
                    Vector2 barPos = startPos + new Vector2(0, m_height * i);

                    if (m_notifications.Count - i - 1 >= 0)
                    {
                        if (m_notifications[m_notifications.Count - i - 1].additionalInformationParagraphs.Count > 0)
                        {
                            string moreInfo = "More...";
                            Vector2 textSize = m_font.MeasureString(moreInfo);

                            Rectangle rect;
                            rect.X = (int)(barPos.X + m_width - textSize.X);
                            rect.Y = (int)(barPos.Y + 0.5f * (m_height - textSize.Y));
                            rect.Width = (int)textSize.X;
                            rect.Height = (int)textSize.Y;

                            Vector2 mousePos = new Vector2(Mouse.GetState().X - p_topLeft.X, Mouse.GetState().Y - p_topLeft.Y);

                            if (rect.Contains((int)mousePos.X, (int)mousePos.Y) && p_hasFocus)
                            {
                                m_showAdditionalInformation = m_notifications.Count - i - 1;
                            }
                        }
                    }
                }
            }
        }
        public bool isShowingAdditionalInformation()
        {
            return m_showAdditionalInformation >= 0;
        }
        public float getAdditionalInformationHeight()
        {
            return m_additionalInformationHeight;
        }
        public void draw(SpriteBatch p_sb, int p_height, Vector2 p_topLeft, bool p_hasFocus, float p_scroll)
        {
            if (m_showAdditionalInformation >= 0)
                drawAdditionalInfo(p_sb, p_scroll, p_topLeft, p_hasFocus);
            else
                draw3(p_sb, p_height, p_topLeft, p_hasFocus, p_scroll);
        }
        public void draw3(SpriteBatch p_sb, int p_height, Vector2 p_topLeft, bool p_hasFocus, float p_scroll)
        {
            Color warningColor = new Color(255, 242, 184, 255);
            Color successColor = new Color(185, 255, 182, 255);
            Color errorColor = new Color(255, 202, 202, 255);
            Color infoColor = new Color(176, 224, 230, 255);

            Color warningHighlight = new Color(200, 200, 50, 255);
            Color successHighlight = new Color(100, 200, 100, 255);
            Color errorHighlight = new Color(255, 100, 100, 255);
            Color infoHighlight = new Color(100, 200, 200, 255);

            float offset = 0;
            if (m_transition)
            {
                offset = (m_transitionTime-m_transitionDT) / m_transitionTime;
                offset *= m_height;
            }

            int start = (int)Math.Max(p_scroll / m_height, 0);

            Vector2 startPos = m_position - new Vector2(0, offset + p_scroll);

            int count = p_height / m_height + 1;
            if (m_transition)
                count++;

            for (int i = start; i < start + count; i++)
            {
                Vector2 barPos = startPos + new Vector2(0, m_height * i);
                p_sb.Draw(m_barTexture, barPos, Color.White);

                if (m_notifications.Count - i - 1 >= 0)
                {
                    Notification n = m_notifications[m_notifications.Count - i - 1];
                    Texture2D icon = m_info;
                    Color color = infoColor;
                    Color highlight = infoHighlight;
                    if (n.type == NotificationType.ERROR)
                    {
                        color = errorColor;
                        icon = m_error;
                        highlight = errorHighlight;
                    }
                    else if (n.type == NotificationType.SUCCESS)
                    {
                        color = successColor;
                        icon = m_success;
                        highlight = successHighlight;
                    }
                    else if (n.type == NotificationType.WARNING)
                    {
                        color = warningColor;
                        icon = m_warning;
                        highlight = warningHighlight;
                    }

                    float grayness = 0.5f * Math.Min(n.age / m_maxAge, 1.0f);

                    Color barColor = Color.White;
                    Color moreinfoColor = Color.Black;
                    barColor.R = (byte)(color.R * (1 - grayness) + Color.White.R * grayness);
                    barColor.G = (byte)(color.G * (1 - grayness) + Color.White.G * grayness);
                    barColor.B = (byte)(color.B * (1 - grayness) + Color.White.B * grayness);
                    barColor.A = (byte)(color.A * (1 - grayness) + Color.White.A * grayness);

                    string moreInfo = "More...";
                    Vector2 textSize = m_font.MeasureString(moreInfo);

                    Rectangle rect;
                    rect.X = (int)(barPos.X + m_width - textSize.X);
                    rect.Y = (int)(barPos.Y + 0.5f * (m_height - textSize.Y));
                    rect.Width = (int)textSize.X;
                    rect.Height = (int)textSize.Y;

                    Vector2 mousePos = new Vector2(Mouse.GetState().X - p_topLeft.X, Mouse.GetState().Y - p_topLeft.Y);

                    if (rect.Contains((int)mousePos.X, (int)mousePos.Y) && p_hasFocus && n.additionalInformationParagraphs.Count > 0)
                    {
                        barColor = highlight;
                        moreinfoColor = Color.White;
                    }

                    rect.X = (int)barPos.X;
                    rect.Y = (int)barPos.Y;
                    rect.Width = m_width;
                    rect.Height = m_height;
                    p_sb.Draw(m_barTexture, barPos, barColor);

                    rect.X = (int)(barPos.X + m_border);
                    rect.Y = (int)(barPos.Y + m_border);
                    rect.Width = m_height - m_border * 2;
                    rect.Height = m_height - m_border * 2;

                    p_sb.Draw(icon, rect, barColor);

                    textSize = m_font.MeasureString(n.message);

                    Vector2 textPos = barPos + new Vector2(0, 0.5f * (m_height - textSize.Y));

                    textPos.X += m_border + m_height;

                    p_sb.DrawString(m_font, n.message, textPos, Color.Black);

                    //More info text
                    moreInfo = "More...";
                    textSize = m_font.MeasureString(moreInfo);

                    textPos = barPos + new Vector2(0, 0.5f * (m_height - textSize.Y));

                    //textPos.X += m_border + m_height;
                    textPos.X += m_width - textSize.X;

                    if (n.additionalInformationParagraphs.Count > 0)
                        p_sb.DrawString(m_font, moreInfo, textPos, moreinfoColor);


                }
            }
        }
        public void drawAdditionalInfo(SpriteBatch p_sb, float p_scroll, Vector2 p_topLeft, bool p_hasFocus)
        {
            List<Paragraph> paragraphs = m_notifications[m_showAdditionalInformation].additionalInformationParagraphs;
            float y = -p_scroll;

            Vector2 textSize = m_font.MeasureString("Go Back");
            Color backColor = Color.ForestGreen;
            Rectangle backRect;
            backRect.X = (int)m_position.X;
            backRect.Y = (int)(m_position.Y + y);
            backRect.Width = (int)(textSize.X);
            backRect.Height = (int)(textSize.Y);

            Vector2 mousePos = new Vector2(Mouse.GetState().X - p_topLeft.X, Mouse.GetState().Y - p_topLeft.Y);

            if (backRect.Contains((int)mousePos.X, (int)mousePos.Y) && p_hasFocus)
            {
                backColor = Color.White;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    m_showAdditionalInformation = -1;
                    return;
                }
            }
            p_sb.DrawString(m_font, "Go Back", m_position + new Vector2(0, y), backColor);
            y += textSize.Y*1.5f;

            textSize = m_font2.MeasureString(m_notifications[m_showAdditionalInformation].heading);
            p_sb.DrawString(m_font2, m_notifications[m_showAdditionalInformation].heading, m_position + new Vector2(0, y), Color.White);
            y += textSize.Y*1.1f;

            for (int i = 0; i < paragraphs.Count; i++)
            {
                string text = (i+1).ToString() + ": " + paragraphs[i].text;

                List<string> rows = makeIntoRows(text);

                for (int row = 0; row < rows.Count; row++)
                {
                    p_sb.DrawString(m_font, rows[row], m_position + new Vector2(0, y), Color.White);
                    y += m_font.MeasureString(rows[row]).Y;
                }
                if (paragraphs[i].texture != null)
                {
                    Texture2D tex = paragraphs[i].texture;

                    float scale = Math.Min(paragraphs[i].maxX / tex.Width, paragraphs[i].maxY / tex.Height);

                    Rectangle target;
                    target.X = (int)m_position.X;
                    target.Y = (int)(m_position.Y + y);
                    target.Width = (int)(scale * tex.Width);
                    target.Height = (int)(scale * tex.Height);

                    p_sb.Draw(tex, target, Color.White);

                    y += scale * tex.Height;
                }

                y += m_font.MeasureString(rows[rows.Count - 1]).Y;
            }

            backRect.X = (int)m_position.X;
            backRect.Y = (int)(m_position.Y + y);
            backRect.Width = (int)(textSize.X);
            backRect.Height = (int)(textSize.Y);

            if (backRect.Contains((int)mousePos.X, (int)mousePos.Y) && p_hasFocus)
            {
                backColor = Color.White;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    m_showAdditionalInformation = -1;
            }
            p_sb.DrawString(m_font, "Go Back", m_position + new Vector2(0, y), backColor);
            y += textSize.Y;

            m_additionalInformationHeight = y+p_scroll;
        }
        public List<string> makeIntoRows(string p_text)
        {
            List<string> rows = new List<string>();

            int start = 0;
            int stableSpace = 0;
            for (int i = 0; i < p_text.Length; i++)
            {
                if (p_text[i] == ' ')
                {
                    Vector2 size = m_font.MeasureString(p_text.Substring(start, i - start));
                    if (size.X > m_width*0.9f)
                    {
                        rows.Add(p_text.Substring(start, stableSpace - start));
                        start = i = stableSpace + 1;
                    }
                    else
                    {
                        stableSpace = i;
                    }

                }
            }
            rows.Add(p_text.Substring(start, p_text.Length - start));

            return rows;
        }
    }
}
