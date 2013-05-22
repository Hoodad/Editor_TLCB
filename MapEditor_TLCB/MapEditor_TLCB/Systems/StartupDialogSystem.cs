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
		private HalfTransparentOverlay overlay;
		private Window startupDialog;
		private GroupPanel recentMaps;
		private GroupPanel possibleMaps;
		private ImageBasedButton tileMapGarden;
		private ImageBasedButton tileMapCellar;
		private Button openMap;

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

                RadialMenuSystem rms = (RadialMenuSystem)world.SystemManager.GetSystem<RadialMenuSystem>()[0];
                rms.setTilemap(tilemap);
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
			StateSystem stateSys = (StateSystem)world.SystemManager.GetSystem<StateSystem>()[0];
			stateSys.SetCanvasCanBeReached(false);
		}
		public bool IsVisible()
		{
			return startupDialog.Visible;
		}
		public bool HasChangedTileMap()
		{
			return hasChangedTilemap;
		}
		void startupDialog_VisibleChanged(object p_sender, TomShane.Neoforce.Controls.EventArgs p_args)
		{
			overlay.Visible = startupDialog.Visible;
			if (startupDialog.Visible == true)
			{
				overlay.BringToFront();
				startupDialog.BringToFront();
			}
		}

		void LoadMapFromFile(object p_sender, TomShane.Neoforce.Controls.EventArgs p_args)
		{
			Button btn = (Button)p_sender;
			btn.Focused = false;
			System.Windows.Forms.OpenFileDialog exportMapDialog = new System.Windows.Forms.OpenFileDialog();
			exportMapDialog.InitialDirectory = Convert.ToString(Environment.SpecialFolder.CommonProgramFilesX86);
			exportMapDialog.Filter = "Save files (*.cheeseboy)|*.cheeseboy";
			exportMapDialog.FilterIndex = 1;
			exportMapDialog.Title = "Load your saved map";
			exportMapDialog.FileOk += new System.ComponentModel.CancelEventHandler(SuccessfullySelectedSaveFile);
			exportMapDialog.ShowDialog();
		}

		void SuccessfullySelectedSaveFile(object p_sender, System.EventArgs e)
		{
			startupDialog.Close();
			System.Windows.Forms.OpenFileDialog dialog = (System.Windows.Forms.OpenFileDialog)p_sender;
			((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]).LoadSerialiazedActions(dialog.FileName);
		}

		public override void Initialize()
		{
			ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();

			overlay = new HalfTransparentOverlay(manager);
			overlay.Init();
			overlay.Width = manager.ScreenWidth;
			overlay.Height = manager.ScreenHeight;
			overlay.BackGroundTexture = contentSystem.LoadTexture("white_dot");
			manager.Add(overlay);

			startupDialog = new Window(manager);
			startupDialog.Init();
			startupDialog.ShowModal();
			startupDialog.Width = 400;
			startupDialog.Height = 158;
			startupDialog.Center();
			startupDialog.Text = "Select what you would like to do...";
			startupDialog.CloseButtonVisible = false;
			startupDialog.Resizable = false;
			startupDialog.Movable = false;
			startupDialog.IconVisible = false;
			startupDialog.VisibleChanged +=new TomShane.Neoforce.Controls.EventHandler(startupDialog_VisibleChanged);
			startupDialog.Closing += new WindowClosingEventHandler(WindowCloseBehavior);
			manager.Add(startupDialog);

			//LEFT PANEL
			possibleMaps = new GroupPanel(manager);
			possibleMaps.Init();
			possibleMaps.Parent = startupDialog;
			possibleMaps.Width = 189;
			possibleMaps.Height = 122;
			possibleMaps.Text = "Start a new Map?";
			possibleMaps.Top = 1;

			int buttonSize = 80;

			tileMapGarden = new ImageBasedButton(manager);
			tileMapGarden.Init();
			tileMapGarden.Parent = possibleMaps;
			tileMapGarden.Width = buttonSize;
			tileMapGarden.Height = buttonSize;
			tileMapGarden.Top = buttonSize * 0 + 8;
			tileMapGarden.Left = buttonSize * 0 + 8 * 1;
			tileMapGarden.tilemap = contentSystem.LoadTexture("TileSheets/tilemap_garden");
			tileMapGarden.tilemap.Name = "Tilemap_garden";
			tileMapGarden.Click += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonClickBehavior);
			tileMapGarden.MouseOver += new MouseEventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapGarden.FocusGained += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapGarden.GenerateFirstTile(contentSystem);
			tileMapGarden.Focused = true;

			tileMapCellar = new ImageBasedButton(manager);
			tileMapCellar.Init();
			tileMapCellar.Parent = possibleMaps;
			tileMapCellar.Width = buttonSize;
			tileMapCellar.Height = buttonSize;
			tileMapCellar.Top = buttonSize * 0 + 8;
			tileMapCellar.Left = buttonSize * 1 + 8 * 2;
			tileMapCellar.tilemap = contentSystem.LoadTexture("TileSheets/tilemap_winecellar");
			tileMapCellar.tilemap.Name = "Tilemap_winecellar";
			tileMapCellar.Click += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonClickBehavior);
			tileMapCellar.MouseOver += new MouseEventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapCellar.FocusGained += new TomShane.Neoforce.Controls.EventHandler(OnTilemapButtonMouseOverBehavior);
			tileMapCellar.GenerateFirstTile(contentSystem);

			tilemap = tileMapGarden.tilemap;

			//RIGHT PANEL
			recentMaps = new GroupPanel(manager);
			recentMaps.Init();
			recentMaps.Parent = startupDialog;
			recentMaps.Width = possibleMaps.Width;
			recentMaps.Height = possibleMaps.Height;
			recentMaps.Text = "Load a recent Map?";
			recentMaps.Left = possibleMaps.Width + 8;
			recentMaps.Top = 1;

			openMap = new Button(manager);
			openMap.Init();
			openMap.Parent = recentMaps;
			openMap.Width = 150;
			openMap.Height = 24;
			openMap.Top = recentMaps.Height/2 - openMap.Height;
			openMap.Left = recentMaps.Width/2 - openMap.Width/2;
			openMap.Text = "Load Saved Map";
			openMap.Click += new TomShane.Neoforce.Controls.EventHandler(LoadMapFromFile);
		}
		void WindowCloseBehavior(object sender, WindowClosingEventArgs e)
		{
			StateSystem stateSys = (StateSystem)world.SystemManager.GetSystem<StateSystem>()[0];
			stateSys.SetCanvasCanBeReached(true);
		}

	}
}
