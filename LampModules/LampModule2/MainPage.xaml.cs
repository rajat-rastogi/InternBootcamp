using org.allseen.LSF.LampState;
using System;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LampModule2
{
    public sealed partial class MainPage : Page
    {
        private AllJoynBusAttachment busAttachment = null;
        private LampStateConsumer consumer = null;
        private string lampDeviceId = "3eb3073dcc98ec532af81ea929aa9462";

        public MainPage()
        {
            this.InitializeComponent();
            hueSlider.Maximum = (double)uint.MaxValue;
            saturationSlider.Maximum = (double)uint.MaxValue;
            brightnessSlider.Maximum = (double)uint.MaxValue;

            // Initialize AllJoyn bus attachment.
            busAttachment = new AllJoynBusAttachment();

            // Initialize LampState watcher.
            LampStateWatcher watcher = new LampStateWatcher(busAttachment);

            // Subscribe to watcher added event - to watch for any producers with LampState interface.
            watcher.Added += Watcher_Added;

            // Start the LampState watcher.
            watcher.Start();
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
                    GetLampState();
                }
            }
        }

        private async void LampSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (consumer != null)
            {
                await consumer.SetOnOffAsync(((ToggleSwitch)sender).IsOn);
            }
        }

        private async void SetHue_Clicked(object sender, RoutedEventArgs e)
        {
            if (consumer != null)
            {
                await consumer.SetHueAsync((uint)hueSlider.Value);
            }
        }

        private async void SetSaturation_Clicked(object sender, RoutedEventArgs e)
        {
            if (consumer != null)
            {
                await consumer.SetSaturationAsync((uint)saturationSlider.Value);
            }
        }

        private async void SetBrightness_Clicked(object sender, RoutedEventArgs e)
        {
            if (consumer != null)
            {
                await consumer.SetBrightnessAsync((uint)brightnessSlider.Value);
            }
        }

        private async void GetLampState()
        {
            if (consumer != null)
            {
                // Get the current On/Off state of the lamp.
                LampStateGetOnOffResult onOffResult = await consumer.GetOnOffAsync();
                if (onOffResult.Status == AllJoynStatus.Ok)
                {
                    toggleSwitch.IsOn = onOffResult.OnOff;
                }

                // Get the current hue, saturation and brightness of the lamp.
                LampStateGetHueResult hueResult = await consumer.GetHueAsync();
                if (hueResult.Status == AllJoynStatus.Ok)
                {
                    hueSlider.Value = (double)hueResult.Hue;
                }

                LampStateGetSaturationResult saturationResult = await consumer.GetSaturationAsync();
                if (saturationResult.Status == AllJoynStatus.Ok)
                {
                    saturationSlider.Value = (double)saturationResult.Saturation;
                }

                LampStateGetBrightnessResult brightnessResult = await consumer.GetBrightnessAsync();
                if (brightnessResult.Status == AllJoynStatus.Ok)
                {
                    brightnessSlider.Value = (double)brightnessResult.Brightness;
                }
            }
        }
    }
}
