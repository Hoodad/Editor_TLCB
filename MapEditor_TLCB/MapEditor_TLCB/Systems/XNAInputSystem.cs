using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB.Systems
{
	class XNAInputSystem : EntitySystem
	{
		KeyboardState oldKeyboardState;
		KeyboardState currentKeyboardState;

		public XNAInputSystem()
			: base()
		{

		}
		public override void Initialize()
		{
			oldKeyboardState = Keyboard.GetState(0);
			currentKeyboardState = Keyboard.GetState(0);
		}

		public override void Process()
		{
			oldKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState(0);
		}

		public bool IsDown(Keys p_key)
		{
			return (currentKeyboardState.IsKeyDown(p_key)) ? true : false;	
		}
		public bool HasBeenPressed(Keys p_key)
		{
			if (currentKeyboardState.IsKeyUp(p_key))
			{
				if (oldKeyboardState.IsKeyDown(p_key))
				{
					return true;
				}
			}
			return false;
		}
	}
}
