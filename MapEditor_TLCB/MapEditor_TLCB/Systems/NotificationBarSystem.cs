using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Artemis;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor_TLCB.Systems
{
	class NotificationBarSystem: EntitySystem
	{
		Manager manager;
		Window notificationWindow;

		public NotificationBarSystem(Manager p_manager)
		{
			manager = p_manager;
		}
		public override void Initialize()
		{
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();
			
			//Notification Bar
			notificationWindow = new Window(manager);
			notificationWindow.Init();
			notificationWindow.Text = "Notification Bar";
			notificationWindow.Height = 200;
			notificationWindow.Width = (int)((float)viewport.Width * 0.3f);
			notificationWindow.Visible = true;
			notificationWindow.Top = viewport.Height - notificationWindow.Height;
			notificationWindow.Left = 0;
			notificationWindow.CloseButtonVisible = false;
			//notificationBar.Movable = false;
			manager.Add(notificationWindow);
		}
		public override void Process()
		{
		}
	}
}
