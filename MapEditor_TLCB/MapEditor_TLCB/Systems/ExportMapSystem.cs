using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Artemis;
using MapEditor_TLCB.Components;
using System.Text.RegularExpressions;

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
					string mapName;
					string mapArrangerPath = ""; //Used to access maps.tx
					string mapCorrectedPath = ""; //Will contain the corrected name of the newly exported map
					string[] seperated = completePath.Split('\\');

					for (int i = 0; i < seperated.Length - 1; i++)
					{
						mapArrangerPath += seperated[i] + "\\";
						mapCorrectedPath += seperated[i] + "\\";
					}
					mapArrangerPath += "maps.txt";
					mapCorrectedPath += seperated[seperated.Length - 1].Replace(' ', '_');

					mapName = seperated[seperated.Length - 1].Replace(' ', '_');

					Tilemap tilemap = mainTilemap.GetComponent <Tilemap>();

					File.WriteAllText(mapCorrectedPath, SaveTheTileMapToFile(tilemap).ToString());
					requestedToSaveMap = false;

					List<Paragraph> exportedInfo = new List<Paragraph>();
					exportedInfo.Add(new Paragraph("Your map was now successfully exported to "+ mapCorrectedPath));
					Notification alreadySaving = new Notification("Successfully exported the map!", NotificationType.SUCCESS, exportedInfo);
					((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(alreadySaving);

					if (File.Exists(mapArrangerPath))
					{
						using (StreamWriter w = File.AppendText(mapArrangerPath))
						{
							w.Write("\r\n");
							string[] formatedMapName = mapName.Split('.');
							formatedMapName[0] = formatedMapName[0].ToUpper();
							w.Write(formatedMapName[0] + " " + mapName + " " + 20 + " " + "POL-rescue-short.wav");
						}
					}
					else
					{
						List<Paragraph> info = new List<Paragraph>();
						info.Add(new Paragraph("Unfortunately the file \"maps.txt\" wasn't found in the same folder as the exported map. This means you will have to add it yourself, manually. Error Code: Missing Maps"));
						Notification unableFindMapFile = new Notification("Sorry, wasn't able to import the map to The Little Cheese Boy!", NotificationType.WARNING, info);
						((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(unableFindMapFile);
					}
					
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
