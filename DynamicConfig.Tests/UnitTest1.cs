using System;
using DynamicConfig.Core.Interfaces.Shared;
using DynamicConfig.Infrastructure.Data;
using NUnit.Framework;


namespace Tests
{
	[TestFixture]
    public class Tests
    {

    private IConfig _configurationReader;

        [SetUp]
        public void Initialize()
        {
            _configurationReader = new ConfigirationReader("countoffiles","",TimeSpan.FromMinutes(2));
        }

        [Test]
        public void ReturnFalseGivenValueOf1()
        {
            string key = "countoffiles";
            string expectedValue = "6";
            string value = _configurationReader.GetValue<string>(key);
            Assert.AreEqual(value, expectedValue);
        }
    }
}