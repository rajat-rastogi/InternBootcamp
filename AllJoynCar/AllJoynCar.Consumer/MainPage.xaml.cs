using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AllJoynCar.Consumer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        AllJoynCarViewModel m_allJoynViewModel = null;

        public MainPage()
        {
            Current = this;
            m_allJoynViewModel = new AllJoynCarViewModel();

            DataContext = m_allJoynViewModel;
            this.InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (m_allJoynViewModel != null)
            {
                m_allJoynViewModel.ScenarioCleanup();
            }
        }

        private void GridLoaded(object sender, RoutedEventArgs e)
        {
            btnForward.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(btnForward_PointerPressed), true);
            btnForward.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(btn_PointerReleased), true);
            btnBackward.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(btnBackward_PointerPressed), true);
            btnBackward.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(btn_PointerReleased), true);
            btnLeft.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(btnLeft_PointerPressed), true);
            btnLeft.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(btn_PointerReleased), true);
            btnRight.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(btnRight_PointerPressed), true);
            btnRight.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(btn_PointerReleased), true);
        }

        private void btnForward_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            m_allJoynViewModel.GoForward();
        }

        private void btn_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            m_allJoynViewModel.Stop();
        }

        private void btnRight_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            m_allJoynViewModel.GoRight();
        }

        private void btnBackward_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            m_allJoynViewModel.GoBackward();
        }

        private void btnLeft_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            m_allJoynViewModel.GoLeft();
        }

        private void sldSpeed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            m_allJoynViewModel.SetSpeed(Convert.ToUInt32(sldSpeed.Value));
        }
    }
}
