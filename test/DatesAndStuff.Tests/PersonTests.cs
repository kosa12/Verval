using NUnit.Framework;
using System;
using FluentAssertions;

namespace DatesAndStuff.Tests
{
    [TestFixture]
    public class PersonTests
    {
        private Person sut;

        [SetUp]
        public void Setup()
        {
        }
        
        [TestFixture]
        public class GotMarriedTests
        {
            private Person sut = PersonFactory.CreateTestPerson();

            [SetUp]
            public void Setup()
            {
            }

            [Test]
            public void GotMarried_First_NameShouldChange()
            {
                // Arrange
                string newName = "Test-Eleso Pista";
                double salaryBeforeMarriage = sut.Salary;

                // Act
                sut.GotMarried(newName);

                // Assert (klasszikus assert)
                sut.Name.Should().Be(newName, "Name should change after marriage.");
                sut.Salary.Should().Be(salaryBeforeMarriage, "Salary should remain unchanged after marriage.");
                
            }

            [Test]
            public void GotMarried_Second_ShouldFail()
            {
                // Arrange
                sut.GotMarried("First Spouse"); // Első házasság

                Action act = () => sut.GotMarried("Second Spouse");
                act.Should().Throw<Exception>()
                    .WithMessage("Poligamy not yet supported.", "Second marriage should throw an exception.");
            
            }
        }

        // Csoportosítás: IncreaseSalary metódus tesztjei
        [TestFixture]
        public class IncreaseSalaryTests
        {
            
            private Person sut;
            [SetUp]
            public void Setup()
            {
                sut = PersonFactory.CreateTestPerson();
            }

            [Test]
            public void IncreaseSalary_PositiveIncrease_ShouldIncrease()
            {
                // Arrange
                double percentage = 10;
                double expectedSalary = sut.Salary * (1 + percentage / 100);

                // Act
                sut.IncreaseSalary(percentage);

                sut.Salary.Should().Be(expectedSalary, "Salary should increase by the given percentage.");
            }

            [Test]
            public void IncreaseSalary_SmallerThanMinusTenPerc_ShouldFail()
            {
                // Arrange
                double percentage = -15;

                Action act = () => sut.IncreaseSalary(percentage);
                act.Should().Throw<ArgumentOutOfRangeException>("Percentage less than -10 should throw an exception.");
            }
            
            [TestCase(-20, 54, false)]
            public void IncreaseSalary_WithVariousPercentages_ShouldHandleCorrectly(double percentage, double expectedSalary, bool shouldSucceed)
            {
                // Arrange
                double initialSalary = sut.Salary;

                if (shouldSucceed)
                {
                    // Act
                    sut.IncreaseSalary(percentage);

                    // Assert
                    sut.Salary.Should().Be(expectedSalary, $"Salary should increase by {percentage}% from {initialSalary}.");
                }
                else
                {
                    // Act & Assert
                    Action act = () => sut.IncreaseSalary(percentage);
                    act.Should().Throw<ArgumentOutOfRangeException>("Percentage less than -10 should throw an exception.");
                    sut.Salary.Should().Be(initialSalary, "Salary should remain unchanged when increase fails.");
                }
            }
        }

        // A meglévő kódhoz képest a CloneTests hiányzott, így azt is hozzáadom
        [TestFixture]
        public class CloneTests
        {
            private Person sut = PersonFactory.CreateTestPerson();

            [SetUp]
            public void Setup()
            {
            }

            [Test]
            public void Clone_ShouldCreateIndependentCopy()
            {
                // Arrange
                var clone = Person.Clone(sut);

                // Act
                clone.IncreaseSalary(10);

                // Assert (klasszikus assert - ez a SimulationTimeTests után került ide az egyensúly miatt)
                sut.Salary.Should().Be(54, "Original salary should remain unchanged.");
                clone.Salary.Should().Be(59.4, "Clone salary should increase independently.");
            }
        }
    }
}