using Newtonsoft.Json;
using NUnit.Framework;

#nullable enable

namespace Stargazer.Extensions.Newtonsoft.Json.StrictContract
{
    [TestFixture()]
    public class StrictContractResolverTests
    {
        private JsonSerializerSettings _settings = new() { ContractResolver = new StrictContractResolver() };

        public class ConstructorHasParametersClass
        {
            public ConstructorHasParametersClass(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }

            public string Gender { get; set; } = "Unknown";
        }

        [Test]
        public void MissingConstructorParametersTest()
        {
            Assert.Throws<JsonSerializationException>(() =>
                JsonConvert.DeserializeObject<ConstructorHasParametersClass>("{\"name\":\"bob\"}", _settings),
            "");
        }

        [Test]
        public void SetNonnullablePropertyAsNullTest()
        {
            Assert.Throws<JsonSerializationException>(() =>
                JsonConvert.DeserializeObject<ConstructorHasParametersClass>("{\"name\":\"bob\",\"age\":18,\"gender\":null}", _settings),
            "");
        }
    }
}