﻿using CQELight.Abstractions.Events;
using CQELight.Buses.RabbitMQ.Client;
using CQELight.Events.Serializers;
using CQELight.TestFramework;
using FluentAssertions;
using RabbitMQ.Client;
using CQELight.Tools.Extensions;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CQELight.Abstractions.CQS.Interfaces;
using Microsoft.Extensions.Configuration;
using CQELight.Configuration;
using CQELight.Abstractions.Configuration;
using Moq;
using CQELight.Buses.RabbitMQ.Common;

namespace CQELight.Buses.RabbitMQ.Integration.Tests
{
    public class RabbitMQClientBusTests : BaseUnitTestClass
    {

        #region Ctor & members

        private class RabbitEvent : BaseDomainEvent
        {
            public string Data { get; set; }
        }

        private class RabbitCommand : ICommand
        {
            public string Data { get; set; }
        }
        private const string CONST_APP_ID = "AA3F9093-D7EE-4BB8-9B4E-EEC3447A89BA";

        private IModel _channel;
        private IConfiguration _testConfiguration;
        private AppId _appId;
        private Mock<IAppIdRetriever> _appIdRetrieverMock;
        private string _queueName = Consts.CONST_QUEUE_NAME_PREFIX + CONST_APP_ID.ToLower();

        public RabbitMQClientBusTests()
        {
            _testConfiguration = new ConfigurationBuilder().AddJsonFile("test-config.json").Build();
            if (_testConfiguration["host"] == "localhost" && !Directory.Exists(@"C:\Program Files\RabbitMQ Server"))
            {
                Assert.False(true, "It seems RabbitMQ is not installed on your system.");
            }
            CleanQueues();
            _appId = new AppId(Guid.Parse(CONST_APP_ID));
            _appIdRetrieverMock = new Mock<IAppIdRetriever>();
            _appIdRetrieverMock.Setup(m => m.GetAppId()).Returns(_appId);
        }

        private void CleanQueues()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _testConfiguration["host"],
                UserName = _testConfiguration["user"],
                Password = _testConfiguration["password"]
            };
            var connection = factory.CreateConnection();

            _channel = connection.CreateModel();

            _channel.ExchangeDelete(exchange: Consts.CONST_CQE_EXCHANGE_NAME);
            _channel.QueueDelete(queue: _queueName);

        }

        #endregion

        #region RegisterAsync

        [Fact]
        public async Task RabbitMQClientBus_RegisterAsync_AsExpected()
        {
            var evt = new RabbitEvent
            {
                Data = "testData"
            };

            var b = new RabbitMQClientBus(
                _appIdRetrieverMock.Object,
                new JsonDispatcherSerializer(),
                new RabbitMQClientBusConfiguration(_testConfiguration["host"], _testConfiguration["user"], _testConfiguration["password"]));


            await b.RegisterAsync(evt).ContinueWith(t =>
            {
                var result = _channel.BasicGet(_queueName, true);
                result.Should().NotBeNull();
                var enveloppeAsStr = Encoding.UTF8.GetString(result.Body);
                enveloppeAsStr.Should().NotBeNullOrWhiteSpace();

                var receivedEnveloppe = enveloppeAsStr.FromJson<Enveloppe>();
                receivedEnveloppe.Should().NotBeNull();

                var type = Type.GetType(receivedEnveloppe.AssemblyQualifiedDataType);
                var evet = receivedEnveloppe.Data.FromJson(type);
                evet.Should().BeOfType<RabbitEvent>();
                evet.As<RabbitEvent>().Data.Should().Be("testData");
            }).ConfigureAwait(false);
        }

        #endregion

    }
}
