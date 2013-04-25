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
		Button roadTool;
		Button eraserTool;
		Button paintTool;

		Button clearMap;
		Button saveMap;
		Button exportMap;
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

			eraserTool = new Button(manager);
			eraserTool.Init();
			eraserTool.Parent = toolbarWindow;
			eraserTool.Width = toolbarWindow.Width / 2;
			eraserTool.Height = 24;
			eraserTool.Left = 50;
			eraserTool.Top = 0;
			eraserTool.Text = "Erase";

			paintTool = new Button(manager);
			paintTool.Init();
			paintTool.Parent = toolbarWindow;
			paintTool.Width = toolbarWindow.Width / 2;
			paintTool.Height = 24;
			paintTool.Left = 0;
			paintTool.Top = 24;
			paintTool.Text = "Paint";

			exportMap = new Button(manager);
			exportMap.Init();
			exportMap.Parent = toolbarWindow;
			exportMap.Text = "Export Map";
			exportMap.Width = toolbarWindow.Width;
			exportMap.Height = 24;
			exportMap.Left = 0;
			exportMap.Top = toolbarWindow.Height - 24 * 4;

			saveMap = new Button(manager);
			saveMap.Init();
			saveMap.Parent = toolbarWindow;
			saveMap.Text = "Save Map";
			saveMap.Width = toolbarWindow.Width;
			saveMap.Height = 24;
			saveMap.Left = 0;
			saveMap.Top = toolbarWindow.Height - 24 * 3;

			clearMap = new Button(manager);
			clearMap.Init();
			clearMap.Parent = toolbarWindow;
			clearMap.Text = "Clear Map";
			clearMap.Width = toolbarWindow.Width;
			clearMap.Height = 24;
			clearMap.Left = 0;
			clearMap.Top = toolbarWindow.Height - 24 * 2;
			
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
	}
}
