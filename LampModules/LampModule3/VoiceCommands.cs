using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace LampModule3
{
    public class VoiceCommands
    {
        private static bool onStateChangeRequested = false;


        public async static void RegisterVoiceCommands()
        {
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///LampVoiceCommands.xml"));
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }

        public static void ProcessVoiceCommand(VoiceCommandActivatedEventArgs eventArgs)
        {
            switch (eventArgs.Result.RulePath[0])
            {
                case "ToggleLamp":
                    string switchableStateChange = eventArgs.Result.SemanticInterpretation.Properties["switchableStateChange"][0];
                    if (string.Equals(switchableStateChange, "on", StringComparison.OrdinalIgnoreCase))
                    {
                        onStateChangeRequested = true;
                    }
                    else
                    {
                        onStateChangeRequested = false;
                    }
                    LampHelper lampHelper = new LampHelper();
                    lampHelper.LampFound += LampHelper_LampFound;
                    break;
                default:
                    break;
            }
        }

        private static void LampHelper_LampFound(object sender, EventArgs e)
        {
            LampHelper lampHelper = sender as LampHelper;
            lampHelper.SetOnOffAsync(onStateChangeRequested);
        }
    }
}
