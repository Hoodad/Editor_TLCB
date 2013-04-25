using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Components
{
	class Transform: Component
	{
		public Transform()
		{
			position = Vector2.Zero;
		}

		public Transform(Vector2 p_position)
		{
			position = p_position;
		}

		public Vector2 position;
	}
}
