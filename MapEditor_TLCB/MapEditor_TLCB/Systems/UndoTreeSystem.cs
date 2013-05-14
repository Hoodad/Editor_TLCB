using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MapEditor_TLCB.CustomControls;
using MapEditor_TLCB.Actions;
using MapEditor_TLCB.Actions.Interface;

namespace MapEditor_TLCB.Systems
{
    class UndoTreeSystem : EntitySystem
    {
        Manager manager;
        Window undoTreeWindow;

        Button undoBtn;
        Button redoBtn;
        RadioButton viewMode;

        ScrollBar sbVert;
        ScrollBar sbHorz;
        private const int scrollMax = 100;

        public UndoTreeContainer undoTreeContainer;
        GraphicsDevice m_gd;
        ContentManager m_content;

        private int m_scrollWheelValue = 0;
        private int m_previousScrollWheelValue = 0;

        private ActionSystem m_actionsystem;

        public UndoTreeSystem(Manager p_manager, GraphicsDevice p_gd, ContentManager p_content)
        {
            manager = p_manager;
            m_gd = p_gd;
            m_content = p_content;
        }

        public override void Initialize()
        {
            ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
            m_actionsystem = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);
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
            undoTreeWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
            undoTreeWindow.IconVisible = false;
            undoTreeWindow.AutoScroll = false;


            //toolbarWindow.BorderVisible = false;
            //toolbar.Movable = false;
            manager.Add(undoTreeWindow);



            undoTreeContainer = new UndoTreeContainer(manager, undoTreeWindow, m_gd, m_content);
            undoTreeContainer.Init();
            undoTreeContainer.Width = viewport.Width-16;
            undoTreeContainer.Height = viewport.Height-16;
            undoTreeContainer.Parent = undoTreeWindow;
            undoTreeContainer.CanFocus = false;
            undoTreeContainer.Click += new TomShane.Neoforce.Controls.EventHandler(OnContainerClickBehavior);
            undoTreeContainer.DoubleClicks = false;

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

            viewMode = new RadioButton(manager);
            viewMode.Init();
            viewMode.Parent = undoTreeWindow;
            viewMode.Width = undoTreeWindow.Width / 2;
            viewMode.Height = 24;
            viewMode.Left = 0;
            viewMode.Top = 48;
            viewMode.Checked = false;
            viewMode.Text = "Mini view";
            viewMode.Click += new TomShane.Neoforce.Controls.EventHandler(ZoomModeBehaviour);

            sbVert = new ScrollBar(manager, Orientation.Vertical);
            sbVert.Init();
            sbVert.Detached = false;
            sbVert.Parent = undoTreeWindow;
            sbVert.Height = undoTreeWindow.ClientHeight-16;
            sbVert.SetPosition(undoTreeWindow.ClientWidth - sbVert.Width, 0);
            sbVert.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            sbVert.ValueChanged += new TomShane.Neoforce.Controls.EventHandler(ScrollBarValueChangedY);
            sbVert.MouseScroll += new TomShane.Neoforce.Controls.MouseEventHandler(ScrollBarMouseScroll);
            sbVert.Range = scrollMax;
            sbVert.PageSize = 0;
            sbVert.Value = 0;
            sbVert.Visible = true;
            undoTreeWindow.Add(sbVert);

            sbHorz = new ScrollBar(manager, Orientation.Horizontal);
            sbHorz.Init();
            sbHorz.Detached = false;
            sbHorz.Parent = undoTreeWindow;
            sbHorz.Width = undoTreeWindow.ClientWidth-16;
            sbHorz.SetPosition(0, undoTreeWindow.ClientHeight - sbHorz.Height);
            sbHorz.Anchor = Anchors.Left | Anchors.Right | Anchors.Bottom;
            sbHorz.ValueChanged += new TomShane.Neoforce.Controls.EventHandler(ScrollBarValueChangedX);
            sbHorz.Range = scrollMax;
            sbHorz.PageSize = 0;
            sbHorz.Value = 0;
            sbHorz.Visible = true;
            undoTreeWindow.Add(sbHorz);


            // undoTreeContainer.Click += new TomShane.Neoforce.Controls.EventHandler(OnClick);
        }

        public override void Process()
        {
            undoTreeContainer.m_undoTree.m_renderArea = undoTreeWindow.ClientRect;
            undoTreeContainer.Update((float)world.Delta / 1000.0f);
        }
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
            Notification n = new Notification("The Undo Tree allows you to jump between various map states.", NotificationType.INFO);
            noteSys.AddNotification(n);

        }


        public void OnContainerClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TomShane.Neoforce.Controls.MouseEventArgs me = e as TomShane.Neoforce.Controls.MouseEventArgs;

            if (me.Button == MouseButton.Left)
            {
                List<ActionInterface> actions = undoTreeContainer.m_undoTree.setCurrentByPosition(me.Position.X, me.Position.Y);
                m_actionsystem.PerformActionList(actions);
            }

        }

        public void UndoBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            m_actionsystem.UndoLastPerformedAction();
        }

        public void RedoBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            m_actionsystem.RedoLastAction();
        }

        public void ViewModeBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (undoTreeContainer.m_undoTree.getMode() == UndoTree.Mode.LIST)
            {
                undoTreeContainer.m_undoTree.setMode(UndoTree.Mode.TREE); // checked
                rb.Checked = true;
            }
            else
            {
                undoTreeContainer.m_undoTree.setMode(UndoTree.Mode.LIST); // unchecked
                rb.Checked = false;
            }
        }

        public void ZoomModeBehaviour(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (undoTreeContainer.m_undoTree.getZoom() == UndoTree.Zoom.MINI)
            {
                undoTreeContainer.m_undoTree.setZoom(UndoTree.Zoom.NORMAL); // unchecked
                rb.Checked = false;
            }
            else
            {
                undoTreeContainer.m_undoTree.setZoom(UndoTree.Zoom.MINI); // checked
                rb.Checked = true;
            }
        }

        public void ScrollBarValueChangedY(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ScrollBar sb = sender as ScrollBar;
            undoTreeContainer.m_undoTree.ScrollY(sb.Value, scrollMax);
        }

        public void ScrollBarValueChangedX(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ScrollBar sb = sender as ScrollBar;
            undoTreeContainer.m_undoTree.ScrollX(sb.Value, scrollMax);
        }

        public void ScrollBarMouseScroll(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            ScrollBar sb = sender as ScrollBar;
            m_scrollWheelValue = me.State.ScrollWheelValue;
            int scrollDiff = m_scrollWheelValue - m_previousScrollWheelValue;
            m_previousScrollWheelValue = m_scrollWheelValue;
            sb.Value += scrollDiff;
        }


    }
}