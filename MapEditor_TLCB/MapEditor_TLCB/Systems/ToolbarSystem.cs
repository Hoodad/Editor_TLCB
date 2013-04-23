using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor_TLCB.Systems
{
	class ToolbarSystem : EntitySystem
	{
		Manager manager;
		Window toolbarWindow;

		public ToolbarSystem(Manager p_manager)
		{
			manager = p_manager;
		}

		public override void Initialize()
		{
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();

			toolbarWindow = new Window(manager);
			toolbarWindow.Init();
			toolbarWindow.Text = "Toolbar";
			toolbarWindow.Width = 200;
			toolbarWindow.Height = (int)((float)viewport.Height * 0.6f);
			toolbarWindow.Top = 0;
			toolbarWindow.Left = viewport.Width - toolbarWindow.Width;
			toolbarWindow.Visible = true;
			toolbarWindow.Resizable = false;
			toolbarWindow.CloseButtonVisible = false;
			toolbarWindow.BorderVisible = false;
			//toolbar.Movable = false;
			manager.Add(toolbarWindow);
		}

		public override void Process()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
