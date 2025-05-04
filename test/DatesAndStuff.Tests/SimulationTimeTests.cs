using FluentAssertions;

namespace DatesAndStuff.Tests
{
    public sealed class SimulationTimeTests
    {
        [OneTimeSetUp]
        public void OneTimeSetupStuff()
        {
            // 
        }

        [SetUp]
        public void Setup()
        {
            // minden teszt felteheti, hogz elotte lefutott ez
        }

        [TearDown]
        public void TearDown()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        // Csoportosítás: Konstruktorok és alapvető működés
        [TestFixture]
        public class ConstructionTests
        {
            [Test]
            public void SimulationTime_ShouldNotUseCurrentTimeByDefault()
            {
                SimulationTime time = new SimulationTime(new DateTime(2025, 3, 18, 12, 0, 0));
                
                time.ToAbsoluteDateTime().Should().NotBe(DateTime.Now, "SimulationTime should not default to current time.");
            }

            [Test]
            public void SimulationTime_ShouldRepresentOneMillisecondPerTick()
            {
                SimulationTime time = new SimulationTime(new DateTime(2025, 1, 1, 0, 0, 0));
                SimulationTime nextTick = time.NextLogicalTick;
                
                long tickDifference = nextTick.LogicalTicks - time.LogicalTicks;
                
                tickDifference.Should().Be(1, "Each logical tick should represent a minimal increment.");
            }
        }
        
        // Csoportosítás: Operátorok
        [TestFixture]
        public class OperatorTests
        {
            // equal
            // not equal
            // <
            // >
            // <= different
            // >= different 
            // <= same
            // >= same
            // max
            // min
            [Test]
            public void SimulationTime_ShouldHandleComparisonOperatorsCorrectly()
            {
                SimulationTime time1 = new SimulationTime(new DateTime(2025, 1, 1, 12, 0, 0));
                SimulationTime time2 = new SimulationTime(new DateTime(2025, 1, 1, 12, 0, 1));
                
                // classical model
                (time1 < time2).Should().BeTrue("time1 should be less than time2.");
                (time1 > time2).Should().BeFalse("time1 should not be greater than time2.");
                (time1 <= time2).Should().BeTrue("time1 should be less than or equal to time2.");
                (time1 >= time2).Should().BeFalse("time1 should not be greater than or equal to time2.");
            }

            [Test]
            public void GivenTwoSimulationTimes_WhenSubtracting_ThenResultMatchesDateTimeDifference()
            {
                throw new NotImplementedException();
            }

            [Test]
            public void SimulationTime_EqualityOperators_ShouldWorkCorrectly()
            {
                DateTime date1 = new DateTime(2025, 1, 1, 12, 0, 0);
                DateTime date2 = new DateTime(2025, 1, 1, 12, 0, 0);
                DateTime date3 = new DateTime(2025, 1, 1, 12, 0, 1);
                
                SimulationTime time1 = new SimulationTime(date1);
                SimulationTime time2 = new SimulationTime(date2);
                SimulationTime time3 = new SimulationTime(date3);
                
                // Act & Assert
                // Egyenlő esetek
                (time1 == time2).Should().BeTrue("Two SimulationTime instances with the same TotalMilliseconds should be equal using '=='.");
                (time1 != time2).Should().BeFalse("Two SimulationTime instances with the same TotalMilliseconds should not be unequal using '!='.");
                time1.Equals(time2).Should().BeTrue("Equals should return true for two SimulationTime instances with the same TotalMilliseconds.");

                (time1 == time3).Should().BeFalse("Two SimulationTime instances with different TotalMilliseconds should not be equal using '=='.");
                (time1 != time3).Should().BeTrue("Two SimulationTime instances with different TotalMilliseconds should be unequal using '!='.");
                time1.Equals(time3).Should().BeFalse("Equals should return false for two SimulationTime instances with different TotalMilliseconds.");
            }
        }
        
        // Csoportosítás: Időmanipulációs metódusok
        [TestFixture]
        public class TimeManipulationTests
        {
            [Test]
            public void SimulationTime_ShouldCalculateNextMillisecondCorrectly()
            {
                SimulationTime time = new SimulationTime(new DateTime(2025, 1, 1, 0, 0, 0));
                
                SimulationTime nextMs = time.NextMillisec;
                
                nextMs.TotalMilliseconds.Should().Be(time.TotalMilliseconds + 1, "NextMillisec should increment TotalMilliseconds by 1.");
            }

            [Test]
            public void GivenSimulationTime_WhenAddingMilliseconds_ThenResultMatchesDateTime()
            {
                throw new NotImplementedException();
            }

            [Test]
            public void SimulationTime_ShouldAddSecondsCorrectly()
            {
                throw new NotImplementedException();
            }

            [Test]
            public void GivenSimulationTime_WhenAddingTimeSpan_ThenResultIsCorrect()
            {
                throw new NotImplementedException();
            }
        }
        
        // Csoportosítás: ToString és konverziók
        [TestFixture]
        public class ConversionTests
        {
            [Test]
            public void SimulationTime_ShouldReturnCorrectStringRepresentation()
            {
                throw new NotImplementedException();
            }
        }
        
        [TestFixture]
        private class TimeSpanArithmeticTests
        {

            [Test]
            // TimeSpanArithmetic
            // add
            // substract
            // Given_When_Then
            public void GivenSimulationTime_WhenAddingTimeSpan_ThenTimeIsShifted()
            {
                // UserSignedIn_OrderSent_OrderIsRegistered
                // DBB, specflow, cucumber, gherkin

                // Arrange
                DateTime baseDate = new DateTime(2010, 8, 23, 9, 4, 49);
                SimulationTime sut = new SimulationTime(baseDate);

                var ts = TimeSpan.FromMilliseconds(4544313);

                // Act
                var result = sut + ts;

                // Assert
                var expectedDateTime = baseDate + ts;
                Assert.AreEqual(expectedDateTime, result.ToAbsoluteDateTime());
            }

            [Test]
            //Method_Should_Then
            public void Subtract_ShouldShiftSimulationTimeBackwards()
            {
                // code kozelibb
                // RegisterOrder_SignedInUserSendsOrder_OrderIsRegistered
                throw new NotImplementedException();
            }
        }
    }
}