using System;
using MapEditor_TLCB.Actions.Interface;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MapEditor_TLCB
{
	class Serializer
	{
		public void SerializeObject(string p_fileName, ActionsSerialized p_object)
		{
			Stream stream = File.Open(p_fileName, FileMode.Create);
			BinaryFormatter binaryForm = new BinaryFormatter();
			binaryForm.Serialize(stream, p_object);
			stream.Close();
		}

		public ActionsSerialized DeSerializeObject(string p_fileName)
		{
			ActionsSerialized deSerializeObj;
			Stream stream = File.Open(p_fileName, FileMode.Open);
			BinaryFormatter binaryForm = new BinaryFormatter();
			deSerializeObj = (ActionsSerialized)binaryForm.Deserialize(stream);
			stream.Close();
			return deSerializeObj;
		}
	}
}
