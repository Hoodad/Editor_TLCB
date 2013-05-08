using System;

namespace MapEditor_TLCB
{
#if WINDOWS || XBOX
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		
		[STAThreadAttribute] // Needed in order to open dialogs using windows forms
		static void Main(string[] args)
		{
			bool useFullscreen = false;
			bool useMaxiumRes = false;

			foreach (string arg in args)
			{
				if (arg == "useFullscreen")
				{
					useFullscreen = true;
				}
				else if (arg == "useMaxResolution")
				{
					useMaxiumRes = true;
				}
			}
			using (Editor game = new Editor(useFullscreen, useMaxiumRes))
			{
				game.Run();
			}
		}
	}
#endif
}