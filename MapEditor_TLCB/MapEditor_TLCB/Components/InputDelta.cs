using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB.Components
{
	class InputDelta: Component
	{
		public InputDelta()
		{
			previousKeyboard = Keyboard.GetState();
			currentKeyboard = Keyboard.GetState();
		}

		public float getDeltaKey(Keys p_key)
		{
			float delta = 0.0f;
			if (currentKeyboard.IsKeyDown(p_key) && previousKeyboard.IsKeyUp(p_key))
			{
				delta = 1.0f;
			}
			else if (currentKeyboard.IsKeyUp(p_key) && previousKeyboard.IsKeyDown(p_key))
			{
				delta = -1.0f;
			}

			return delta;
		}

		public KeyboardState previousKeyboard;
		public KeyboardState currentKeyboard;
	}
}
