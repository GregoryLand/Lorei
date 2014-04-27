using System;
using Gtk;
using Lorei.Forms;

namespace Lorei
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			Application.Run ();
		}
	}
}
