using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test;

[TestClass]
public class UserRepositoryTests
{
    private AppDbContext _context = null!;
    private UserRepository _repository = null!;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        _repository = new UserRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [TestMethod]
    public void GetByEmail_ExistingUser_ReturnsUser()
    {
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!" };
        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _repository.GetByEmail("user@test.com");

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id > 0);
        Assert.AreEqual("user@test.com", result.Email);
    }

    [TestMethod]
    public void GetByEmail_NonExistingUser_ReturnsNull()
    {
        var result = _repository.GetByEmail("noexiste@test.com");

        Assert.IsNull(result);
    }
}
