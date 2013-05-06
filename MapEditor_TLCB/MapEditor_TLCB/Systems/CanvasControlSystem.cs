using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.CustomControls;
using TomShane.Neoforce.Controls;

namespace MapEditor_TLCB.Systems
{
	class CanvasControlSystem: TagSystem
	{
		public CanvasControlSystem(Manager p_manager, RenderTarget2D p_canvasRender)
			: base("mainCamera")
		{
			previousState = Mouse.GetState();
			m_canvasRender = p_canvasRender;
			m_manager = p_manager;
			m_scrollWheelValue = 0;
			m_previousScrollWheelValue = 0;
		}

		public override void Process(Artemis.Entity e)
		{
			m_canvasWindow.Refresh();
		}

		public override void Initialize()
		{
			ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();
			m_canvasWindow = new CanvasWindow(m_manager);
			m_canvasWindow.Init();
			m_canvasWindow.Left = 0;
			m_canvasWindow.Top = 0;
			m_canvasWindow.Width = m_manager.Window.Width;
			m_canvasWindow.Height = m_manager.Window.Height;
			m_canvasWindow.Parent = null;
			//m_canvasWindow.BorderVisible = false;
			m_canvasWindow.Resizable = false;
			m_canvasWindow.StayOnBack = true;
			m_canvasWindow.CanvasTexture = m_canvasRender;
			RoadToolSystem roadSys = ((RoadToolSystem)world.SystemManager.GetSystem<RoadToolSystem>()[0]);
			m_canvasWindow.MouseDown += new MouseEventHandler(roadSys.canvasGroupBehavior);
			m_canvasWindow.MouseUp += new MouseEventHandler(roadSys.canvasGroupBehavior);
			m_canvasWindow.MouseDown += new MouseEventHandler(roadSys.canvasWindow_MouseMove);
			m_canvasWindow.MouseMove += new MouseEventHandler(roadSys.canvasWindow_MouseMove);
			m_canvasWindow.MouseMove += new MouseEventHandler(canvasWindow_MouseMove);
			m_canvasWindow.MouseScroll += new MouseEventHandler(canvasWindow_MouseScroll);
			m_manager.Add(m_canvasWindow);
		}

		private void canvasWindow_MouseMove(object sender, MouseEventArgs e)
		{
			MouseState currentState = e.State;
			if (e.State.MiddleButton == ButtonState.Pressed)
			{
				Transform camTransform = world.TagManager.GetEntity("mainCamera").GetComponent<Transform>();
				if (camTransform != null)
				{
					Vector2 mouseDiff = new Vector2(currentState.X - previousState.X,
						currentState.Y - previousState.Y);
				
					camTransform.position += mouseDiff;
				}
				limitCameraPosition(camTransform);
			}
			previousState = currentState;
		}

		private void canvasWindow_MouseScroll(object sender, MouseEventArgs e)
		{
			Transform camTransform = world.TagManager.GetEntity("mainCamera").GetComponent<Transform>();
			if (camTransform != null)
			{
				Vector2 mousePos = new Vector2(e.Position.X, e.Position.Y);
				Vector2 worldMousePos = Vector2.Transform(mousePos, Matrix.Invert(camTransform.getMatrix()));

				m_scrollWheelValue = e.State.ScrollWheelValue;
				int scrollDiff = m_scrollWheelValue - m_previousScrollWheelValue;
				m_previousScrollWheelValue = m_scrollWheelValue;
				if (scrollDiff > 0 && camTransform.scale <= 4.0f)
				{
					camTransform.scale *= 1.15f;
					Vector2 worldMousePosChanged = Vector2.Transform(mousePos, Matrix.Invert(camTransform.getMatrix()));
					camTransform.position += (worldMousePosChanged - worldMousePos) * camTransform.scale;
				}
				else if (scrollDiff < 0 && camTransform.scale >= 0.2f)
				{
					camTransform.scale /= 1.15f;
					Vector2 worldMousePosChanged = Vector2.Transform(mousePos, Matrix.Invert(camTransform.getMatrix()));
					camTransform.position += (worldMousePosChanged - worldMousePos) * camTransform.scale;
				}
				limitCameraPosition(camTransform);
			}
		}

		private void limitCameraPosition(Transform p_camTransform)
		{
			Entity mainTilemap = World.TagManager.GetEntity("mainTilemap");
			if (mainTilemap != null)
			{
				Tilemap tilemap = mainTilemap.GetComponent<Tilemap>();
				if (tilemap != null)
				{
					float camX = -p_camTransform.position.X / p_camTransform.scale;
					float camY = -p_camTransform.position.Y / p_camTransform.scale;
					float maxWidth = (float)tilemap.getColumns() * 32;
					float maxHeight = (float)tilemap.getRows() * 32;
					float canvasWidth = (float)m_canvasWindow.Width / p_camTransform.scale;
					float canvasHeight = (float)m_canvasWindow.Height / p_camTransform.scale;
					float camCenterX = camX + canvasWidth / 2.0f;
					float camCenterY = camY + canvasHeight / 2.0f;
					if (camCenterX > maxWidth)
					{
						p_camTransform.position.X = -(maxWidth - canvasWidth / 2.0f) * p_camTransform.scale;
					}
					else if (camCenterX < 0)
					{
						p_camTransform.position.X = (canvasWidth / 2.0f) * p_camTransform.scale;
					}
					if (camCenterY > maxHeight)
					{
						p_camTransform.position.Y = -(maxHeight - canvasHeight / 2.0f) * p_camTransform.scale;
					}
					else if (camCenterY < 0)
					{
						p_camTransform.position.Y = (canvasHeight / 2.0f) * p_camTransform.scale;
					}
				}
			}
		}

		private MouseState previousState;
		RenderTarget2D m_canvasRender;
		CanvasWindow m_canvasWindow;
		Manager m_manager;
		int m_scrollWheelValue;
		int m_previousScrollWheelValue;
	}
}
