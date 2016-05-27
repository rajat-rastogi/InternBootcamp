using com.microsoft.Sample.AllJoynCar;
using System;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Foundation;

namespace AllJoynCar.Producer
{
    class AllJoynCarService : IAllJoynCarService
    {
        CarModel car = null;

        public AllJoynCarService()
        {
            car = new CarModel();
            StopMotors();
        }

        public void StopMotors()
        {
            if (car != null)
                car.Drive(false, (uint)Direction.Stopped);
        }

        public IAsyncOperation<AllJoynCarDriveResult> DriveAsync(AllJoynMessageInfo info, bool isMoving, uint direction)
        {
            Task<AllJoynCarDriveResult> task = new Task<AllJoynCarDriveResult>(() =>
            {
                car.Drive(isMoving, direction);
                return AllJoynCarDriveResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<AllJoynCarGetStateResult> GetStateAsync(AllJoynMessageInfo info)
        {
            Task<AllJoynCarGetStateResult> task = new Task<AllJoynCarGetStateResult>(() =>
            {
                return AllJoynCarGetStateResult.CreateSuccessResult(AppData.State);
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<AllJoynCarGetSpeedResult> GetSpeedAsync(AllJoynMessageInfo info)
        {
            Task<AllJoynCarGetSpeedResult> task = new Task<AllJoynCarGetSpeedResult>(() =>
            {
                return AllJoynCarGetSpeedResult.CreateSuccessResult(AppData.Speed);
            });

            task.Start();

            return task.AsAsyncOperation();
        }
        
        IAsyncOperation<AllJoynCarSetSpeedResult> IAllJoynCarService.SetSpeedAsync(AllJoynMessageInfo info, uint value)
        {
            Task<AllJoynCarSetSpeedResult> task = new Task<AllJoynCarSetSpeedResult>(() =>
            {
                car.SetSpeed(value);
                return AllJoynCarSetSpeedResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
