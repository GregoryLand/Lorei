using Gtk;

namespace Lorei.Forms
{
    class MainWindow : Window
    {
        public MainWindow() : base("Language Operated Request and Execution Interface")
        {
            SetDefaultSize(500, 200);
            SetPosition(WindowPosition.Center);
            this.Name = "Main Window";

            Button EnableButton = new Button();
            EnableButton.


            DeleteEvent += delegate { Application.Quit(); };
        
            ShowAll();    
        }
    }
}
