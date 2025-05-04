using System;
using DatesAndStuff;
using NUnit.Framework;

namespace DatesAndStuff.Tests
{
    [TestFixture]
    public class DateTimeStringFormatterTests
    {
        [Test]
        public void ToIsoStringFast_ValidDateTime_ReturnsCorrectIsoFormat()
        {
            // Arrange
            var dateTime = new DateTime(2023, 05, 14, 09, 08, 07);
            var expected = "2023-05-14T09:08:07";

            // Act
            var result = dateTime.ToIsoStringFast();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToIsoStringFast_SingleDigitMonthAndDay_ReturnsPaddedIsoFormat()
        {
            // Arrange
            var dateTime = new DateTime(2023, 01, 02, 03, 04, 05);
            var expected = "2023-01-02T03:04:05";

            // Act
            var result = dateTime.ToIsoStringFast();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToIsoStringFast_MaxValue_ReturnsCorrectIsoFormat()
        {
            // Arrange
            var dateTime = DateTime.MaxValue; // 9999-12-31T23:59:59
            var expected = "9999-12-31T23:59:59";

            // Act
            var result = dateTime.ToIsoStringFast();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ToIsoStringFast_MinValue_ReturnsCorrectIsoFormat()
        {
            // Arrange
            var dateTime = DateTime.MinValue; // 0001-01-01T00:00:00
            var expected = "0001-01-01T00:00:00";

            // Act
            var result = dateTime.ToIsoStringFast();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}