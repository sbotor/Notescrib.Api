﻿namespace Notescrib.Notes.Models.Exceptions;

public class AppException : Exception
{
    public AppException(string? message = null) : base(message)
    {
    }
}
