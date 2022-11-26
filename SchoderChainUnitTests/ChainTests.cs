using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoderChain;

namespace SchoderChainUnitTests
{
    [TestClass]
    public class ChainTests
    {
        [TestMethod]
        public async Task GetResultAsync_Processes_All_Processors()
        {
            // Given I have empty test parameters and two test processors, and I have mocked the SlackManager
            var parameters = new BLLParameters { Email = "test" };
            var mockSlackManager = new Mock<ISlackManager>();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(mockSlackManager.Object),
                new TestProcessor2(mockSlackManager.Object)
            });

            // When I process a chain containing these test processors
            await chain.ProcessAsync(parameters,
                typeof(TestProcessor1),
                typeof(TestProcessor2));

            // Then I expect both processors to be processed
            Assert.AreEqual("TestProcessor1TestProcessor2", string.Concat(parameters.StackTrace));

            // And I expect the Email in the parameters to be changed
            Assert.AreEqual("test2", parameters.Email);
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Processor()
        {
            // Given I have empty test parameters and one test processor, and I have mocked the SlackManager
            var parameters = new BLLParameters();
            var mockSlackManager = new Mock<ISlackManager>();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(mockSlackManager.Object)
            });

            // When I process a chain containing this one processor
            await chain.ProcessAsync(parameters,
                typeof(TestProcessor1));

            // Then I expect one processor to be processed
            Assert.AreEqual("TestProcessor1", string.Concat(parameters.StackTrace));
        }

        [TestMethod]
        public async Task GetResultAsync_Reverts_All_Processor_Actions_Before_An_Exception_And_Returns_The_Exception_Message()
        {
            // Given I have empty test parameters, three test processors, and a fourth processor throwing an exception, and I have mocked the SlackManager
            var parameters = new BLLParameters();
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorAsync(It.IsAny<string>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(mockSlackManager.Object),
                new TestProcessor2(mockSlackManager.Object),
                new TestProcessor3(mockSlackManager.Object),
                new TestProcessor4(mockSlackManager.Object),
                new TestProcessorException(mockSlackManager.Object)
            });

            // When I process a chain containing these processors
            await chain.ProcessAsync(
                parameters,
                typeof(TestProcessor1),
                typeof(TestProcessor2),
                typeof(TestProcessor3),
                typeof(TestProcessorException),
                typeof(TestProcessor4)
                );

            // And I expect the actions until the exception to be processed and then undone again (in the correct order)
            Assert.AreEqual("TestProcessor1TestProcessor2TestProcessor3TestProcessorExceptionUndoTestProcessorExceptionUndoTestProcessor3UndoTestProcessor2UndoTestProcessor1",
                string.Concat(parameters.StackTrace));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(parameters.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorAsync(It.Is<string>(
                p => p == "\r\n--------------------\r\nTestProcessor1\r\nTestProcessor2\r\nTestProcessor3\r\nTestProcessorException\r\nUndoTestProcessorException\r\nUndoTestProcessor3\r\nUndoTestProcessor2\r\nUndoTestProcessor1\r\n\r\nAttempted to divide by zero.\r\n")));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_Zero_ProcessorElements()
        {
            // Given I have empty test parameters and no test processors
            var parameters = new BLLParameters();

            var chain = new Chain(new Processor[0]);

            // When I process a chain containing no processors
            await chain.ProcessAsync(parameters);

            // Then I expect nothing to be done and the chain not to fall over
            Assert.AreEqual(0, parameters.StackTrace.Count);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task GetResultAsync_Stops_The_Chain_Conditionally(bool processAll)
        {
            // Given I have test parameters, and I have mocked the SlackManager
            var parameters = new BLLParameters { ActionResult = processAll ? null : new OkResult() };
            var mockSlackManager = new Mock<ISlackManager>();

            // And I have two test processors of which the first one stops the chain depending on a specific condition
            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(mockSlackManager.Object),
                new TestProcessor2(mockSlackManager.Object)
            });

            // When I process a chain containing these processors
            await chain.ProcessAsync(parameters,
                typeof(TestProcessor1),
                typeof(TestProcessor2)
                );

            // Then I expect both processors, resp. only one processor to be processed depending on the "stop" condition
            Assert.AreEqual(processAll ? "TestProcessor1TestProcessor2" : "TestProcessor1", string.Concat(parameters.StackTrace));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Failing_Processor()
        {
            // Given I have empty test parameters and one processor throwing an exception, and I have mocked the SlackManager
            var parameters = new BLLParameters();
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorAsync(It.IsAny<string>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessorException(mockSlackManager.Object)
            });

            // When I process a chain containing this processor
            await chain.ProcessAsync(parameters,
                typeof(TestProcessorException));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(parameters.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorAsync(It.Is<string>(
                p => p == "\r\n--------------------\r\nTestProcessorException\r\nUndoTestProcessorException\r\n\r\nAttempted to divide by zero.\r\n")));
        }
    }
}