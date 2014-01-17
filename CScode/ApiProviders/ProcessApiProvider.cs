using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Lorei.CScode.Interfaces;

namespace Lorei.CScode.ApiProviders
{
    class ProcessApiProvider : ApiProvider
    {
        /************ Constructors ************/
        public ProcessApiProvider(TextToSpeechApiProvider p_textToSpeechApi)
        {
            m_textToSpeechApi = p_textToSpeechApi;
        }

        /************ Destructor ************/
        /************ Methods ************/
        // Api accessible Program Control Methods
        public void LaunchProgram(String p_program)
        {
            if (!m_runningPrograms.ContainsKey(p_program))
            {
                // If the program isn't running start it
                // Add the program to the dictionary and continue as normal
                try
                {
                    m_runningPrograms.Add(p_program, Process.Start(p_program));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.StackTrace);
                    Console.WriteLine(p_program);
                    m_textToSpeechApi.SayMessage("I cannot find the file");
                }
            }
            else
            {
                if (m_runningPrograms[p_program].HasExited)
                {
                    // Program has exited and can be restarted
                    m_runningPrograms[p_program].Start();
                }
                else
                {
                    // Program is still running and shouldn't me messed with
                    m_textToSpeechApi.SayMessage("Program Is Already Running");
                }
            }

        }
        public void ExitProgram(String p_program)
        {
            // Check and see if the process exists
            Process procToKill;
            if (m_runningPrograms.TryGetValue(p_program, out procToKill))
            {
                // Check to make sure process is still alive.
                procToKill.Refresh();

                if (!procToKill.HasExited)
                {
                    // Close the main window so the program exits
                    procToKill.CloseMainWindow();
                }
                // Then remove the process from the process map so we can
                // launch the program again later.
                m_runningPrograms.Remove(p_program);
            }
            // work done go home 
        }
        public void ExitStubbornProgram(String p_program)
        {
            Process procToKill;
            if (m_runningPrograms.TryGetValue(p_program, out procToKill))
            {
                // Check to make sure process is still alive.
                procToKill.Refresh();

                if (!procToKill.HasExited)
                {
                    // Close the main window so the program exits
                    procToKill.Kill();
                }
                // Then remove the process from the process map so we can
                // launch the program again later.
                m_runningPrograms.Remove(p_program);
            }
        }
        public void SendMessage(String p_program, int p_myMessage, int p_myWParam, int p_myLParam)
        {
            // This is cool
            Process myProcess;
            int myMessage = p_myMessage;
            int myWParam = p_myWParam;
            int myLParam = p_myLParam;

            // Check and make sure the program exists because trying with a bad handle would be bad.....
            if (m_runningPrograms.TryGetValue(p_program, out myProcess))
            {
                // With out this call to refresh the process never updates the information 
                // about windows handles so when a program creates its main window it never changes
                // the information in this class.  Evil Evil Evil thing....
                myProcess.Refresh();  // This command took me days to find gotta love msdn docs

                // Check to make sure our process is still alive
                if (!myProcess.HasExited)
                {
                    // Import the win32 post message function so we can drop messages into the program
                    ProcessApiProvider.PostMessage(myProcess.MainWindowHandle, myMessage, myWParam, myLParam);
                }
            }
            return;
        }

        // Imported Stuff
        // We use post message instead of send message for threading reasons. Post message is Async
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        private static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        /************ Api Provider Interface ************/
        public List<System.Reflection.MethodInfo> GetMethods()
        {
            List<System.Reflection.MethodInfo> methods = new List<System.Reflection.MethodInfo>();

            // Setup the list
            methods.Add(this.GetType().GetMethod("LaunchProgram"));
            methods.Add(this.GetType().GetMethod("ExitProgram"));
            methods.Add(this.GetType().GetMethod("ExitStubbornProgram"));
            methods.Add(this.GetType().GetMethod("SendMessage"));

            return methods;
        }


        /************ Data ************/
        private Dictionary<String, Process> m_runningPrograms = new Dictionary<string, Process>();
        TextToSpeechApiProvider m_textToSpeechApi;
    }
}
