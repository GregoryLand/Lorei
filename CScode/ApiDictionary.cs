using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lorei
{
    //Added to hold the apps and make it easier to pass them. 
    public class ApiDictionary
    { 
        public ApiDictionary()
        {
            TextToSpeechApiProvider textToSpeechApiProvider = new TextToSpeechApiProvider();
            ProcessApiProvider processorApiProvider = new ProcessApiProvider(textToSpeechApiProvider);
            LoggingApiProvider loggingApiProvider = new LoggingApiProvider();

            apiDictionary = new Dictionary<string,ApiProvider>();

            apiDictionary.Add("TextToSpeechApi", textToSpeechApiProvider);
            apiDictionary.Add("ProcessApi", processorApiProvider);
            apiDictionary.Add("LoggingApi", loggingApiProvider);
        }

        public ApiProvider getApiProvider(String apiKey)
        {
            if (apiDictionary.ContainsKey(apiKey))
                return apiDictionary[apiKey]; 
            else
                return null; 
        }

        public Dictionary<String, ApiProvider> apiDictionary;
    }
}
