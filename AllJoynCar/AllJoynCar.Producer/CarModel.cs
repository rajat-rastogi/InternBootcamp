using com.microsoft.Sample.AllJoynCar;
using I2CMotorHAT;

namespace AllJoynCar.Producer
{
    enum Direction
    {
        Stopped = 0,
        Backward = 1,
        Forward = 2,
        Right = 3,
        Left = 4,
    };

    class CarModel
    {
        const uint NUM_MOTORS = 4;
        MotorHAT motorHAT;
        DCMotor[] motors = new DCMotor[NUM_MOTORS];
        AllJoynCarState currentState = new AllJoynCarState();

        public CarModel()
        {
            motorHAT = new MotorHAT();
            currentState.Value2 = (uint)Direction.Stopped;
            currentState.Value1 = false;
            SetSpeed(0);
        }

        public void SetSpeed(uint speed)
        {
            for (uint i = 1; i <= NUM_MOTORS; ++i)
            {
                motorHAT.GetMotor(i).SetSpeed(speed);
            }
        }

        public void Drive(bool isMoving, uint direction)
        {           
                switch ((Direction)direction)
                {
                    case Direction.Stopped:
                        Stop();
                        break;
                    case Direction.Forward:
                        if (!currentState.Value1)
                            GoForward();
                        else
                            System.Diagnostics.Debug.WriteLine("Car is already moving");
                    break;
                    case Direction.Backward:
                        if (!currentState.Value1)
                            GoBackward();
                        else
                            System.Diagnostics.Debug.WriteLine("Car is already moving");
                    break;
                    case Direction.Left:
                        if (!currentState.Value1)
                            GoLeft();
                        else
                            System.Diagnostics.Debug.WriteLine("Car is already moving");
                    break;
                    case Direction.Right:
                        if (!currentState.Value1)
                            GoRight();
                        else
                            System.Diagnostics.Debug.WriteLine("Car is already moving");
                    break;
                    default:
                        Stop();
                        break;
                }
            
            
        }

        private void GoLeft()
        {
            motorHAT.GetMotor(1).Run(Movement.Backward);
            motorHAT.GetMotor(2).Run(Movement.Forward);
            motorHAT.GetMotor(3).Run(Movement.Backward);
            motorHAT.GetMotor(4).Run(Movement.Forward);

            currentState.Value1 = true;
            currentState.Value2 = (uint)Direction.Left;
        }

        private void GoRight()
        {
            motorHAT.GetMotor(1).Run(Movement.Forward);
            motorHAT.GetMotor(2).Run(Movement.Backward);
            motorHAT.GetMotor(3).Run(Movement.Forward);
            motorHAT.GetMotor(4).Run(Movement.Backward);

            currentState.Value1 = true;
            currentState.Value2 = (uint)Direction.Right;
        }

        private void GoForward()
        {
            motorHAT.GetMotor(1).Run(Movement.Backward);
            motorHAT.GetMotor(2).Run(Movement.Forward);
            motorHAT.GetMotor(3).Run(Movement.Forward);
            motorHAT.GetMotor(4).Run(Movement.Backward);

            currentState.Value1 = true;
            currentState.Value2 = (uint)Direction.Forward;
        }

        private void GoBackward()
        {
            motorHAT.GetMotor(1).Run(Movement.Forward);
            motorHAT.GetMotor(2).Run(Movement.Backward);
            motorHAT.GetMotor(3).Run(Movement.Backward);
            motorHAT.GetMotor(4).Run(Movement.Forward);
        }

        private void Stop()
        {
            for (uint i = 1; i <= NUM_MOTORS; ++i)
                motorHAT.GetMotor(i).Run(Movement.Release);
            
            currentState.Value1 = false;
            currentState.Value2 = (uint)Direction.Stopped;
        }
    }
}
