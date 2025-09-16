using NUnit.Framework;

namespace EmployeeManagementSystem.Tests
{
    public class SampleTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DummyTest_Passes()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
        }
    }
}
