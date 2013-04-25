using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor_TLCB
{
    enum NotificationType
    {
        INFO, WARNING, SUCCESS, ERROR
    }
    class Notification
    {
        public string message;
        public NotificationType type;
        public float age;
        public Notification(string p_message, NotificationType p_type)
        {
            message = p_message;
            type = p_type;
            age = 0.0f;
        }
    }
}
