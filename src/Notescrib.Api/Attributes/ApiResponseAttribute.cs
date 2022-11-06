using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Notescrib.Api.Attributes;

public class ApiResponseAttribute : ProducesResponseTypeAttribute
{
    public ApiResponseAttribute(Type type, HttpStatusCode statusCode = HttpStatusCode.OK)
        : base(type, (int)statusCode)
    {
    }
}

public class CreatedApiResponseAttribute : ApiResponseAttribute
{
    public CreatedApiResponseAttribute() : base(typeof(void), HttpStatusCode.Created)
    {
    }
}

public class NoContentApiResponseAttribute : ApiResponseAttribute
{
    public NoContentApiResponseAttribute() : base(typeof(void), HttpStatusCode.NoContent)
    {
    }
}
