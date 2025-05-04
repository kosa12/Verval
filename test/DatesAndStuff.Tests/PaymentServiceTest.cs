using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DatesAndStuff.Tests
{
    internal class PaymentServiceTest
    {
        [Test]
        public void TestPaymentService_ManualMock_BalanceGreaterThanFee_ShouldSucceed()
        {
            // Arrange
            var paymentService = new TestPaymentService(600);
            Person sut = new Person("Test Pista",
                new EmploymentInformation(
                    54,
                    new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentService,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams()
                {
                    CanEatChocolate = true,
                    CanEatEgg = true,
                    CanEatLactose = true,
                    CanEatGluten = true
                }
            );

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeTrue("Payment should succeed when balance exceeds the fee.");
        }

        [Test]
        public void TestPaymentService_ManualMock_BalanceLessThanFee_ShouldFail()
        {
            // Arrange
            var paymentService = new TestPaymentService(400);
            Person sut = new Person("Test Pista",
                new EmploymentInformation(
                    54,
                    new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentService,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams()
                {
                    CanEatChocolate = true,
                    CanEatEgg = true,
                    CanEatLactose = true,
                    CanEatGluten = true
                }
            );

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeFalse("Payment should fail and cancel when balance is less than the fee.");
        }

        [Test]
        public void TestPaymentService_Mock_BalanceGreaterThanFee_ShouldSucceed()
        {
            // Arrange
            var paymentSequence = new MockSequence();
            var paymentService = new Mock<IPaymentService>();
            paymentService.InSequence(paymentSequence).Setup(m => m.StartPayment());
            paymentService.Setup(m => m.GetBalance()).Returns(600);
            paymentService.InSequence(paymentSequence).Setup(m => m.SpecifyAmount(Person.SubscriptionFee));
            paymentService.InSequence(paymentSequence).Setup(m => m.ConfirmPayment());
            paymentService.Setup(m => m.SuccessFul()).Returns(true);

            Person sut = new Person("Test Pista",
                new EmploymentInformation(
                    54,
                    new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentService.Object,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams()
                {
                    CanEatChocolate = true,
                    CanEatEgg = true,
                    CanEatLactose = true,
                    CanEatGluten = true
                }
            );

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeTrue("Payment should succeed when balance exceeds the fee.");
            paymentService.Verify(m => m.StartPayment(), Times.Once());
            paymentService.Verify(m => m.GetBalance(), Times.Once());
            paymentService.Verify(m => m.SpecifyAmount(Person.SubscriptionFee), Times.Once());
            paymentService.Verify(m => m.ConfirmPayment(), Times.Once());
            paymentService.Verify(m => m.SuccessFul(), Times.Once());
            paymentService.Verify(m => m.Cancel(), Times.Never());
        }

        [Test]
        public void TestPaymentService_Mock_BalanceLessThanFee_ShouldCancel()
        {
            // Arrange
            var paymentSequence = new MockSequence();
            var paymentService = new Mock<IPaymentService>();
            paymentService.InSequence(paymentSequence).Setup(m => m.StartPayment());
            paymentService.Setup(m => m.GetBalance()).Returns(400);
            paymentService.InSequence(paymentSequence).Setup(m => m.Cancel());

            Person sut = new Person("Test Pista",
                new EmploymentInformation(
                    54,
                    new Employer("RO1234567", "Valami city valami hely", "Dagobert bacsi", new List<int>() { 6201, 7210 })),
                paymentService.Object,
                new LocalTaxData("4367558"),
                new FoodPreferenceParams()
                {
                    CanEatChocolate = true,
                    CanEatEgg = true,
                    CanEatLactose = true,
                    CanEatGluten = true
                }
            );

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeFalse("Payment should cancel when balance is less than the fee.");
            paymentService.Verify(m => m.StartPayment(), Times.Once());
            paymentService.Verify(m => m.GetBalance(), Times.Once());
            paymentService.Verify(m => m.Cancel(), Times.Once());
            paymentService.Verify(m => m.SpecifyAmount(It.IsAny<double>()), Times.Never());
            paymentService.Verify(m => m.ConfirmPayment(), Times.Never());
            paymentService.Verify(m => m.SuccessFul(), Times.Never());
        }

        [Test]
        [CustomPersonCreationAutodataAttribute]
        public void TestPaymentService_MockWithAutodata(Person sut, Mock<IPaymentService> paymentService)
        {
            // Arrange

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeTrue();
            paymentService.Verify(m => m.StartPayment(), Times.Once);
            paymentService.Verify(m => m.SpecifyAmount(Person.SubscriptionFee), Times.Once);
            paymentService.Verify(m => m.ConfirmPayment(), Times.Once);
        }
        
        [Test]
        [CustomPersonCreationAutodataAttribute]
        public void TestPaymentService_MockWithAutodata_BalanceGreaterThanFee_ShouldSucceed(Person sut, Mock<IPaymentService> paymentService)
        {
            // Arrange
            paymentService.Setup(m => m.GetBalance()).Returns(600);
            paymentService.Setup(m => m.SuccessFul()).Returns(true);
            sut = new Person(sut.Name, sut.Employment, paymentService.Object, sut.TaxData, new FoodPreferenceParams
            {
                CanEatGluten = sut.CanEatGluten,
                CanEatLactose = sut.CanEatLactose,
                CanEatEgg = sut.CanEatEgg,
                CanEatChocolate = sut.CanEatChocolate
            });

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeTrue();
            paymentService.Verify(m => m.StartPayment(), Times.Once());
            paymentService.Verify(m => m.GetBalance(), Times.Once());
            paymentService.Verify(m => m.SpecifyAmount(Person.SubscriptionFee), Times.Once());
            paymentService.Verify(m => m.ConfirmPayment(), Times.Once());
            paymentService.Verify(m => m.SuccessFul(), Times.Once());
        }

        [Test]
        [CustomPersonCreationAutodataAttribute]
        public void TestPaymentService_MockWithAutodata_BalanceLessThanFee_ShouldCancel(Person sut, Mock<IPaymentService> paymentService)
        {
            // Arrange
            paymentService.Setup(m => m.GetBalance()).Returns(400);
            sut = new Person(sut.Name, sut.Employment, paymentService.Object, sut.TaxData, new FoodPreferenceParams
            {
                CanEatGluten = sut.CanEatGluten,
                CanEatLactose = sut.CanEatLactose,
                CanEatEgg = sut.CanEatEgg,
                CanEatChocolate = sut.CanEatChocolate
            });

            // Act
            bool result = sut.PerformSubsriptionPayment();

            // Assert
            result.Should().BeFalse();
            paymentService.Verify(m => m.StartPayment(), Times.Once());
            paymentService.Verify(m => m.GetBalance(), Times.Once());
            paymentService.Verify(m => m.Cancel(), Times.Once());
            paymentService.Verify(m => m.SpecifyAmount(It.IsAny<double>()), Times.Never());
        }
    }
    
    
}