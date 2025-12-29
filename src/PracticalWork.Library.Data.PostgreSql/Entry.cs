using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Repositories;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// Точка входа для регистрации зависимостей PostgreSQL‑хранилища.
/// Содержит методы для подключения контекста базы данных и репозиториев.
/// </summary>
public static class Entry
{
    private static readonly Action<DbContextOptionsBuilder> DefaultOptionsAction = _ => { };

    /// <summary>
    /// Регистрирует в контейнере сервисов контекст базы данных и репозитории,
    /// необходимые для работы с PostgreSQL‑хранилищем.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов приложения.</param>
    /// <param name="optionsAction">
    /// Делегат конфигурации <see cref="DbContextOptionsBuilder"/>.
    /// Если не указан, используется конфигурация по умолчанию.
    /// </param>
    /// <returns>Коллекция сервисов с добавленными зависимостями.</returns>
    public static IServiceCollection AddPostgreSqlStorage(
        this IServiceCollection serviceCollection,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        serviceCollection.AddDbContext<AppDbContext>(
            optionsAction ?? DefaultOptionsAction,
            optionsLifetime: ServiceLifetime.Singleton);

        serviceCollection.AddScoped<IBookRepository, BookRepository>();
        serviceCollection.AddScoped<IBorrowRepository, BorrowRepository>();
        serviceCollection.AddScoped<IReaderRepository, ReaderRepository>();

        return serviceCollection;
    }
}
