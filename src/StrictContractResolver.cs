using System;
using System.Linq;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Stargazer.Extensions.Newtonsoft.Json.StrictContract
{
    public class StrictContractResolver : IContractResolver
    {
        private readonly IContractResolver _backingResolver;

        public StrictContractResolver(IContractResolver backingResolver)
        {
            _backingResolver = backingResolver;
        }

        public StrictContractResolver() : this(new DefaultContractResolver())
        {
        }

        public JsonContract ResolveContract(Type type)
        {
            var contract = _backingResolver.ResolveContract(type);

            if (contract is JsonObjectContract objectContract)
            {
                foreach (var prop in objectContract.Properties)
                {
                    var propInfo = prop.DeclaringType
                                       !.GetProperties()
                                       .FirstOrDefault(p => string.Equals(p.Name, prop.UnderlyingName!))
                                       ?.ToContextualProperty();
                    if (propInfo?.Nullability is Nullability.NotNullable)
                    {
                        prop.Required = prop.Required switch
                        {
                            Required.Default => Required.DisallowNull,
                            Required.AllowNull => Required.Always,
                            var x => x
                        };
                    }

                    if (objectContract.CreatorParameters.Any(x => string.Equals(x.UnderlyingName, prop.UnderlyingName, StringComparison.OrdinalIgnoreCase)))
                    {
                        prop.Required = prop.Required switch
                        {
                            Required.Default => Required.AllowNull,
                            Required.DisallowNull => Required.Always,
                            var x => x
                        };
                    }
                }

                return objectContract;
            }

            return contract;
        }
    }
}
