using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor_TLCB.Systems
{
	class TilemapBarSystem : EntitySystem
	{
		Manager manager;
		Window tilemapWindow;

		public TilemapBarSystem(Manager p_manager)
		{
			manager = p_manager;
		}

		public override void Initialize()
		{
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();

			tilemapWindow = new Window(manager);
			tilemapWindow.Init();
			tilemapWindow.Text = "Tilemap";
			tilemapWindow.Height = 200;
			tilemapWindow.Width = (int)((float)viewport.Width * 0.3f);
			tilemapWindow.Visible = true;
			tilemapWindow.Top = viewport.Height - tilemapWindow.Height;
			tilemapWindow.Left = (int)((float)viewport.Width * 0.7f);
			tilemapWindow.CloseButtonVisible = false;
			//tileMap.Movable = false;
			manager.Add(tilemapWindow);
		}

		public override void Process()
		{
		}
	}
}
