using Xunit;

namespace Microsoft.Extensions.Logging.Test
{
    public class EventIdTests
    {
        [Fact]
        public void EventId_AddInt_ReturnsCorrectEventId()
        {
            // Arrange
            var event1 = new EventId(1);
            var adder = 2;

            // Act
            var event3 = event1 + adder;

            // Assert
            Assert.Equal(3, event3.Id);
            Assert.Equal(new EventId(3), event3);
        }

        [Fact]
        public void EventId_SubtractInt_ReturnsCorrectEventId()
        {
            // Arrange
            var event42 = new EventId(42);
            var subtracter = 12;

            // Act
            var event30 = event42 - subtracter;

            // Assert
            Assert.Equal(30, event30.Id);
            Assert.Equal(new EventId(30), event30);
        }

        [Fact]
        public void EventId_AddEventId_ReturnsCorrectEventId()
        {
            // Arrange
            var event1 = new EventId(1);
            var adder = new EventId(2);

            // Act
            var event3 = event1 + adder;

            // Assert
            Assert.Equal(3, event3.Id);
            Assert.Equal(new EventId(3), event3);
        }

        [Fact]
        public void EventId_SubtractEventId_ReturnsCorrectEventId()
        {
            // Arrange
            var event42 = new EventId(42);
            var subtracter = new EventId(12);

            // Act
            var event30 = event42 - subtracter;

            // Assert
            Assert.Equal(30, event30.Id);
            Assert.Equal(new EventId(30), event30);
        }
    }
}
