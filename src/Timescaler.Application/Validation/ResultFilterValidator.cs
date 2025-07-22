using FluentValidation;
using Timescaler.Application.Contracts;

namespace Timescaler.Application.Validation;

public class ResultFilterValidator : AbstractValidator<ResultFilterDto>
{
    public ResultFilterValidator()
    {
        RuleFor(x => x)
            .Must(x => x.StartDateFrom == null || x.StartDateTo == null || x.StartDateFrom <= x.StartDateTo)
            .WithMessage("Начальная дата диапазона должна быть меньше или равна конечной.");

        RuleFor(x => x)
            .Must(x => x.AvgValueMin == null || x.AvgValueMax == null || x.AvgValueMin <= x.AvgValueMax)
            .WithMessage("Минимальное значение должно быть меньше или равно максимальному.");

        RuleFor(x => x)
            .Must(x => x.AvgExecTimeMin == null || x.AvgExecTimeMax == null || x.AvgExecTimeMin <= x.AvgExecTimeMax)
            .WithMessage("Минимальное время выполнения должно быть меньше или равно максимальному.");

        RuleFor(x => x.AvgValueMin)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AvgValueMin.HasValue)
            .WithMessage("Минимальное значение показателя не может быть отрицательным.");

        RuleFor(x => x.AvgValueMax)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AvgValueMax.HasValue)
            .WithMessage("Максимальное значение показателя не может быть отрицательным.");

        RuleFor(x => x.AvgExecTimeMin)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AvgExecTimeMin.HasValue)
            .WithMessage("Минимальное время выполнения не может быть отрицательным.");

        RuleFor(x => x.AvgExecTimeMax)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AvgExecTimeMax.HasValue)
            .WithMessage("Максимальное время выполнения не может быть отрицательным.");
    }
}