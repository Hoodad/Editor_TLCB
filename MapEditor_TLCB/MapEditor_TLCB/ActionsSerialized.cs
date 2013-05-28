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
		public string preferredTileMap;
        public int currentNode = 0;

		const String ACTIONNODE = "ActionNode";
		const String ACTION = "ActionInterface";
        const String CURRENTNODE = "CurrentNode";
		const String TILEMAP = "TileMap";

		public ActionsSerialized()
		{
		}

		public ActionsSerialized(SerializationInfo info, StreamingContext ctxt)
		{
			nodes	= (InvariableIndexList<ActionNode>)info.GetValue(		ACTIONNODE,		typeof(InvariableIndexList<ActionNode>));
			actions = (InvariableIndexList<ActionInterface>)info.GetValue(	ACTION,			typeof(InvariableIndexList<ActionInterface>));
            currentNode = (int)info.GetValue(CURRENTNODE, typeof(int));
			preferredTileMap = (string)info.GetValue(TILEMAP, typeof(string));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(ACTIONNODE, nodes);
			info.AddValue(ACTION, actions);
            info.AddValue(CURRENTNODE, currentNode);
			info.AddValue(TILEMAP, preferredTileMap);
		}
	}
}
