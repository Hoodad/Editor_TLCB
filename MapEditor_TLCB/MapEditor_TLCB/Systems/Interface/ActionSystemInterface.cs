using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Actions.Interface;

namespace MapEditor_TLCB.Systems.Interface
{
	interface ActionSystemInterface
	{
		void ReceiveAction(ActionInterface p_action);
	}
}
