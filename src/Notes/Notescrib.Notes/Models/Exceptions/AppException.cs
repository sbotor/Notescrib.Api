﻿namespace Notescrib.Notes.Application.Models.Exceptions;

public class AppException : Exception
{
    public AppException(string? message = null) : base(message)
    {
    }
}