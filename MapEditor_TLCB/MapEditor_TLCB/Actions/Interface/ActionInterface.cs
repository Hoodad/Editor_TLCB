using System.Runtime.Serialization;
using Artemis;

namespace MapEditor_TLCB.Actions.Interface
{

	interface ActionInterface : ISerializable
	{
		void PerformAction();
		void AddAffectedSystems(SystemManager p_systemManager);
	}
}
