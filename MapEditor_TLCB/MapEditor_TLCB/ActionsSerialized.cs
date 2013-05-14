using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MapEditor_TLCB.Actions.Interface
{	
	//This class can be removed and replaced if needed.
	[Serializable()]
	class ActionsSerialized : ISerializable
	{
		public List<ActionInterface> queuedActions;

		public ActionsSerialized()
		{

		}

		public ActionsSerialized(SerializationInfo info, StreamingContext ctxt)
		{
			queuedActions = (List<ActionInterface>)info.GetValue("Queued", typeof(List<ActionInterface>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Queued", queuedActions);
		}
	}
}
