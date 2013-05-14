using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Artemis;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB.Systems
{
	class ExportMapSystem : EntitySystem
	{
		bool requestedToSaveMap = false;
		string completePath;

		public ExportMapSystem()
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
					Tilemap tilemap = mainTilemap.GetComponent <Tilemap>();

					File.WriteAllText(completePath, SaveTheTileMapToFile(tilemap).ToString());
					requestedToSaveMap = false;
					Notification alreadySaving = new Notification("Successfully exported the map! Its located " + completePath, NotificationType.SUCCESS);
					((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(alreadySaving);
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
				Notification alreadySaving = new Notification("You are already trying to save a file!", NotificationType.WARNING);
				((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(alreadySaving);
			}
		}
		private StringBuilder SaveTheTileMapToFile(Tilemap p_tileMap)
		{
			StartupDialogSystem sys = (StartupDialogSystem)world.SystemManager.GetSystem<StartupDialogSystem>()[0];
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Width= " + p_tileMap.getColumns());
			sb.AppendLine("Height= " + p_tileMap.getRows());
			sb.AppendLine("TileMapTheme= " + sys.tilemap.Name);
			sb.AppendLine("Data");
			for (int row = 0; row < p_tileMap.getRows(); row++ )
			{
				string resultingRow="";
				for (int col = 0; col < p_tileMap.getColumns(); col++)
				{
					int state = p_tileMap.getState(col, row)+1;
					resultingRow += state + ",";
				}
				sb.AppendLine(resultingRow);
			}
			return sb;
		}
	}
}
