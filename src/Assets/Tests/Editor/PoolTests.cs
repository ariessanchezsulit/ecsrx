﻿using System.Linq;
using EcsRx.Events;
using EcsRx.Pools;
using EcsRx.Pools.Identifiers;
using NSubstitute;
using NUnit.Framework;
using UniRx;

namespace EcsRx.Tests
{
    [TestFixture]
    public class PoolTests
    {
        [Test]
        public void should_create_new_entity()
        {
            var expectedId = 123456;
            var mockIdentityGenerator = Substitute.For<IIdentifyGenerator>();
            mockIdentityGenerator.GenerateId().Returns(expectedId);

            var mockMessageBroker = Substitute.For<IMessageBroker>();

            var pool = new Pool("", mockIdentityGenerator, mockMessageBroker);
            var entity = pool.CreateEntity();

            Assert.That(entity.Id, Is.EqualTo(expectedId));
            Assert.That(entity.Components, Is.Not.Null);
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void should_raise_event_when_creating_entity()
        {
            var mockIdentityGenerator = Substitute.For<IIdentifyGenerator>();
            var mockMessageBroker = Substitute.For<IMessageBroker>();

            var hasRaised = false;
            var pool = new Pool("", mockIdentityGenerator, mockMessageBroker);
            var entity =pool.CreateEntity();

            mockMessageBroker.Received().Publish(Arg.Is<EntityAddedEvent>(x => x.Entity == entity && x.Pool == pool));
        }

        [Test]
        public void should_add_created_entity_into_the_pool()
        {
            var mockIdentityGenerator = Substitute.For<IIdentifyGenerator>();
            var mockMessageBroker = Substitute.For<IMessageBroker>();

            var pool = new Pool("", mockIdentityGenerator, mockMessageBroker);
            var entity = pool.CreateEntity();

            Assert.That(pool.Entities.Count(), Is.EqualTo(1));
            Assert.That(pool.Entities.First(), Is.EqualTo(entity));
        }

        [Test]
        public void should_raise_event_when_removing_entity()
        {
            var mockIdentityGenerator = Substitute.For<IIdentifyGenerator>();
            var mockMessageBroker = Substitute.For<IMessageBroker>();
            
            var pool = new Pool("", mockIdentityGenerator, mockMessageBroker);
            var entity = pool.CreateEntity();
            pool.RemoveEntity(entity);

            mockMessageBroker.Received().Publish(Arg.Is<EntityRemovedEvent>(x => x.Entity == entity && x.Pool == pool));
        }

        [Test]
        public void should_remove_created_entity_from_the_pool()
        {
            var mockIdentityGenerator = Substitute.For<IIdentifyGenerator>();
            var mockMessageBroker = Substitute.For<IMessageBroker>();

            var pool = new Pool("", mockIdentityGenerator, mockMessageBroker);
            var entity = pool.CreateEntity();
            pool.RemoveEntity(entity);

            Assert.That(pool.Entities, Is.Empty);
        }
    }
}