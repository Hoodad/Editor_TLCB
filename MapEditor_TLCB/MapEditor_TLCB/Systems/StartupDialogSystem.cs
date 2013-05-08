using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.CustomControls;

namespace MapEditor_TLCB.Systems
{
	class StartupDialogSystem : EntitySystem
	{
		private Manager manager;
		private Window startupDialog;
		private GroupPanel recentMaps;
		private GroupPanel possibleMaps;
		private ImageBasedButton tileMapGarden;
		private ImageBasedButton tileMapCellar;

		public Texture2D tilemap;
		private bool hasChangedTilemap;
		private bool requestToChangeTilemap;

		public StartupDialogSystem(Manager p_manager)
		{
			manager = p_manager;
			hasChangedTilemap = false;
			requestToChangeTilemap = false;
		}
		public override void Process()
		{
			if (requestToChangeTilemap)
			{
				hasChangedTilemap = true;
				requestToChangeTilemap = false;
			}
			else
			{
				hasChangedTilemap = false;
				requestToChangeTilemap = false;
			}
		}
		
		public void OnTilemapButtonClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			startupDialog.Close();

			ImageBasedButton btn = (ImageBasedButton)sender;
			tilemap = btn.tilemap;
			requestToChangeTilemap = true;
		}
		public void OnTilemapButtonMouseOverBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			ImageBasedButton btn = (ImageBasedButton)sender;
			tilemap = btn.tilemap;
			requestToChangeTilemap = true;
		}
		public void ShowStartUpDialog()
		{
			startupDialog.ShowModal();
		}
		public bool IsVisible()
		{
			return startupDialog.Visible;
		}
		public bool HasChangedTileMap()
		{
			return hasChangedTilemap;
		}
		public override void Initialize()
		{
			int yOffset = 0;
			int xOffset = 50;

			ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();

			startupDialog = new Window(manager);
			startupDialog.Init();
			startupDialog.Width = 400;
			startupDialog.Height = 158;
			startupDialog.Center();
			startupDialog.Text = "What would you like to do?";
			startupDialog.ShowModal();
			startupDialog.CloseButtonVisible = false;
			startupDialog.Resizable = false;
			startupDialog.Movable = false;
			startupDialog.IconVisible = false;
			manager.Add(startupDialog);

			possibleMaps = new GroupPanel(manager);
			possibleMaps.Init();
			possibleMaps.Parent = startupDialog;
			possibleMaps.Height = 104;
			possibleMaps.Width = 158;
			possibleMaps.Text = "Start a new Map";
			//possibleMaps.Anchor = Anchors.Left | Anchors.Top;
			possibleMaps.Top = yOffset;
			possibleMaps.Left = 0;

			recentMaps = new GroupPanel(manager);
			recentMaps.Init();
			recentMaps.Parent = startupDialog;
			recentMaps.Width = startupDialog.Width / 2;
			recentMaps.Height = startupDialog.Height - 38;
			recentMaps.Text = "Load a recent Map";
			//recentMaps.Anchor = Anchors.Left | Anchors.Top;
			recentMaps.Top = yOffset;
			recentMaps.Left = possibleMaps.Width + 8;

			int buttonSize = 64;

			tileMapGarden = new ImageBasedButton(manager);
			tileMapGarden.Init();
			tileMapGarden.Width = buttonSize;
			tileMapGarden.Height = buttonSize;
			tileMapGarden.Top = buttonSize * 0 + 8;
			tileMapGarden.Left = buttonSize * 0 + 8 * 1;
			tileMapGarden.Parent = possibleMaps;
			tileMapGarden.tilemap = contentSystem.LoadTexture("TileSheets/tilemap_garden");
			tileMapGarden.tilemap.Name = "Tilemap_garden";
			tileMapGarden.Click += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonClickBehavior);
			tileMapGarden.MouseOver += new MouseEventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapGarden.FocusGained += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapGarden.Text = "";
			tileMapGarden.GenerateFirstTile(contentSystem);
			tileMapGarden.Focused = true;
			//manager.Add(tileMapGarden);

			tileMapCellar = new ImageBasedButton(manager);
			tileMapCellar.Init();
			tileMapCellar.Width = buttonSize;
			tileMapCellar.Height = buttonSize;
			tileMapCellar.Top = buttonSize * 0 + 8;
			tileMapCellar.Left = buttonSize * 1 + 8 * 2;
			tileMapCellar.Parent = possibleMaps;
			tileMapCellar.tilemap = contentSystem.LoadTexture("TileSheets/tilemap_winecellar");
			tileMapGarden.tilemap.Name = "Tilemap_winecellar";
			tileMapCellar.Click += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonClickBehavior);
			tileMapCellar.MouseOver += new MouseEventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapCellar.FocusGained += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapCellar.Text = "";

			tileMapCellar.GenerateFirstTile(contentSystem);
			//manager.Add(tileMapCellar);

			tilemap = tileMapGarden.tilemap;
		}

	}
}
