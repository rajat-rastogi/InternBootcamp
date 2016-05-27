using com.microsoft.Sample.AllJoynCar;
using System;
using System.Windows.Input;
using Windows.Devices.AllJoyn;
using Windows.UI.Core;

namespace AllJoynCar.Consumer
{
    class AllJoynCarViewModel
    {
        private MainPage m_rootPage;
        private CoreDispatcher m_dispatcher;
        private AllJoynBusAttachment m_busAttachment = null;
        private AllJoynCarWatcher m_watcher = null;
        private AllJoynCarConsumer m_consumer = null;
        private AllJoynCarState movingState;
        private uint speed;

        public AllJoynCarViewModel()
        {
            m_dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            m_rootPage = MainPage.Current;

            Start();
        }

        public ICommand LeftPressed
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    GoLeft();
                });
            }
        }

        public ICommand LeftReleased
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    Stop();
                });
            }
        }

        public ICommand RightPressed
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    GoRight();
                });
            }
        }

        public ICommand RightReleased
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    Stop();
                });
            }
        }

        public ICommand ForwardPressed
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    GoForward();
                });
            }
        }

        public ICommand ForwardReleased
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    Stop();
                });
            }
        }

        public ICommand BackwardPressed
        {
            get
            {
                return new RelayCommand((object args) =>
                 {
                     GoBackward();
                 });
            }
        }

        public ICommand BackwardReleased
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    Stop();
                });
            }
        }

        private void Start()
        {
            m_busAttachment = new AllJoynBusAttachment();
            m_watcher = new AllJoynCarWatcher(m_busAttachment);
            m_watcher.Added += Watcher_Added;
            m_watcher.Start();
        }

        public void GoLeft()
        {
            DriveAsync(true, Direction.LEFT);
        }

        public void GoRight()
        {
            DriveAsync(true, Direction.RIGHT);
        }

        public void GoForward()
        {
            DriveAsync(true, Direction.FORWARD);
        }

        public void GoBackward()
        {
            DriveAsync(true, Direction.BACKWARD);
        }

        public void Stop()
        {
            DriveAsync(false, Direction.STOPPED);
        }

        private async void DriveAsync(bool isMoving, Direction dir)
        {
            if (m_consumer != null)
            {
                AllJoynCarDriveResult actionResult = await m_consumer.DriveAsync(isMoving, (uint)dir);

                if (actionResult.Status == AllJoynStatus.Ok)
                {
                    System.Diagnostics.Debug.WriteLine("Method call success");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error during method call");
                }
            }
        }

        public async void UpdateStateAsync()
        {
            AllJoynCarGetStateResult geStateResult = await m_consumer.GetStateAsync();

            if (geStateResult.Status == AllJoynStatus.Ok)
            {
                movingState = geStateResult.State;
            }
        }

        private async void UpdateSpeedAsync()
        {
            AllJoynCarGetSpeedResult getSpeedResult = await m_consumer.GetSpeedAsync();

            if (getSpeedResult.Status == AllJoynStatus.Ok)
                speed = getSpeedResult.Speed;
        }

        public void SetSpeed(uint speed)
        {
            SetSpeedAsync(speed);
        }

        private async void SetSpeedAsync(uint speed)
        {
            if (m_consumer != null)
            {
                AllJoynCarSetSpeedResult result = await m_consumer.SetSpeedAsync(speed);

                if (result.Status == AllJoynStatus.Ok)
                {
                    System.Diagnostics.Debug.WriteLine("Speed successfully updated");
                }
            }
        }

        private async void Watcher_Added(AllJoynCarWatcher sender, AllJoynServiceInfo args)
        {
            AllJoynCarJoinSessionResult joinSessinResult = await AllJoynCarConsumer.JoinSessionAsync(args, sender);

            if (joinSessinResult.Status == AllJoynStatus.Ok)
            {
                DisposeConsumer();
                m_consumer = joinSessinResult.Consumer;
                System.Diagnostics.Debug.WriteLine("Connected to Producer");
                m_consumer.SessionLost += Consumer_SessionLost;

                await m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    speed = Convert.ToUInt32(m_rootPage.sldSpeed.Value);
                });

                SetSpeed(speed);
            }
        }

        private void DisposeConsumer()
        {
            if (m_consumer != null)
            {
                m_consumer.SessionLost -= Consumer_SessionLost;
                m_consumer.Dispose();
                m_consumer = null;
            }
        }

        public void ScenarioCleanup()
        {
            if (m_consumer != null)
            {
                m_consumer.SessionLost -= Consumer_SessionLost;
                m_consumer.Dispose();
                m_consumer = null;
            }

            if (m_busAttachment != null)
            {
                m_busAttachment.Disconnect();
            }
        }

        private void DisposeWatcher()
        {
            if (m_watcher != null)
            {
                m_watcher.Added -= Watcher_Added;
                m_watcher.Stop();
                m_watcher.Dispose();
                m_watcher = null;
            }
        }

        private void Consumer_SessionLost(AllJoynCarConsumer sender, AllJoynSessionLostEventArgs args)
        {
            DisposeConsumer();
        }
    }
}
