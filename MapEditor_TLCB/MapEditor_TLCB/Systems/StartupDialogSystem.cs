using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor_TLCB.Systems
{
	class StartupDialogSystem : EntitySystem
	{
		private Manager manager;
		private Window startupDialog;
		private GroupPanel recentMaps;
		private GroupPanel possibleMaps;
		private Button tileMapGarden;
		private Button tileMapCellar;

		public StartupDialogSystem(Manager p_manager)
		{
			manager = p_manager;
		}

		public override void Initialize()
		{
			int xOffset = 50;
			int yOffset = 0;
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();

			startupDialog = new Window(manager);
			startupDialog.Init();
			startupDialog.Width = 400;
			startupDialog.Height = 350;
			startupDialog.Center();
			startupDialog.Text = "What would you like to do.";
			startupDialog.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			startupDialog.ShowModal();
			startupDialog.CloseButtonVisible = false;
			manager.Add(startupDialog);

			possibleMaps = new GroupPanel(manager);
			possibleMaps.Init();
			possibleMaps.Parent = startupDialog;
			possibleMaps.Height = startupDialog.Height - 38;
			possibleMaps.Width = startupDialog.Width / 2;
			possibleMaps.Text = "Start a new Map?";
			//possibleMaps.Anchor = Anchors.Left | Anchors.Top;
			possibleMaps.Top = yOffset;
			possibleMaps.Left =  0;

			recentMaps = new GroupPanel(manager);
			recentMaps.Init();
			recentMaps.Parent = startupDialog;
			recentMaps.Width = startupDialog.Width / 2;
			recentMaps.Height = startupDialog.Height - 38;
			recentMaps.Text = "Load a recent Map?";
			//recentMaps.Anchor = Anchors.Left | Anchors.Top;
			recentMaps.Top = yOffset;
			recentMaps.Left = 200;

			int buttonSize =64;
	
			tileMapGarden = new Button(manager);
			tileMapGarden.Init();
			tileMapGarden.Width = buttonSize;
			tileMapGarden.Height = buttonSize;
			tileMapGarden.Top = buttonSize * 0;
			tileMapGarden.Left = possibleMaps.Width / 2 - buttonSize / 2;
			tileMapGarden.Parent = possibleMaps;
			//manager.Add(tileMapGarden);

			tileMapCellar = new Button(manager);
			tileMapCellar.Init();
			tileMapCellar.Width = buttonSize;
			tileMapCellar.Height = buttonSize;
			tileMapCellar.Top = buttonSize * 1 + 8;
			tileMapCellar.Left = possibleMaps.Width / 2 - buttonSize / 2;
			tileMapCellar.Parent = possibleMaps;
			//manager.Add(tileMapCellar);


		}

		public override void Process()
		{
		}
		public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			//startupDialog.Close();
		}
	}
}
