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
			scale = 1.0f;
		}

		public Transform(Vector2 p_position)
		{
			position = p_position;
		}

		public Transform(Vector2 p_position, float p_scale)
		{
			position = p_position;
			scale = p_scale;
		}

		public Matrix getMatrix()
		{
			return Matrix.CreateScale(scale) * Matrix.CreateTranslation(position.X, position.Y, 0);
		}

		public Vector2 position;
		public float scale;
	}
}
