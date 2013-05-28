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
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB.Systems
{
    class UndoTreeSystem : EntitySystem
    {
        Manager manager;
        Window undoTreeWindow;

        RadioButton viewMode;

        ScrollBar sbVert;
        ScrollBar sbHorz;
        private const int scrollMax = 100;

		bool haveShownUndoTreeInfo = false;

        public UndoTreeContainer undoTreeContainer;
        GraphicsDevice m_gd;
        ContentManager m_content;
        private Vector2 oldMousePos=Vector2.Zero;

        private int m_scrollWheelValue = 0;
        private int m_previousScrollWheelValue = 0;

        private ActionSystem m_actionsystem;

        float refocusTick = 0.0f;

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

            int toolbarwindowh = 360;
            undoTreeWindow = new Window(manager);
            undoTreeWindow.Init();
            undoTreeWindow.Text = "History";
            undoTreeWindow.Width = 160;
            undoTreeWindow.Height = (int)((float)viewport.Height - toolbarwindowh);
            undoTreeWindow.Top = toolbarwindowh; // height of toolbarwindow
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
            undoTreeContainer.MouseScroll += new TomShane.Neoforce.Controls.MouseEventHandler(OnContainerScrollBehaviour);
            //undoTreeContainer.MousePress += new TomShane.Neoforce.Controls.MouseEventHandler(OnContainerPanBehaviour);
			undoTreeContainer.MouseMove += new MouseEventHandler(OnContainerPanBehaviour);
            undoTreeContainer.DoubleClicks = false;

            /*
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
             * */



            sbVert = new ScrollBar(manager, Orientation.Vertical);
            sbVert.Init();
            sbVert.Detached = false;
            sbVert.Parent = undoTreeWindow;
            sbVert.Height = undoTreeWindow.ClientHeight-16;
            sbVert.SetPosition(undoTreeWindow.ClientWidth - sbVert.Width, 0);
            sbVert.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            sbVert.ValueChanged += new TomShane.Neoforce.Controls.EventHandler(ScrollBarValueChangedY);
            // sbVert.MouseScroll += new TomShane.Neoforce.Controls.MouseEventHandler(ScrollBarMouseScroll);
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
            float dt = (float)world.Delta / 1000.0f;
            undoTreeContainer.Update(dt);

            //
            if (undoTreeContainer.m_undoTree.isThereANewNode())
            {
                refocusTick = 1.0f;
            }
            if (refocusTick > 0.0f)
            {
                refocusTick -= dt;
                Vector2 newPos = Vector2.Max(Vector2.Zero, undoTreeContainer.m_undoTree.getCurrentNodeContextPosition() - new Vector2(undoTreeWindow.Width * 0.2f, undoTreeWindow.Height * 0.3f));
                // newPos -= new Vector2(undoTreeWindow.Width, undoTreeWindow.Height) * 0.15f;
                undoTreeContainer.m_undoTree.scrollOffset = Vector2.Lerp(undoTreeContainer.m_undoTree.scrollOffset,-newPos,10.0f*dt);
                UpdateScrollBarsFromTreeValues();
            }
        }
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
			if (!haveShownUndoTreeInfo)
			{
				NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
				Notification n = new Notification("The Undo Tree allows you to jump between various map states.", NotificationType.INFO);
				noteSys.AddNotification(n);

				haveShownUndoTreeInfo = true;
			}
        }


        public void OnContainerClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TomShane.Neoforce.Controls.MouseEventArgs me = e as TomShane.Neoforce.Controls.MouseEventArgs;

            undoTreeContainer.m_undoTree.m_currentMousePosX = me.Position.X - undoTreeWindow.AbsoluteLeft;
            undoTreeContainer.m_undoTree.m_currentMousePosY = me.Position.Y - undoTreeWindow.AbsoluteTop;

            if (me.Button == MouseButton.Left)
            {
                List<ActionInterface> actions = undoTreeContainer.m_undoTree.setCurrentByPosition();
                m_actionsystem.PerformActionList(actions);

                world.TagManager.GetEntity("mainTilemap").GetComponent<TilemapValidate>().validateThisTick = true;
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

        /*
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
        }*/

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

        private void OnContainerPanBehaviour(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            MouseState currentState = e.State;
            Vector2 mousePos = new Vector2(e.Position.X, e.Position.Y);
            undoTreeContainer.m_undoTree.m_currentMousePosX = e.Position.X+undoTreeWindow.ClientLeft;
            undoTreeContainer.m_undoTree.m_currentMousePosY = e.Position.Y+undoTreeWindow.ClientTop;


            if (e.State.MiddleButton == ButtonState.Pressed)
            {
                Vector2 mouseDiff = mousePos - oldMousePos;
                undoTreeContainer.m_undoTree.scrollOffset += mouseDiff*3.0f;
                undoTreeContainer.m_undoTree.scrollOffset = Vector2.Min(Vector2.Zero, undoTreeContainer.m_undoTree.scrollOffset);
                // update gui
                UpdateScrollBarsFromTreeValues();
            }
            oldMousePos = mousePos;
            refocusTick = 0.0f;
            
        }

        public void OnContainerScrollBehaviour(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            m_scrollWheelValue=e.State.ScrollWheelValue;
            Vector2 mousePos = new Vector2(e.Position.X, e.Position.Y)-new Vector2(undoTreeContainer.AbsoluteLeft,undoTreeContainer.AbsoluteTop);
            mousePos -= undoTreeContainer.m_undoTree.m_renderOffset;
            Vector2 border = new Vector2(undoTreeWindow.Width, undoTreeWindow.Height);
            //mousePos /= border;
            Vector2 mousePosOldScale = mousePos/(undoTreeContainer.m_undoTree.m_zoomValue/*(undoTreeContainer.m_undoTree.m_nodeWidth/undoTreeContainer.m_undoTree.m_nodeOrigWidth)*/);

            int diff = m_scrollWheelValue - m_previousScrollWheelValue;
            m_previousScrollWheelValue = m_scrollWheelValue;

            if (diff > 0)
            {
                undoTreeContainer.m_undoTree.m_zoomValue *= 1.15f; // use same values as canvas zoom
            }
            else if (diff < 0)
            {
                undoTreeContainer.m_undoTree.m_zoomValue /= 1.15f;
            }
            undoTreeContainer.m_undoTree.RefreshZoom(); // update conditions and apply boundaries
            // recalc offsets
            Vector2 mousePosNewScale = mousePos / (undoTreeContainer.m_undoTree.m_zoomValue /* (undoTreeContainer.m_undoTree.m_nodeWidth / undoTreeContainer.m_undoTree.m_nodeOrigWidth)*/);
            Vector2 scroll = (mousePosNewScale - mousePosOldScale);
            undoTreeContainer.m_undoTree.scrollOffset += scroll;
            // update gui
            refocusTick = 0.0f;
            UpdateScrollBarsFromTreeValues();
        }

        public void UpdateScrollBarsFromTreeValues()
        {
            sbHorz.Value = (int)((undoTreeContainer.m_undoTree.scrollOffset.X / -undoTreeContainer.m_undoTree.m_totalSize.X) * (float)scrollMax);
            sbVert.Value = (int)((undoTreeContainer.m_undoTree.scrollOffset.Y / -undoTreeContainer.m_undoTree.m_totalSize.Y) * (float)scrollMax);
        }

		public void ClearTheUndoTree()
		{
            undoTreeContainer.m_undoTree.ResetData();
		}

    }
}