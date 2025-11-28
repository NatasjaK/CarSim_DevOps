using DataLogicLibrary.DTO;
using DataLogicLibrary.Infrastructure.Enums;
using DataLogicLibrary.Services;
using DataLogicLibrary.DirectionStrategies;
using DataLogicLibrary.DirectionStrategies.Interfaces;
using Xunit;

namespace CarSimulator.Tests
{
    public class SimulationLogicServiceTests
    {
        private static SimulationLogicService.DirectionStrategyResolver CreateRealResolver()
        {
            return movementAction =>
                movementAction switch
                {
                    MovementAction.Left => new TurnLeftStrategy(),
                    MovementAction.Right => new TurnRightStrategy(),
                    MovementAction.Forward => new DriveForwardStrategy(),
                    MovementAction.Backward => new ReverseStrategy(),
                    _ => new DummyDirectionStrategy()
                };
        }

        [Fact]
        public void PerformAction_WithRefuelAction_SetsGasTo20()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { GasValue = 0, EnergyValue = 10 };

            // Act
            var result = service.PerformAction(6, status);

            // Assert
            Assert.Equal(20, result.GasValue);
        }

        [Fact]
        public void DecreaseStatusValues_EnergyAlwaysDecreasesOrStaysZero()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { GasValue = 10, EnergyValue = 10 };

            // Act
            var result = service.DecreaseStatusValues(3, status); // 3 = Drive forward

            // Assert
            Assert.InRange(result.EnergyValue, 0, 9); // 10 - (1..5)
        }

        [Fact]
        public void DecreaseStatusValues_GasDecreasesWhenNotResting()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { GasValue = 10, EnergyValue = 10 };

            // Act
            var result = service.DecreaseStatusValues(3, status); // 3 != 5

            // Assert
            Assert.InRange(result.GasValue, 5, 9); // 10 - (1..5)
        }

        [Fact]
        public void PerformAction_TurnLeft_FromNorth_SetsDirectionToWest()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { CardinalDirection = CardinalDirection.North, GasValue = 10, EnergyValue = 10 };

            // Act
            var result = service.PerformAction(1, status); // 1 = Turn left

            // Assert
            Assert.Equal(CardinalDirection.West, result.CardinalDirection);
            Assert.Equal(MovementAction.Left, result.MovementAction);
        }

        [Fact]
        public void PerformAction_TurnRight_FromNorth_SetsDirectionToEast()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { CardinalDirection = CardinalDirection.North, GasValue = 10, EnergyValue = 10 };

            // Act
            var result = service.PerformAction(2, status); // 2 = Turn right

            // Assert
            Assert.Equal(CardinalDirection.East, result.CardinalDirection);
            Assert.Equal(MovementAction.Right, result.MovementAction);
        }

        [Fact]
        public void PerformAction_TurnLeft_FromEast_SetsDirectionToNorth()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { CardinalDirection = CardinalDirection.East, GasValue = 10, EnergyValue = 10 };

            // Act
            var result = service.PerformAction(1, status); // 1 = Turn left

            // Assert
            Assert.Equal(CardinalDirection.North, result.CardinalDirection);
            Assert.Equal(MovementAction.Left, result.MovementAction);
        }

        [Fact]
        public void PerformAction_Reverse_FromNorth_SetsDirectionToSouth()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { CardinalDirection = CardinalDirection.North, GasValue = 10, EnergyValue = 10 };

            // Act
            var result = service.PerformAction(4, status); // 4 = Reverse

            // Assert
            Assert.Equal(CardinalDirection.South, result.CardinalDirection);
            Assert.Equal(MovementAction.Backward, result.MovementAction);
        }

        [Fact]
        public void PerformAction_DriveForward_SetsMovementToForward_AndKeepsDirection()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            var resolver = CreateRealResolver();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO
            {
                CardinalDirection = CardinalDirection.East,
                MovementAction = MovementAction.Left,
                GasValue = 10,
                EnergyValue = 10
            };

            // Act
            var result = service.PerformAction(3, status); // 3 = Drive forward

            // Assert
            Assert.Equal(CardinalDirection.East, result.CardinalDirection);
            Assert.Equal(MovementAction.Forward, result.MovementAction);
        }
    }

    public class DummyDirectionContext : IDirectionContext
    {
        private IDirectionStrategy? _strategy;

        public void SetStrategy(IDirectionStrategy strategy)
        {
            _strategy = strategy;
        }

        public StatusDTO ExecuteStrategy(StatusDTO status)
        {
            return _strategy == null ? status : _strategy.Execute(status);
        }
    }

    public class DummyDirectionStrategy : IDirectionStrategy
    {
        public StatusDTO Execute(StatusDTO status) => status;
    }
}
