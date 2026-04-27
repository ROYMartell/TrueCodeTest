using MediatR;
using TrueCodeTest.Finance.Domain;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Queries.ListCurrencies;

public sealed record ListCurrenciesQuery : IRequest<Result<IReadOnlyList<Currency>>>;
