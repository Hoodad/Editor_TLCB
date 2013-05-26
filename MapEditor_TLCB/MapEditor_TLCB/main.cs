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
			using (Editor game = new Editor(args))
			{
				game.Run();
			}
		}
	}
#endif
}