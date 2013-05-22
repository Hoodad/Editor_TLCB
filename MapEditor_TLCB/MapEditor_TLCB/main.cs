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
			bool useMaxiumRes = false;

			foreach (string arg in args)
			{
				if (arg == "maximizeWindow")
				{
					useMaxiumRes = true;
				}
			}
			using (Editor game = new Editor(false, useMaxiumRes))
			{
				game.Run();
			}
		}
	}
#endif
}