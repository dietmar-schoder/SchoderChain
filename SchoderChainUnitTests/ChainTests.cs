using Moq;
using SchoderChain;

namespace SchoderChainUnitTests
{
    [TestClass]
    public class ChainTests
    {
        // IMPORTANT: Add these lines to your Program.cs to perform dependency injections
        //
        // using SchoderChain;
        //
        // builder.Services.AddScoped<ISlackManager, SlackManager>();
        // builder.Services.AddScoped<IChain, Chain>();

        // // Processors
        // builder.Services.AddScoped<IProcessor, TestProcessor1>();
        // builder.Services.AddScoped<IProcessor, ChangeEmailProcessor>();
        // builder.Services.AddScoped<IProcessor, TestProcessor3>();
        // builder.Services.AddScoped<IProcessor, TestProcessor4>();

        // // Processor data
        // builder.Services.AddScoped<BLLData, BLLData>();

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
            Assert.AreEqual("TestProcessor1,ChangeEmailProcessor", string.Join(",", result.StackTrace));

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
            mockSlackManager.Setup(m => m.SlackErrorChainResultAsync(It.IsAny<ChainResult>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessor1(bllData, mockSlackManager.Object),
                new ChangeEmailProcessor(bllData, mockSlackManager.Object),
                new TestProcessor3(mockSlackManager.Object),
                new TestProcessor4(mockSlackManager.Object),
                new TestProcessorException1(mockSlackManager.Object)
            });

            // When I process a chain containing these processors
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessor1),
                typeof(ChangeEmailProcessor),
                typeof(TestProcessor3),
                typeof(TestProcessorException1),
                typeof(TestProcessor4));

            var expectedActions = "TestProcessor1,ChangeEmailProcessor,TestProcessor3,TestProcessorException1,UndoTestProcessorException1,UndoTestProcessor3,UndoChangeEmailProcessor,UndoTestProcessor1";
            // And I expect the actions until the exception to be processed and then undone again (in the correct order)
            Assert.AreEqual(expectedActions, string.Join(",", result.StackTrace));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorChainResultAsync(It.Is<ChainResult>(
                c => string.Join(",", c.StackTrace).Equals(expectedActions)
                && c.Exception.Message.Equals("Attempted to divide by zero."))));
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
            Assert.AreEqual(processAll ? "TestProcessor1,ChangeEmailProcessor" : "TestProcessor1", string.Join(",", result.StackTrace));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Failing_Processor_ProcessOkAsync()
        {
            // Given I have empty test parameters and one processor throwing an exception, and I have mocked the SlackManager
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorChainResultAsync(It.IsAny<ChainResult>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessorException1(mockSlackManager.Object)
            });

            // When I process a chain containing this processor
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessorException1));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorChainResultAsync(It.Is<ChainResult>(
                c => string.Join(",", c.StackTrace).Equals("TestProcessorException1,UndoTestProcessorException1")
                && c.Exception.Message.Equals("Attempted to divide by zero."))));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Failing_Processor_ProcessOk()
        {
            // Given I have empty test parameters and one processor throwing an exception, and I have mocked the SlackManager
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorChainResultAsync(It.IsAny<ChainResult>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessorException2(mockSlackManager.Object)
            });

            // When I process a chain containing this processor
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessorException2));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorChainResultAsync(It.Is<ChainResult>(
                c => string.Join(",", c.StackTrace).Equals("TestProcessorException2,UndoTestProcessorException2")
                && c.Exception.Message.Equals("Attempted to divide by zero."))));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Failing_Processor_ProcessAsync()
        {
            // Given I have empty test parameters and one processor throwing an exception, and I have mocked the SlackManager
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorChainResultAsync(It.IsAny<ChainResult>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessorException3(mockSlackManager.Object)
            });

            // When I process a chain containing this processor
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessorException3));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorChainResultAsync(It.Is<ChainResult>(
                c => string.Join(",", c.StackTrace).Equals("TestProcessorException3,UndoTestProcessorException3")
                && c.Exception.Message.Equals("Attempted to divide by zero."))));
        }

        [TestMethod]
        public async Task GetResultAsync_Works_For_One_Failing_Processor_Process()
        {
            // Given I have empty test parameters and one processor throwing an exception, and I have mocked the SlackManager
            var mockSlackManager = new Mock<ISlackManager>();
            mockSlackManager.Setup(m => m.SlackErrorChainResultAsync(It.IsAny<ChainResult>())).Verifiable();

            var chain = new Chain(new Processor[]
            {
                new TestProcessorException4(mockSlackManager.Object)
            });

            // When I process a chain containing this processor
            var result = await chain.ProcessAsync(string.Empty,
                typeof(TestProcessorException4));

            // And I expect the message in the exception to be the message of the exception thrown
            Assert.AreEqual(result.Exception.Message, "Attempted to divide by zero.");

            // And I expect the error to be sent to Slack
            mockSlackManager.Verify(m => m.SlackErrorChainResultAsync(It.Is<ChainResult>(
                c => string.Join(",", c.StackTrace).Equals("TestProcessorException4,UndoTestProcessorException4")
                && c.Exception.Message.Equals("Attempted to divide by zero."))));
        }
    }
}
