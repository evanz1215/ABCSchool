namespace Infrastructure.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerHeaderAttribute(string headerName, string description, string defaultValue, bool isRequired) : Attribute
{
    public string HeaderName { get; set; } = headerName;
    public string Description { get; set; } = description;
    public string DefaultValue { get; set; } = defaultValue;
    public bool IsRequired { get; set; } = isRequired;
}

//[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
//public class SwaggerHeaderAttribute : Attribute
//{
//    public SwaggerHeaderAttribute(string headerName, string description, string defaultValue, bool isRequired)
//    {
//        HeaderName = headerName;
//        Description = description;
//        DefaultValue = defaultValue;
//        IsRequired = isRequired;
//    }

//    public string HeaderName { get; set; }
//    public string Description { get; set; }
//    public string DefaultValue { get; set; }
//    public bool IsRequired { get; set; }
//}