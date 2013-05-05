using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MapEditor_TLCB.CustomControls;

namespace MapEditor_TLCB.Systems
{
	class NotificationBarSystem: EntitySystem
	{
		Manager manager;
		Window notificationWindow;

        NotificationBarContainer notificationBar;

        GraphicsDevice m_device;
        ContentManager m_content;

        int originalWidth;

		public NotificationBarSystem(Manager p_manager, GraphicsDevice p_device, ContentManager p_content)
		{
			manager = p_manager;
            m_device = p_device;
            m_content = p_content;
		}
		public override void Initialize()
		{
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();

            originalWidth = (int)((float)viewport.Width * 0.4f);

			//Notification Bar
			notificationWindow = new Window(manager);
			notificationWindow.Init();
			notificationWindow.Text = "Notification Bar";
			notificationWindow.Height = 160;
            notificationWindow.Width = originalWidth;
			notificationWindow.Visible = true;
			notificationWindow.Top = viewport.Height - notificationWindow.Height;
			notificationWindow.Left = 0;
			notificationWindow.CloseButtonVisible = false;
            notificationWindow.AutoScroll = false;
            notificationWindow.Resizable = true;
            notificationWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			notificationWindow.IconVisible = false;
			manager.Add(notificationWindow);

            NotificationBar data = new NotificationBar(m_device, m_content, originalWidth, 25);

            notificationBar = new NotificationBarContainer(manager, notificationWindow, data);
            notificationBar.Init();
            notificationBar.Width = originalWidth;// tilemap.tilemapImage.Width;
            notificationBar.Height = 160;// tilemap.tilemapImage.Height;
            notificationBar.Parent = notificationWindow;
            notificationBar.CanFocus = false;
		}
		public override void Process()
		{
            notificationBar.Update(World.Delta / 1000.0f);
   
            
            notificationBar.Width = notificationWindow.Width;
            notificationBar.Height = notificationWindow.Height;
            if (notificationWindow.Width != originalWidth)
                notificationWindow.Width = originalWidth;
            notificationBar.Height = notificationWindow.Height;

            notificationWindow.Refresh();
		}
        public void AddNotification(Notification p_notification)
        {
            notificationBar.AddNotification(p_notification);
        }
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Notification n = new Notification("The notification bar shows important information, warnings and errors.", NotificationType.INFO);
            AddNotification(n);
        }
	}
}

///OSTBIT I MITTEN?
