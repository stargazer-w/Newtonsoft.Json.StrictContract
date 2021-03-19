using System;
using System.Linq;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Stargazer.Extensions.Newtonsoft.Json.Strict
{
    public class StrictContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            foreach(var prop in contract.Properties)
            {
                if(prop.DeclaringType!.GetMember(prop.UnderlyingName!).First().ToContextualMember().Nullability is Nullability.NotNullable)
                {
                    prop.Required = prop.Required switch
                    {
                        Required.Default => Required.DisallowNull,
                        Required.AllowNull => Required.Always,
                        _ => prop.Required
                    };
                }

                if(contract.CreatorParameters.Any(x => string.Equals(x.UnderlyingName, prop.UnderlyingName, StringComparison.OrdinalIgnoreCase)))
                {
                    prop.Required = prop.Required switch
                    {
                        Required.Default => Required.AllowNull,
                        Required.DisallowNull => Required.Always,
                        _ => prop.Required
                    };
                }
            }
            return contract;
        }
    }
}
