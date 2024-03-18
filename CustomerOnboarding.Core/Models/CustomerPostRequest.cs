using CustomerOnboarding.Core.Enums;
using System.Runtime.Serialization;

namespace CustomerOnboarding.Core.Models
{
    [DataContract]
    public class CustomerPostRequest
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; init; }
        [DataMember(Name = "shirtSize", IsRequired = true)]
        public ShirtSize ShirtSize { get; init; }
    }
}
