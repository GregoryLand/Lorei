import clr
import LoreiApi

def ParseSpeech(e) :
    ParseSpeech.counter += 1

    # Open the log file
    logfile = open('logfile.txt', 'a')

    # Print Header if we just started running
    if ParseSpeech.counter == 1:
        logfile.write( '-----------------------Lorei Started -----------------------\n' )
   
    # Write out all speech events
    logfile.write( e.Result.Text )
    logfile.write( '\n' )

    # close logfile and and set flag so we dont put header in again
    logfile.close()
    FirstLoad = 'false';

ParseSpeech.counter = 0