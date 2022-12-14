using Moq;
using SchoderChain;

namespace SchoderChainUnitTests
{
    [TestClass]
    public class ChainTests
    {
        // IMPORTANT: Add these lines to your Startup.cs to perform dependency injections for all your processors in one go
        //
        // using SchoderChain;
        //
        // Assembly.GetEntryAssembly().GetTypesAssignableFrom().ToList().ForEach((processor) =>
        // {
        //     services.AddScoped(typeof(IProcessor), processor);
        // });

        [TestMethod]
        public async Task GetResultAsync_Processes_All_Processors()
        {
            // Given I have empty test parameters and two test processors, and I have mocked the SlackManager
            var bllData = new BLLData();
            var mockSlackManager = new Mock<ISlackManager>();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(bllData, mockSlackManager.Object),
                new ChangeEmailProcessor(bllData, mockSlackManager.Object)
            });

            // When I process a chain containing these test processors
            var result = await chain.ProcessAsync(nameof(GetResultAsync_Processes_All_Processors),
                typeof(TestProcessor1),
                typeof(ChangeEmailProcessor));

            // Then I expect both processors to be processed
            Assert.AreEqual("TestProcessor1ChangeEmailProcessor", string.Concat(result.StackTrace));

            // And I expect the ChainStart to be filled
            Assert.AreEqual(nameof(GetResultAsync_Processes_All_Processors), result.CalledBy);

            // And I expect the Email in the parameters to be changed
            Assert.AreEqual("changed", bllData.Email);
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Processor()
        {
            // Given I have empty test parameters and one test processor, and I have mocked the SlackManager
            var bllData = new BLLData();
            var mockSlackManager = new Mock<ISlackManager>();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(bllData, mockSlackManager.Object),
            });

            // When I process a chain containing this one processor
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessor1));

            // Then I expect one processor to be processed
            Assert.AreEqual("TestProcessor1", string.Concat(result.StackTrace));
        }

        [TestMethod]
        public async Task GetResultAsync_Reverts_All_Processor_Actions_Before_An_Exception_And_Returns_The_Exception_Message()
        {
            // Given I have empty test parameters, three test processors, and a fourth processor throwing an exception, and I have mocked the SlackManager
            var bllData = new BLLData();
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorAsync(It.IsAny<string>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(bllData, mockSlackManager.Object),
                new ChangeEmailProcessor(bllData, mockSlackManager.Object),
                new TestProcessor3(mockSlackManager.Object),
                new TestProcessor4(mockSlackManager.Object),
                new TestProcessorException(mockSlackManager.Object)
            });

            // When I process a chain containing these processors
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessor1),
                typeof(ChangeEmailProcessor),
                typeof(TestProcessor3),
                typeof(TestProcessorException),
                typeof(TestProcessor4));

            // And I expect the actions until the exception to be processed and then undone again (in the correct order)
            Assert.AreEqual("TestProcessor1ChangeEmailProcessorTestProcessor3TestProcessorExceptionUndoTestProcessorExceptionUndoTestProcessor3UndoChangeEmailProcessorUndoTestProcessor1",
                string.Concat(result.StackTrace));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorAsync(It.Is<string>(
                p => p == "\r\n--------------------\r\nTestProcessor1\r\nChangeEmailProcessor\r\nTestProcessor3\r\nTestProcessorException\r\nUndoTestProcessorException\r\nUndoTestProcessor3\r\nUndoChangeEmailProcessor\r\nUndoTestProcessor1\r\n\r\nAttempted to divide by zero.\r\n")));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_Zero_ProcessorElements()
        {
            // Given I have empty test parameters and no test processors
            var chain = new Chain(new Processor[0]);

            // When I process a chain containing no processors
            var result = await chain.ProcessAsync(string.Empty);

            // Then I expect nothing to be done and the chain not to fall over
            Assert.AreEqual(0, result.StackTrace.Count);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task GetResultAsync_Stops_The_Chain_Conditionally(bool processAll)
        {
            // Given I have test parameters, and I have mocked the SlackManager
            var bllData = new BLLData { Email = processAll ? string.Empty : "test" };
            var mockSlackManager = new Mock<ISlackManager>();

            // And I have two test processors of which the first one stops the chain depending on a specific condition
            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(bllData, mockSlackManager.Object),
                new ChangeEmailProcessor(bllData, mockSlackManager.Object)
            });

            // When I process a chain containing these processors
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessor1),
                typeof(ChangeEmailProcessor));

            // Then I expect both processors, resp. only one processor to be processed depending on the "stop" condition
            Assert.AreEqual(processAll ? "TestProcessor1ChangeEmailProcessor" : "TestProcessor1", string.Concat(result.StackTrace));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Failing_Processor()
        {
            // Given I have empty test parameters and one processor throwing an exception, and I have mocked the SlackManager
            var mockSlackManager = new Mock<ISlackManager>();
            // mockSlackManager.Setup(m => m.SlackErrorAsync(It.IsAny<string>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessorException(mockSlackManager.Object)
            });

            // When I process a chain containing this processor
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessorException));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorAsync(It.Is<string>(
                p => p == "\r\n--------------------\r\nTestProcessorException\r\nUndoTestProcessorException\r\n\r\nAttempted to divide by zero.\r\n")));
        }
    }
}