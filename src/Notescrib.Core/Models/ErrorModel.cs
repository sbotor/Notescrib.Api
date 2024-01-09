namespace Notescrib.Core.Models;

public record ErrorModel(string Code, string? Message = null);

public record ValidationErrorModel(string Code, string[] Messages);
