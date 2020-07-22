﻿using AutoFixture;
using NUnit.Framework;

namespace Atlas.Tests
{
    [TestFixture]
    public abstract class TextFixtureBase
    {
        protected Fixture Fixture { get; private set; }

        [SetUp]
        protected void SetUpAutoFixture()
        {
            Fixture = new Fixture();

            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
