using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Artemis;
using Microsoft.Xna.Framework;
using MapEditor_TLCB.UndoTree;

namespace MapEditor_TLCB.Systems
{
	class UndoTreeSystem : EntitySystem
	{
		Manager manager;
		Window undoTreeWindow;

        Button undoBtn;
        Button redoBtn;
        RadioButton viewMode;

        UndoTreeContext undoTreeContext;
        GraphicsDevice m_gd;


		public UndoTreeSystem(Manager p_manager, GraphicsDevice p_gd)
		{
			manager = p_manager;
            m_gd = p_gd;
		}

		public override void Initialize()
		{
            ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();

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

            undoTreeContext = new UndoTreeContext(manager, undoTreeWindow, m_gd);
            undoTreeContext.Init();
            undoTreeContext.Width = undoTreeWindow.Width;
            undoTreeContext.Height = undoTreeWindow.Height;
            undoTreeContext.Parent = undoTreeWindow;
            undoTreeContext.CanFocus = false;
            undoTreeContext.Click += new TomShane.Neoforce.Controls.EventHandler(OnClick);
		}

		public override void Process()
		{
            undoTreeContext.Update((float)world.Delta/1000.0f);
		}


        public void OnClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

        }
	}
}
