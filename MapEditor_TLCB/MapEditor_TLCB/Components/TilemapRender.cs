using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;

namespace MapEditor_TLCB.Components
{
	class TilemapRender: Component
	{
		public TilemapRender(string p_theme)
		{
			theme = p_theme;
		}

		public string theme;
	}
}
