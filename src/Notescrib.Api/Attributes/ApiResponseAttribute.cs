﻿using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Notescrib.Api.Attributes;

public class ApiResponseAttribute : ProducesResponseTypeAttribute
{
    public ApiResponseAttribute(Type type, HttpStatusCode statusCode = HttpStatusCode.OK)
        : base(type, (int)statusCode)
    {
    }
}