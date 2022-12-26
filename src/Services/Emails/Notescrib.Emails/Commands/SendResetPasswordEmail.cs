using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Emails.Services;

namespace Notescrib.Emails.Commands;

public static class SendResetPasswordEmail
{
    public record Command(string ConfirmationUri, string To) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IEmailSender _sender;

        public Handler(IEmailSender sender)
        {
            _sender = sender;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(request.To, "Reset password",
                $"Reset your password here: {request.ConfirmationUri}");

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ConfirmationUri)
                .NotEmpty()
                .Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute));

            RuleFor(x => x.To)
                .EmailAddress();
        }
    }
}
