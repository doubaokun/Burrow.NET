﻿using System;
using System.IO;
using System.Threading;
using Burrow.Extras.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

// ReSharper disable InconsistentNaming
namespace Burrow.Tests.Extras.Internal.PriorityBurrowConsumerTests
{
    [TestClass]
    public class MethodDoAck
    {
        [TestMethod]
        public void Should_do_nothing_if_already_disposed()
        {
            // Arrange
            var waitHandler = new ManualResetEvent(false);
            var model = Substitute.For<IModel>();
            var consumer = new PriorityBurrowConsumer(model, Substitute.For<IMessageHandler>(), Substitute.For<IRabbitWatcher>(), false, 1);
            var queue = Substitute.For<IInMemoryPriorityQueue<GenericPriorityMessage<BasicDeliverEventArgs>>>();
            queue.When(x => x.Dequeue()).Do(callInfo => waitHandler.WaitOne());
            consumer.Init(queue, new CompositeSubscription(), 1, "sem");
            consumer.Ready();

            // Action
            consumer.Dispose();
            consumer.DoAck(new BasicDeliverEventArgs());

            // Assert
            model.DidNotReceive().BasicAck(Arg.Any<ulong>(), Arg.Any<bool>());
            waitHandler.Set();
        }


        
    }
}
// ReSharper restore InconsistentNaming