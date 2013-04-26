using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;

namespace MapEditor_TLCB.Components
{
	class TilemapRender: Component
	{
		public TilemapRender(string p_theme, bool p_overlay)
		{
			theme = p_theme;
			overlay = p_overlay;
		}

		public string theme;
		public bool overlay;
	}
}
