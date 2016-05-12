using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LampModule3
{
    public sealed partial class MainPage : Page
    {
        private LampHelper lampHelper;
        private bool lampFound = false;

        public MainPage()
        {
            this.InitializeComponent();
            hueSlider.Maximum = (double)uint.MaxValue;
            saturationSlider.Maximum = (double)uint.MaxValue;
            brightnessSlider.Maximum = (double)uint.MaxValue;

            lampHelper = new LampHelper();
            lampHelper.LampFound += LampHelper_LampFound;
            lampHelper.LampStateChanged += LampHelper_LampStateChanged;
        }

        private void LampHelper_LampFound(object sender, EventArgs e)
        {
            lampFound = true;
            GetLampState();
        }

        private void LampHelper_LampStateChanged(object sender, EventArgs e)
        {
            GetLampState();
        }

        private void LampSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (lampFound)
            {
                lampHelper.SetOnOffAsync(((ToggleSwitch)sender).IsOn);
            }
        }

        private void SetHue_Clicked(object sender, RoutedEventArgs e)
        {
            if (lampFound)
            {
                lampHelper.SetHueAsync((uint)hueSlider.Value);
            }
        }

        private void SetSaturation_Clicked(object sender, RoutedEventArgs e)
        {
            if (lampFound)
            {
                lampHelper.SetSaturationAsync((uint)saturationSlider.Value);
            }
        }

        private void SetBrightness_Clicked(object sender, RoutedEventArgs e)
        {
            if (lampFound)
            {
                lampHelper.SetBrightnessAsync((uint)brightnessSlider.Value);
            }
        }

        private async void GetLampState()
        {
            if (lampFound)
            {
                // Get the current On/Off state of the lamp.
                toggleSwitch.IsOn = await lampHelper.GetOnOffAsync();

                // Get the current hue, saturation and brightness of the lamp.
                hueSlider.Value = await lampHelper.GetHueAsync();
                saturationSlider.Value = await lampHelper.GetSaturationAsync();
                brightnessSlider.Value = await lampHelper.GetBrightnessAsync();
            }
        }
    }
}
