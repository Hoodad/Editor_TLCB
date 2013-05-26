using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.Components;
using MapEditor_TLCB.CustomControls;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class ToolbarSystem : EntitySystem
	{
		Manager manager;
		Window toolbarWindow;
		ImageBasedButton roadTool;
		ImageBasedButton eraserTool;
		ImageBasedButton paintTool;

		Button newMap;
		Button saveMap;
		Button exportMap;
		Button backToStartScreen;
		Button exitButton;
		Container validationInfo;
		public CheckBox pathsValid;
		public CheckBox playerValid;
		public CheckBox switchesValid;

		Window newMapConfirmationWindow;
		Button accept;
		Button cancel;

		public ToolbarSystem(Manager p_manager)
		{
			manager = p_manager;
		}

		public override void Initialize()
		{
			ContentSystem sys = (ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0];
			Viewport viewport = sys.GetViewport();

			toolbarWindow = new Window(manager);
			toolbarWindow.Init();
			toolbarWindow.Text = "Toolbar";
			toolbarWindow.Width = 100;
			toolbarWindow.Height = (int)((float)viewport.Height * 0.35f);
			toolbarWindow.Top = 10;
			toolbarWindow.Left = viewport.Width - toolbarWindow.Width;
			toolbarWindow.Resizable = false;
			toolbarWindow.CloseButtonVisible = false;
			toolbarWindow.BorderVisible = false;
            toolbarWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			toolbarWindow.Movable = false;
			manager.Add(toolbarWindow);

			roadTool = new ImageBasedButton(manager);
			roadTool.Init();
			roadTool.Parent = toolbarWindow;
			roadTool.Width = toolbarWindow.Width / 2;
			roadTool.Height = roadTool.Width;
			roadTool.Left = 0;
			roadTool.Top = 0;
			roadTool.Text = "";
			roadTool.image = sys.LoadTexture("RoadIcon");
            roadTool.Click += new TomShane.Neoforce.Controls.EventHandler(RoadToolBehavior);
			roadTool.Pushed = true;
			roadTool.Mode = ButtonMode.PushButton;

			eraserTool = new ImageBasedButton(manager);
			eraserTool.Init();
			eraserTool.Parent = toolbarWindow;
			eraserTool.Width = toolbarWindow.Width / 2;
			eraserTool.Height = eraserTool.Width;
			eraserTool.Left = 50;
			eraserTool.Top = 0;
			eraserTool.Text = "";
			eraserTool.image = sys.LoadTexture("EraserIcon");
            eraserTool.Click += new TomShane.Neoforce.Controls.EventHandler(EraseToolBehavior);

			paintTool = new ImageBasedButton(manager);
			paintTool.Init();
			paintTool.Parent = toolbarWindow;
			paintTool.Width = toolbarWindow.Width / 2;
			paintTool.Height = paintTool.Width;
			paintTool.Left = 0;
			paintTool.Top = roadTool.Height;
			paintTool.Text = "";
			paintTool.image = sys.LoadTexture("PaintingIcon");
            paintTool.Click += new TomShane.Neoforce.Controls.EventHandler(PaintToolBehavior);

			//
			int top = toolbarWindow.Height - 24 * 6 + 10;
			pathsValid = new CheckBox(manager);
			pathsValid.Init();
			pathsValid.Parent = toolbarWindow;
			pathsValid.Text = "";
			pathsValid.Left = 1;
			pathsValid.Top = top;
			pathsValid.Width = 15;
			pathsValid.Enabled = false;
			pathsValid.TextColor = Color.Red;
			pathsValid.ToolTip = new CustomToolTip(manager);
			pathsValid.ToolTip.Parent = pathsValid;
			pathsValid.ToolTip.Init();
			pathsValid.ToolTip.Text = "0";
			pathsValid.ToolTip.TextColor = Color.White;
			pathsValid.ToolTip.Visible = false;
			pathsValid.ToolTip.Color = Color.Red;

			playerValid = new CheckBox(manager);
			playerValid.Init();
			playerValid.Parent = toolbarWindow;
			playerValid.Text = "";
			playerValid.Left = 16;
			playerValid.Top = top;
			playerValid.Width = 15;
			playerValid.Enabled = false;
			playerValid.TextColor = Color.Red;
			playerValid.ToolTip = new CustomToolTip(manager);
			playerValid.ToolTip.Parent = playerValid;
			playerValid.ToolTip.Init();
			playerValid.ToolTip.Text = "1";
			playerValid.ToolTip.TextColor = Color.White;
			playerValid.ToolTip.Visible = false;
			playerValid.ToolTip.Color = Color.Red;

			switchesValid = new CheckBox(manager);
			switchesValid.Init();
			switchesValid.Parent = toolbarWindow;
			switchesValid.Text = "";
			switchesValid.Left = 31;
			switchesValid.Top = top;
			switchesValid.Width = 15;
			switchesValid.Enabled = false;
			switchesValid.TextColor = Color.Red;
			switchesValid.ToolTip = new CustomToolTip(manager);
			switchesValid.ToolTip.Parent = switchesValid;
			switchesValid.ToolTip.Init();
			switchesValid.ToolTip.Text = "2";
			switchesValid.ToolTip.TextColor = Color.White;
			switchesValid.ToolTip.Visible = false;
			switchesValid.ToolTip.Color = Color.Red;
			//

			exportMap = new Button(manager);
			exportMap.Init();
			exportMap.Parent = toolbarWindow;
			exportMap.Text = "Export Map";
			exportMap.Width = toolbarWindow.Width;
			exportMap.Height = 24;
			exportMap.Left = 0;
			exportMap.Top = toolbarWindow.Height - 24 * 5;
			exportMap.Click += new TomShane.Neoforce.Controls.EventHandler(ExportMapBehavior);

			saveMap = new Button(manager);
			saveMap.Init();
			saveMap.Parent = toolbarWindow;
			saveMap.Text = "Save Map";
			saveMap.Width = toolbarWindow.Width;
			saveMap.Height = 24;
			saveMap.Left = 0;
			saveMap.Top = toolbarWindow.Height - 24 * 4;
			saveMap.Click += new TomShane.Neoforce.Controls.EventHandler(SaveMapBehavior);

			newMap = new Button(manager);
			newMap.Init();
			newMap.Parent = toolbarWindow;
			newMap.Text = "New Map";
			newMap.Width = toolbarWindow.Width;
			newMap.Height = 24;
			newMap.Left = 0;
			newMap.Top = toolbarWindow.Height - 24 * 3;
			newMap.Click += new TomShane.Neoforce.Controls.EventHandler(NewMapBehavior);

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
			

			newMapConfirmationWindow = new Window(manager);
			newMapConfirmationWindow.Init();
			newMapConfirmationWindow.Text = "Start a new map?";
			newMapConfirmationWindow.Width = 248;
			newMapConfirmationWindow.Height = 48;
			newMapConfirmationWindow.Center();
			newMapConfirmationWindow.Visible = false;
			newMapConfirmationWindow.Resizable = false;
			manager.Add(newMapConfirmationWindow);

			accept = new Button(manager);
			accept.Init();
			accept.Parent = newMapConfirmationWindow;
			accept.Width = 100;
			accept.Height = 24;
			accept.Click += new TomShane.Neoforce.Controls.EventHandler(ConfirmedNewMapBehavior);
			accept.Left = 12;
			accept.Top = 8;
			accept.Text = "Yes";

			cancel = new Button(manager);
			cancel.Init();
			cancel.Parent = newMapConfirmationWindow;
			cancel.Width = 100;
			cancel.Height = 24;
			cancel.Click += new TomShane.Neoforce.Controls.EventHandler(CancelNewMapBehavior);
			cancel.Left = 124;
			cancel.Top = 8;
			cancel.Text = "No thanks";
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

			HighligthButton(paintTool);
        }
        public void RoadToolBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.ROAD_TOOL);

			HighligthButton(roadTool);
        }
        public void EraseToolBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.ERASE_TOOL);

			HighligthButton(eraserTool);
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
		private void NewMapBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			newMapConfirmationWindow.ShowModal();
			cancel.Focused = true;
		}
		private void ConfirmedNewMapBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			Entity singleTilemap = world.TagManager.GetEntity("singlesTilemap");
			Entity roadTilemap = world.TagManager.GetEntity("roadTilemap");

			((Tilemap)roadTilemap.GetComponent<Tilemap>()).clear();
			((Tilemap)singleTilemap.GetComponent<Tilemap>()).clear();
			newMapConfirmationWindow.Close();

			UndoTreeSystem sys = (UndoTreeSystem)world.SystemManager.GetSystem<UndoTreeSystem>()[0];
			sys.ClearTheUndoTree();
			world.SystemManager.GetSystem<ActionSystem>()[0].Initialize();
		}

		private void CancelNewMapBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			newMapConfirmationWindow.Close();
		}
		public void ExportMapBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			Button btn = (Button)sender;
			btn.Focused = false;
			System.Windows.Forms.SaveFileDialog exportMapDialog = new System.Windows.Forms.SaveFileDialog();
			exportMapDialog.InitialDirectory = Convert.ToString(Environment.SpecialFolder.CommonProgramFilesX86);
			exportMapDialog.Filter = "Map files (*.datmap)|*.datmap";
			exportMapDialog.FilterIndex = 1;
			exportMapDialog.Title = "Export your map";
			exportMapDialog.FileOk += new System.ComponentModel.CancelEventHandler(SuccessfullyExportedMap);
			exportMapDialog.ShowDialog();
		}

		public void SaveMapBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			Button btn = (Button)sender;
			btn.Focused = false;
			System.Windows.Forms.SaveFileDialog saveMapDialog = new System.Windows.Forms.SaveFileDialog();
			saveMapDialog.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
			saveMapDialog.Filter = "Project files (*.cheeseboy)|*.cheeseboy";
			saveMapDialog.FilterIndex = 1;
			saveMapDialog.Title = "Save your project";
			saveMapDialog.FileOk += new System.ComponentModel.CancelEventHandler(SuccessfullySavedMap);
			saveMapDialog.ShowDialog();
		}
		private void SuccessfullyExportedMap(object sender, System.EventArgs e)
		{
			System.Windows.Forms.SaveFileDialog dialog = (System.Windows.Forms.SaveFileDialog)(sender);
			((ExportMapSystem)world.SystemManager.GetSystem<ExportMapSystem>()[0]).RequestToSaveMap(dialog.FileName);
		}
		private void SuccessfullySavedMap(object sender, System.EventArgs e)
		{
			System.Windows.Forms.SaveFileDialog dialog = (System.Windows.Forms.SaveFileDialog)(sender);
			((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]).SaveSerialiazedActions(dialog.FileName);
		}

		private void ResetTools(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			roadTool.Mode = ButtonMode.Normal;
			paintTool.Mode = ButtonMode.Normal;
			eraserTool.Mode = ButtonMode.Normal;
		}

		private void HighligthButton(ImageBasedButton p_button)
		{
			if (p_button != eraserTool)
			{
				eraserTool.Mode = ButtonMode.Normal;
			}
			if (p_button != paintTool)
			{
				paintTool.Mode = ButtonMode.Normal;
			}
			if (p_button != roadTool)
			{
				roadTool.Mode = ButtonMode.Normal;
			}

			p_button.Mode = ButtonMode.PushButton;
			p_button.Pushed = true;
		}
	}
}
