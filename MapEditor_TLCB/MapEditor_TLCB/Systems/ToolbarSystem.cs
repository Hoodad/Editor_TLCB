using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB.Systems
{
	class ToolbarSystem : EntitySystem
	{
		Manager manager;
		Window toolbarWindow;
		Button roadTool;
		Button eraserTool;
		Button paintTool;

		Button clearMap;
		Button saveMap;
		Button exportMap;
		Button backToStartScreen;
		Button exitButton;

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
			toolbarWindow.Width = 100;
			toolbarWindow.Height = (int)((float)viewport.Height * 0.3f);
			toolbarWindow.Top = 110;
			toolbarWindow.Left = viewport.Width - toolbarWindow.Width;
			//toolbarWindow.Visible = true;
			toolbarWindow.Resizable = false;
			toolbarWindow.CloseButtonVisible = false;
			toolbarWindow.BorderVisible = false;
            toolbarWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			//toolbar.Movable = false;
			manager.Add(toolbarWindow);

			roadTool = new Button(manager);
			roadTool.Init();
			roadTool.Parent = toolbarWindow;
			roadTool.Width = toolbarWindow.Width / 2;
			roadTool.Height = 24;
			roadTool.Left = 0;
			roadTool.Top = 0;
			roadTool.Text = "Road";
            roadTool.Click += new TomShane.Neoforce.Controls.EventHandler(RoadToolBehavior);

			eraserTool = new Button(manager);
			eraserTool.Init();
			eraserTool.Parent = toolbarWindow;
			eraserTool.Width = toolbarWindow.Width / 2;
			eraserTool.Height = 24;
			eraserTool.Left = 50;
			eraserTool.Top = 0;
			eraserTool.Text = "Erase";
            eraserTool.Click += new TomShane.Neoforce.Controls.EventHandler(EraseToolBehavior);

			paintTool = new Button(manager);
			paintTool.Init();
			paintTool.Parent = toolbarWindow;
			paintTool.Width = toolbarWindow.Width / 2;
			paintTool.Height = 24;
			paintTool.Left = 0;
			paintTool.Top = 24;
			paintTool.Text = "Paint";
            paintTool.Click += new TomShane.Neoforce.Controls.EventHandler(PaintToolBehavior);

			exportMap = new Button(manager);
			exportMap.Init();
			exportMap.Parent = toolbarWindow;
			exportMap.Text = "Export Map";
			exportMap.Width = toolbarWindow.Width;
			exportMap.Height = 24;
			exportMap.Left = 0;
			exportMap.Top = toolbarWindow.Height - 24 * 5;

			saveMap = new Button(manager);
			saveMap.Init();
			saveMap.Parent = toolbarWindow;
			saveMap.Text = "Save Map";
			saveMap.Width = toolbarWindow.Width;
			saveMap.Height = 24;
			saveMap.Left = 0;
			saveMap.Top = toolbarWindow.Height - 24 * 4;

			clearMap = new Button(manager);
			clearMap.Init();
			clearMap.Parent = toolbarWindow;
			clearMap.Text = "Clear Map";
			clearMap.Width = toolbarWindow.Width;
			clearMap.Height = 24;
			clearMap.Left = 0;
			clearMap.Top = toolbarWindow.Height - 24 * 3;
			clearMap.Click += new TomShane.Neoforce.Controls.EventHandler(ClearMapBehavior);

			backToStartScreen = new Button(manager);
			backToStartScreen.Init();
			backToStartScreen.Parent = toolbarWindow;
			backToStartScreen.Text = "To Start Screen";
			backToStartScreen.Width = toolbarWindow.Width;
			backToStartScreen.Height = 24;
			backToStartScreen.Left = 0;
			backToStartScreen.Top = toolbarWindow.Height - 24 * 2;
			backToStartScreen.Click += new TomShane.Neoforce.Controls.EventHandler(BackToStartScreenBehavior);
			
			exitButton = new Button(manager);
			exitButton.Init();
			exitButton.Parent = toolbarWindow;
			exitButton.Text = "Exit";
			exitButton.Width = toolbarWindow.Width;
			exitButton.Height = 25;
			exitButton.Left = 0;
			exitButton.Top = toolbarWindow.Height - exitButton.Height;
			exitButton.Click += new TomShane.Neoforce.Controls.EventHandler(ExitBehavior);

		}

		public override void Process()
		{
		}

		public void ExitBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			((StateSystem)world.SystemManager.GetSystem<StateSystem>()[0]).RequestToShutdown();
		}
        public void PaintToolBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.PAINT_TOOL);
        }
        public void RoadToolBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.ROAD_TOOL);
        }
        public void EraseToolBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.ERASE_TOOL);
        }
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
            Notification n = new Notification("The toolbar enables you to select draw tools and handle the project.", NotificationType.INFO);
            noteSys.AddNotification(n);
        }
		public void BackToStartScreenBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			((StartupDialogSystem)world.SystemManager.GetSystem<StartupDialogSystem>()[0]).ShowStartUpDialog();
		}
		public void ClearMapBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			Entity singleTilemap = world.TagManager.GetEntity("singlesTilemap");
			Entity roadTilemap = world.TagManager.GetEntity("roadTilemap");

			((Tilemap)roadTilemap.GetComponent<Tilemap>()).clear();
			((Tilemap)singleTilemap.GetComponent<Tilemap>()).clear();
		}
	}
}
