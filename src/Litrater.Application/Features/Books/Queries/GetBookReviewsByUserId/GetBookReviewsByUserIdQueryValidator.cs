using FluentValidation;

namespace Litrater.Application.Features.Books.Queries.GetBookReviewsByUserId;

internal sealed class GetBookReviewsByUserIdQueryValidator : AbstractValidator<GetBookReviewsByUserIdQuery>
{
    public GetBookReviewsByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must not exceed 100");
    }
}