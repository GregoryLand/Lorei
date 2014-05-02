using Gtk;

namespace Lorei.Forms
{
    class MainWindow : Window
    {
        public MainWindow() : base("L.O.R.E.I.")
        {
            SetDefaultSize(300, 150);
            SetPosition(WindowPosition.Center);
            this.Name = "Main Window";

            Fixed positions = new Fixed();

            Label LastCommand = new Label("Fuck you");
            Label StateText = new Label("Enabled");
            Button EnableButton = new Button("Enable/Disable");

            EnableButton.SetSizeRequest(150, 75);

            positions.Put(StateText, 50, 55);
            positions.Put(EnableButton, 140, 10);
            positions.Put(LastCommand, 20, 100);
            Add(positions);
            DeleteEvent += delegate { Application.Quit(); };
        
            ShowAll();    
        }
    }
}
