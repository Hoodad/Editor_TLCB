using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor_TLCB
{
    enum NotificationType
    {
        INFO, WARNING, SUCCESS, ERROR
    }

    struct Paragraph
    {
        public string text;
        public Texture2D texture;
        public float maxX;
        public float maxY;

        public Paragraph(string p_text)
        {
            text = p_text;
            texture = null;
            maxX = maxY = 0;
        }
        public Paragraph(string p_text, Texture2D p_texture, float p_maxX, float p_maxY)
        {
            text = p_text;
            texture = p_texture;
            maxX = p_maxX;
            maxY = p_maxY;
        }
    }

    class Notification
    {
        public string message;
        public NotificationType type;
        public float age;

        public List<Paragraph> additionalInformationParagraphs;

        public Notification(string p_message, NotificationType p_type)
        {
            message = p_message;
            type = p_type;
            age = 0.0f;
            additionalInformationParagraphs = new List<Paragraph>();
        }
        public Notification(string p_message, NotificationType p_type, List<Paragraph> p_additionalInformationParagraphs)
        {
            message = p_message;
            type = p_type;
            age = 0.0f;
            additionalInformationParagraphs = p_additionalInformationParagraphs;
        }
    }
}
