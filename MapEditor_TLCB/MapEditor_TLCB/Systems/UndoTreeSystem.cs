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

        Button undoBtn;
        Button redoBtn;
        RadioButton viewMode;

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

            undoBtn = new Button(manager);
            undoBtn.Init();
            undoBtn.Parent = undoTreeWindow;
            undoBtn.Width = undoTreeWindow.Width / 2;
            undoBtn.Height = 24;
            undoBtn.Left = 0;
            undoBtn.Top = 0;
            undoBtn.Text = "Undo";

            redoBtn = new Button(manager);
            redoBtn.Init();
            redoBtn.Parent = undoTreeWindow;
            redoBtn.Width = undoTreeWindow.Width / 2;
            redoBtn.Height = 24;
            redoBtn.Left = undoTreeWindow.Width / 2;
            redoBtn.Top = 0;
            redoBtn.Text = "Redo";

            viewMode = new RadioButton(manager);
            viewMode.Init();
            viewMode.Parent = undoTreeWindow;
            viewMode.Width = undoTreeWindow.Width / 2;
            viewMode.Height = 24;
            viewMode.Left = 0;
            viewMode.Top = 24;
            viewMode.Text = "Tree view";
		}

		public override void Process()
		{
		}

        public void Render(SpriteBatch p_spriteBatch)
        {
            
        }
	}
}
