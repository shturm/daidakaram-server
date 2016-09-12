using System;
using Gtk;

namespace LegacyImporter
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			var session = FNHibernateConfiguration.OpenSession ();

			Application.Init ();
			MainWindow win = new MainWindow (session);
			win.Show ();
			Application.Run ();


		}
	}
}