namespace Bekam.Application.Abstraction.Contracts.Persistence.DbInitializers;
public interface IDbInitializer
{
    Task InitializeAsync();
    Task SeedAsync();
}
