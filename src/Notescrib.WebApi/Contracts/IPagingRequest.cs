﻿namespace Notescrib.WebApi.Contracts;

public interface IPagingRequest
{
    int Page { get; set; }
    int PageSize { get; set; }
}
