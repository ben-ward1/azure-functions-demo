using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CustomerOnboarding.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShirtSize
    {
        [EnumMember(Value = "xs")]
        XS,
        [EnumMember(Value = "s")]
        S,
        [EnumMember(Value = "m")]
        M,
        [EnumMember(Value = "l")]
        L,
        [EnumMember(Value = "xl")]
        XL,
        [EnumMember(Value = "xxl")]
        XXL,
        [EnumMember(Value = "xxxl")]
        XXXL
    }
}
