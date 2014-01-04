import clr
clr.AddReference('System.Windows.Forms')
import System.Windows.Forms
import LoreiApi

def Simple():
	System.Windows.Forms.MessageBox.Show("Hello World")
	LoreiApi.SayMessage("Hello From Python")