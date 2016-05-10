using org.allseen.LSF.LampState;
using System;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LampModule1
{
    public sealed partial class MainPage : Page
    {
        private AllJoynBusAttachment busAttachment = null;
        private LampStateConsumer consumer = null;
        private string lampDeviceId = "3eb3073dcc98ec532af81ea929aa9462";

        public MainPage()
        {
            this.InitializeComponent();

            // Initialize AllJoyn bus attachment.
            busAttachment = new AllJoynBusAttachment();

            // Initialize LampState watcher.
            LampStateWatcher watcher = new LampStateWatcher(busAttachment);

            // Subscribe to watcher added event - to watch for any producers with LampState interface.
            watcher.Added += Watcher_Added;

            // Start the LampState watcher.
            watcher.Start();
        }

        private async void LampSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (consumer != null)
            {
                await consumer.SetOnOffAsync(((ToggleSwitch)sender).IsOn);
            }
        }

        private async void Watcher_Added(LampStateWatcher sender, AllJoynServiceInfo args)
        {
            AllJoynAboutDataView aboutData = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, busAttachment, args.SessionPort);

            if (aboutData != null && !string.IsNullOrWhiteSpace(aboutData.DeviceId) && string.Equals(aboutData.DeviceId, lampDeviceId))
            {
                // Join session with the producer of the LampState interface.
                LampStateJoinSessionResult joinSessionResult = await LampStateConsumer.JoinSessionAsync(args, sender);

                if (joinSessionResult.Status == AllJoynStatus.Ok)
                {
                    consumer = joinSessionResult.Consumer;

                    // Get the current On/Off state of the lamp.
                    LampStateGetOnOffResult onOffResult = await consumer.GetOnOffAsync();
                    if (onOffResult.Status == AllJoynStatus.Ok)
                    {
                        toggleSwitch.IsOn = onOffResult.OnOff;
                    }
                }
            }
        }
    }
}
