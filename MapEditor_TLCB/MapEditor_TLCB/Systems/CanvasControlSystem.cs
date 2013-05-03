using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class CanvasControlSystem: TagSystem
	{
		public CanvasControlSystem(): base("mainCamera")
		{
			previousState = Mouse.GetState();
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
		}

		private MouseState previousState;
	}
}
