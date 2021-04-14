using System;
using System.Linq;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Stargazer.Extensions.Newtonsoft.Json.Strict
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
            if(contract is not JsonObjectContract objectContract)
                return contract;

            foreach(var prop in objectContract.Properties)
            {
                if(prop.DeclaringType!.GetMember(prop.UnderlyingName!).First().ToContextualMember().Nullability is Nullability.NotNullable)
                {
                    prop.Required = prop.Required switch
                    {
                        Required.Default => Required.DisallowNull,
                        Required.AllowNull => Required.Always,
                        { } x => x
                    };
                }

                if(objectContract.CreatorParameters.Any(x => string.Equals(x.UnderlyingName, prop.UnderlyingName, StringComparison.OrdinalIgnoreCase)))
                {
                    prop.Required = prop.Required switch
                    {
                        Required.Default => Required.AllowNull,
                        Required.DisallowNull => Required.Always,
                        { } x => x
                    };
                }
            }
            return objectContract;
        }
    }
}
