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
					string unlockedMapsPath = ""; //The path used to open the number of unlocked maps

					string[] seperated = completePath.Split('\\');

					for (int i = 0; i < seperated.Length - 1; i++)
					{
						mapArrangerPath += seperated[i] + "\\";
						mapCorrectedPath += seperated[i] + "\\";
						unlockedMapsPath += seperated[i] + "\\";
					}
					mapArrangerPath += "maps.txt";
					unlockedMapsPath += "unlocked.txt";
					mapName = CorrectMapName(seperated[seperated.Length - 1]);
					mapCorrectedPath += mapName;

					if (ExportMapToFile(mapCorrectedPath, mainTilemap))
					{
						SendInfoToNotificationBar("Successfully exported the map!", NotificationType.SUCCESS, "Your map was now successfully exported to " + mapCorrectedPath);

						if (File.Exists(mapArrangerPath) && File.Exists(unlockedMapsPath))
						{
							string[] linesContained = File.ReadAllLines(mapArrangerPath);
							string numberOfUnlocked = File.ReadAllLines(unlockedMapsPath)[0];

							string[] formatedMapName = mapName.Split('.');
							formatedMapName[0] = formatedMapName[0].ToUpper();
							string newMapData = formatedMapName[0] + " " + mapName + " " + 20 + " " + "POL-rescue-short.wav";

							bool isItANewMap = true;
							for (int i = 0; i < linesContained.Length; i++)
							{
								if (newMapData == linesContained[i]) 
								{
									isItANewMap = false;
									break;
								}
							}

							if (isItANewMap)
							{
								SendInfoToNotificationBar("This was a new map so added it to maps.txt", NotificationType.INFO);
								if (Convert.ToInt32(numberOfUnlocked) <= linesContained.Length)
								{
									File.WriteAllText(unlockedMapsPath, (linesContained.Length + 1).ToString()); // Plus one is for the new map being exported
								}
								using (StreamWriter w = File.AppendText(mapArrangerPath))
								{
									w.Write("\r\n");
									w.Write(newMapData);
								}
							}
							else
							{
								SendInfoToNotificationBar("There is already any existing entry of this map.", NotificationType.INFO,"The file maps.txt wasn't changed due to it already containg a file named exactly the same.");
							}
						}
						else
						{
							SendInfoToNotificationBar("Sorry, wasn't able to import the map to The Little Cheese Boy!", NotificationType.WARNING,
								"Unfortunately the file \"maps.txt\" wasn't found in the same folder as the exported map. This means you will have to add it yourself, manually. Error Code: Missing Maps");
						}
					}
					else
					{
						SendInfoToNotificationBar("Failed to export file, please click for more for details.", NotificationType.WARNING,
							"Due to unknown error exportation of the file was unsuccessful and please make sure you run the program as administrator!");
					}
				}
			}
		}
		private void SendInfoToNotificationBar(string p_message, NotificationType p_type, string p_additionalInformation = "")
		{
			Notification newNotification;
			if (p_additionalInformation != "")
			{
				List<Paragraph> info = new List<Paragraph>();
				info.Add(new Paragraph(p_additionalInformation));
				newNotification = new Notification(p_message, p_type, info, "Export");
			}
			else
			{
				newNotification = new Notification(p_message, p_type);
			}
			((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(newNotification);
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
		private string CorrectMapName(string p_mapName)
		{
			string correctedMapName = "";
			correctedMapName = p_mapName.Replace(' ', '_');
			return correctedMapName;
		}
		private bool ExportMapToFile(string p_path, Entity p_mainTileMap)
		{
			Tilemap tilemap = p_mainTileMap.GetComponent<Tilemap>();

			try
			{
				File.WriteAllText(p_path, SaveTheTileMapToFile(tilemap).ToString());
			}
			catch (System.Exception ex)
			{
				requestedToSaveMap = false;
				return false;
			}

			requestedToSaveMap = false;
			return true;
		}
		private StringBuilder SaveTheTileMapToFile(Tilemap p_tileMap)
		{
			StartupDialogSystem sys = (StartupDialogSystem)world.SystemManager.GetSystem<StartupDialogSystem>()[0];
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Width= " + p_tileMap.getColumns());
			sb.AppendLine("Height= " + p_tileMap.getRows());
			sb.AppendLine("TileMapTheme= " + sys.tilemap.Name);
			sb.AppendLine("Data");
			for (int row = 0; row < p_tileMap.getRows(); row++)
			{
				string resultingRow = "";
				for (int col = 0; col < p_tileMap.getColumns(); col++)
				{
					int state = p_tileMap.getState(col, row) + 1;
					resultingRow += state + ",";
				}
				sb.AppendLine(resultingRow);
			}
			return sb;
		}
	}
}
