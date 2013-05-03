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
		}

		public override void Process(Artemis.Entity e)
		{
			Transform camTransform = e.GetComponent<Transform>();
			if (camTransform != null)
			{
				MouseState currentState = Mouse.GetState();
				Vector2 mouseDiff = new Vector2(currentState.X - previousState.X,
					currentState.Y - previousState.Y);
				
				if (currentState.MiddleButton == ButtonState.Pressed &&
					previousState.MiddleButton == ButtonState.Pressed)
				{
					camTransform.position += mouseDiff;
				}
				


				Vector2 mousePos = new Vector2(currentState.X, currentState.Y);
				Vector2 worldMousePos = Vector2.Transform(mousePos, Matrix.Invert(camTransform.getMatrix()));

				int scrollDiff = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
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

				previousState = currentState;
			}
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
			m_canvasWindow.BorderVisible = false;
			m_canvasWindow.Resizable = false;
			m_canvasWindow.StayOnBack = true;
			//m_canvasWindow.CanFocus = false;
			//m_canvasWindow.Focused = true;
			//m_canvasWindow.Passive = true;
			m_canvasWindow.CanvasTexture = m_canvasRender;
			RoadToolSystem roadSys = ((RoadToolSystem)world.SystemManager.GetSystem<RoadToolSystem>()[0]);
			m_canvasWindow.MouseMove += new MouseEventHandler(roadSys.canvasWindow_MouseMove);
			
			m_manager.Add(m_canvasWindow);
		}

		public Window getCanvasWindow()
		{
			return m_canvasWindow;
		}

		private MouseState previousState;
		RenderTarget2D m_canvasRender;
		CanvasWindow m_canvasWindow;
		Manager m_manager;
	}
}
