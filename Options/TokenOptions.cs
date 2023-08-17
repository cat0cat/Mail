namespace YOApi.Options;

public class TokenOptions
{
    public const string Section = "Token";
#nullable disable
    public string Secret { get; set; }
    public string Expires { get; set; } 
#nullable enable
}
