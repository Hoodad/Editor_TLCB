using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;

namespace MapEditor_TLCB.Components
{
	class TilemapValidate: Component
	{
		public TilemapValidate()
		{
			pathsValid = false;
			playerValid = false;
			switchesValid = false;
			validateThisTick = true;
		}

		public bool pathsValid;
		public bool playerValid;
		public bool switchesValid;
		public bool validateThisTick;
	}
}
