using com.microsoft.Sample.AllJoynCar;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AllJoynCar.Producer
{
    public sealed partial class MainPage : Page
    {
        private AllJoynBusAttachment m_busAttachment = null;
        private AllJoynCarProducer m_producer = null;

        public MainPage()
        {
            this.InitializeComponent();
            m_busAttachment = new AllJoynBusAttachment();
            m_producer = new AllJoynCarProducer(m_busAttachment);
            m_producer.Service = new AllJoynCarService();
            m_producer.Start();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (m_producer != null)
            {
                ((AllJoynCarService)m_producer.Service).StopMotors();

                m_producer.Stop();
                m_producer.Dispose();
            }

            if (m_busAttachment != null)
            {
                m_busAttachment.Disconnect();
            }
        }
    }
}
