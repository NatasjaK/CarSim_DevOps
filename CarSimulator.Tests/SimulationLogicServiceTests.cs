using Xunit;
using DataLogicLibrary.DTO;
using DataLogicLibrary.Services;

namespace CarSimulator.Tests
{
    public class SimulationLogicServiceTests
    {
        [Fact]
        public void PerformAction_WithRefuelAction_SetsGasTo20()
        {
            // Arrange
            var dummyDirectionContext = new DummyDirectionContext();
            SimulationLogicService.DirectionStrategyResolver resolver = _ => new DummyDirectionStrategy();
            var service = new SimulationLogicService(dummyDirectionContext, resolver);
            var status = new StatusDTO { GasValue = 0, EnergyValue = 10 };

            // Act
            var result = service.PerformAction(6, status);

            // Assert
            Assert.Equal(20, result.GasValue);
        }
    }

    public class DummyDirectionContext : DataLogicLibrary.DirectionStrategies.Interfaces.IDirectionContext
    {
        public void SetStrategy(DataLogicLibrary.DirectionStrategies.Interfaces.IDirectionStrategy strategy) { }
        public StatusDTO ExecuteStrategy(StatusDTO status) => status;
    }

    public class DummyDirectionStrategy : DataLogicLibrary.DirectionStrategies.Interfaces.IDirectionStrategy
    {
        public StatusDTO Execute(StatusDTO status) => status;
    }
}
