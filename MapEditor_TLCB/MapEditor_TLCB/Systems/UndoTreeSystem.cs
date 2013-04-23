using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Artemis;

namespace MapEditor_TLCB.Systems
{
	class UndoTreeSystem : EntitySystem
	{
		Manager manager;
		Window undoTreeWindow;

		public UndoTreeSystem(Manager p_manager)
		{
			manager = p_manager;
		}

		public override void Initialize()
		{
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();

			undoTreeWindow = new Window(manager);
			undoTreeWindow.Init();
			undoTreeWindow.Text = "Undo Tree";
			undoTreeWindow.Width = 200;
			undoTreeWindow.Height = (int)((float)viewport.Height * 0.6f);
			undoTreeWindow.Top = 0;
			undoTreeWindow.Left = 0;
			undoTreeWindow.Visible = true;
			undoTreeWindow.CloseButtonVisible = false;
			//toolbarWindow.BorderVisible = false;
			//toolbar.Movable = false;
			manager.Add(undoTreeWindow);
		}

		public override void Process()
		{
		}
	}
}
