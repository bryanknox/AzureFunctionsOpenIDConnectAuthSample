using System;

namespace TestFixtures.AzureFunctions
{
    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message)
            : base(message)
        {
        }
    }
}
