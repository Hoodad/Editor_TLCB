using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MapEditor_TLCB.Actions.Interface
{	
	[Serializable()]
	class ActionsSerialized : ISerializable
	{
		public List<ActionInterface> queuedActions;
		public List<ActionInterface> performedActions;
		public List<ActionInterface> redoActions;

		public ActionsSerialized()
		{

		}

		public ActionsSerialized(SerializationInfo info, StreamingContext ctxt)
		{
			queuedActions = (List<ActionInterface>)info.GetValue("Queued", typeof(List<ActionInterface>));
			performedActions = (List<ActionInterface>)info.GetValue("Performed", typeof(List<ActionInterface>));
			redoActions = (List<ActionInterface>)info.GetValue("Redo", typeof(List<ActionInterface>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Queued", queuedActions);
			info.AddValue("Performed", performedActions);
			info.AddValue("Redo", redoActions);
		}
	}
}
