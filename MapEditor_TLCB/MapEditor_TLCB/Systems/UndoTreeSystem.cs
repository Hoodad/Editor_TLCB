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

        // ScrollBar sbVert;
        // ScrollBar sbHorz;

        UndoTreeContainer undoTreeContainer;
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
            undoTreeWindow.AutoScroll = false;


			//toolbarWindow.BorderVisible = false;
			//toolbar.Movable = false;
			manager.Add(undoTreeWindow);



            undoTreeContainer = new UndoTreeContainer(manager, undoTreeWindow, m_gd);
            undoTreeContainer.Init();
            undoTreeContainer.Width = undoTreeWindow.Width;
            undoTreeContainer.Height = undoTreeWindow.Height;
            undoTreeContainer.Parent = undoTreeWindow;
            undoTreeContainer.CanFocus = false;

            undoBtn = new Button(manager);
            undoBtn.Init();
            undoBtn.Parent = undoTreeWindow;
            undoBtn.Width = undoTreeWindow.Width / 2;
            undoBtn.Height = 24;
            undoBtn.Left = 0;
            undoBtn.Top = 0;
            undoBtn.Text = "Undo";
            undoBtn.Click += new TomShane.Neoforce.Controls.EventHandler(UndoBehaviour);

            redoBtn = new Button(manager);
            redoBtn.Init();
            redoBtn.Parent = undoTreeWindow;
            redoBtn.Width = undoTreeWindow.Width / 2;
            redoBtn.Height = 24;
            redoBtn.Left = undoTreeWindow.Width / 2;
            redoBtn.Top = 0;
            redoBtn.Text = "Redo";
            redoBtn.Click += new TomShane.Neoforce.Controls.EventHandler(RedoBehaviour);

            viewMode = new RadioButton(manager);
            viewMode.Init();
            viewMode.Parent = undoTreeWindow;
            viewMode.Width = undoTreeWindow.Width / 2;
            viewMode.Height = 24;
            viewMode.Left = 0;
            viewMode.Top = 24;
            viewMode.Checked = true;
            viewMode.Text = "Tree view";
            viewMode.Click += new TomShane.Neoforce.Controls.EventHandler(ViewModeBehaviour);

            /*sbVert = new ScrollBar(manager, Orientation.Vertical);
            sbVert.Init();
            sbVert.Detached = false;
            sbVert.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            //sbVert.ValueChanged += new EventHandler(ScrollBarValueChanged);
            sbVert.Range = 0;
            sbVert.PageSize = 0;
            sbVert.Value = 0;
            sbVert.Visible = true;
            undoTreeWindow.Add(sbVert);

            sbHorz = new ScrollBar(manager, Orientation.Horizontal);
            sbHorz.Init();
            sbHorz.Detached = false;
            sbHorz.Anchor = Anchors.Right | Anchors.Left | Anchors.Bottom;
            //sbHorz.ValueChanged += new EventHandler(ScrollBarValueChanged);
            sbHorz.Range = 0;
            sbHorz.PageSize = 0;
            sbHorz.Value = 0;
            sbHorz.Visible = true;
            undoTreeWindow.Add(sbHorz);*/

       
            // undoTreeContainer.Click += new TomShane.Neoforce.Controls.EventHandler(OnClick);
		}

		public override void Process()
		{
            undoTreeContainer.Update((float)world.Delta/1000.0f);
		}


        public void OnClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

        }

        public void UndoBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            undoTreeContainer.m_undoTree.undo();
        }

        public void RedoBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            undoTreeContainer.m_undoTree.redo();
        }

        public void ViewModeBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (undoTreeContainer.m_undoTree.getMode() == UndoTree.UndoTree.Mode.LIST)
            {
                undoTreeContainer.m_undoTree.setMode(UndoTree.UndoTree.Mode.TREE); // checked
                rb.Checked = true;
            }
            else
            {
                undoTreeContainer.m_undoTree.setMode(UndoTree.UndoTree.Mode.LIST); // unchecked
                rb.Checked = false;
            }
        }
	}
}
