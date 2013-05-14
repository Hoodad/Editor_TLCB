using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MapEditor_TLCB.Common;

namespace MapEditor_TLCB.Actions.Interface
{	
	//This class can be removed and replaced if needed.
	[Serializable()]
	class ActionsSerialized : ISerializable
	{
		//public List<ActionInterface> queuedActions;
		public InvariableIndexList<ActionNode> nodes;
		public InvariableIndexList<ActionInterface> actions;

		const String ACTIONNODE = "ActionNode";
		const String ACTION = "ActionInterface";

		public ActionsSerialized()
		{

		}

		public ActionsSerialized(SerializationInfo info, StreamingContext ctxt)
		{
			//queuedActions = (List<ActionInterface>)info.GetValue("Queued", typeof(List<ActionInterface>));
			nodes	= (InvariableIndexList<ActionNode>)info.GetValue(		ACTIONNODE,		typeof(InvariableIndexList<ActionNode>));
			actions = (InvariableIndexList<ActionInterface>)info.GetValue(	ACTION,			typeof(InvariableIndexList<ActionInterface>));

		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//info.AddValue("Queued", queuedActions);
			info.AddValue(ACTIONNODE, nodes);
			info.AddValue(ACTION, actions);
		}
	}
}
