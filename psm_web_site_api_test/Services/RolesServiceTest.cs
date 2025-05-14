using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Roles;
using psm_web_site_api_project.Services.Roles;
using Xunit;

namespace psm_web_site_api_test.Services;

public class RolesServiceTest
{
    private readonly Mock<IRolesRepository> _rolesRepositoryMock;
    private readonly RolesService _rolesService;

    public RolesServiceTest()
    {
        _rolesRepositoryMock = new Mock<IRolesRepository>();
        _rolesService = new RolesService(_rolesRepositoryMock.Object);
    }

    private void SetupRolesRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Rol>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListRoles());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

       _rolesRepositoryMock
            .Setup(repo => repo.SelectRolesRepository())
            .ReturnsAsync(ListRoles());

       _rolesRepositoryMock
            .Setup(repo => repo.SelectRolRepository(It.IsAny<string>()))
            .ReturnsAsync(new Rol
            {
                IdRol = "1",
                Nombre = "Admin"
            });

        _rolesRepositoryMock
            .Setup(repo => repo.SelectRolesFilterRepository(
                It.IsAny<FilterDefinition<Rol>>()))
            .ReturnsAsync(asyncCursorMock.Object);
    }

    private static List<Rol> ListRoles()
    {
        return new List<Rol>
        {
            new Rol
            {
                IdRol = "1",
                Nombre = "Admin"
            },
            new Rol
            {
                IdRol = "2",
                Nombre = "User"
            }
        };
    }

    [Fact]
    public async Task SelectRolesService_Valid_ReturnsRoles()
    {
        // Arrange: Datos de prueba
        SetupRolesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _rolesService.SelectRolesService();

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    [Fact]
    public async Task SelectRolPorIdService_Valid_ReturnsRol()
    {
        // Arrange: Datos de prueba
        var rolId = "1";

        SetupRolesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _rolesService.SelectRolPorIdService(rolId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.IdRol));
        Assert.False(string.IsNullOrEmpty(result.Nombre));
    }

    [Fact]
    public async Task GetCursorRol_Valid_ReturnsRoles()
    {
        // Arrange: Datos de prueba
        var rolesId = new List<string> { "1" };

        SetupRolesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _rolesService.GetCursorRol(rolesId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }
}