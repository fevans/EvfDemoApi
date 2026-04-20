using System.ComponentModel.DataAnnotations;

namespace EvfDemoApi.Api.Infrastructure.Validation;

public static class ValidationExtensions
{
    public static Dictionary<string, string[]>? Validate<T>(this T model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(model, context, results, validateAllProperties: true))
        {
            return null;
        }

        return results
            .SelectMany(result =>
            {
                var members = result.MemberNames.Any()
                    ? result.MemberNames
                    : new[] { "request" };

                return members.Select(member => new
                {
                    Member = member,
                    Error = result.ErrorMessage ?? "The request is invalid."
                });
            })
            .GroupBy(x => x.Member, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.Error).Distinct().ToArray(),
                StringComparer.OrdinalIgnoreCase);
    }
}
