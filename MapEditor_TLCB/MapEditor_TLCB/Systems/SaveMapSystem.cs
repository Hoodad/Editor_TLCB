using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Artemis;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB.Systems
{
	class SaveMapSystem : EntitySystem
	{
		bool requestedToSaveMap = false;
		string completePath;

		public SaveMapSystem()
		{
			completePath = "";
		}

		public override void Process()
		{
			if (requestedToSaveMap)
			{
				Entity mainTilemap = world.TagManager.GetEntity("mainTilemap");
				if (mainTilemap != null)
				{
					File.WriteAllText(completePath, "DERP");
					requestedToSaveMap = false;
				}
			}
		}
		public void RequestToSaveMap(string p_completePath)
		{
			if (!requestedToSaveMap)
			{
				completePath = p_completePath;
				requestedToSaveMap = true;
			}
			else
			{
				Notification alreadySaving = new Notification("You are already trying to save a file!",NotificationType.WARNING);
				((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(alreadySaving);
			}
		}
	}
}
