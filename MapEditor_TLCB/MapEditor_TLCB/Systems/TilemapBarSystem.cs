using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MapEditor_TLCB.CustomControls;

namespace MapEditor_TLCB.Systems
{
	class TilemapBarSystem : EntitySystem
	{
		Manager manager;
		Window tilemapWindow;

		TilemapContainer tilemap;

		public TilemapBarSystem(Manager p_manager)
		{
			manager = p_manager;
		}

		public override void Initialize()
		{
			ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();

			tilemapWindow = new Window(manager);
			tilemapWindow.Init();
			tilemapWindow.Text = "Tilemap";
			tilemapWindow.Height = 224;
			tilemapWindow.Width = 576;
			tilemapWindow.Visible = true;
			tilemapWindow.Top = viewport.Height - tilemapWindow.Height;
			tilemapWindow.Left = (int)((float)viewport.Width -tilemapWindow.Width);
			tilemapWindow.CloseButtonVisible = false;
			tilemapWindow.MaximumHeight = 960 + 54;
			tilemapWindow.MaximumWidth = 960;
            tilemapWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			//tileMap.Movable = false;
			manager.Add(tilemapWindow);

			tilemap = new TilemapContainer(manager);
			tilemap.gridImage = contentSystem.LoadTexture("TileSheets/grid");
			tilemap.tilemapImage = contentSystem.LoadTexture("TileSheets/tilemap_garden");
			tilemap.tileSelectorImage = contentSystem.LoadTexture("TileSelector_v3");
			tilemap.Parent = tilemapWindow;
			tilemap.windowParent = tilemapWindow;
			tilemap.Width = tilemap.tilemapImage.Width;
			tilemap.Height = tilemap.tilemapImage.Height;
			tilemap.Parent = tilemapWindow;
			tilemap.gridColor = Color.Black;
			tilemap.CanFocus = false;
			tilemap.Click += new TomShane.Neoforce.Controls.EventHandler(OnClick);
			tilemap.Init(contentSystem.GetViewportSize());

		}

		public override void Process()
		{
		}
		public void OnClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.PAINT_TOOL);
		}
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
            Notification n = new Notification("Select a tile from the tilemap to start drawing.", NotificationType.INFO);
            noteSys.AddNotification(n);
        }
        public TilemapContainer GetTilemapContainer()
        {
            return tilemap;
        }
	}
}
