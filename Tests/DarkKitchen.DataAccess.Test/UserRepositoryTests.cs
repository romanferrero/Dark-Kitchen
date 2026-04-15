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
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!x" };
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

    [TestMethod]
    public void GetAll_NoPredicate_ReturnsAllUsers()
    {
        _context.Users.AddRange(
            new User { Email = "user1@test.com", Password = "ValidPass@1Ab!x" },
            new User { Email = "user2@test.com", Password = "ValidPass@1Ab!x" });
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_WithPredicate_ReturnsFilteredUsers()
    {
        _context.Users.AddRange(
            new User { Email = "user1@test.com", Password = "ValidPass@1Ab!x" },
            new User { Email = "user2@test.com", Password = "ValidPass@1Ab!x" });
        _context.SaveChanges();

        var result = _repository.GetAll(u => u.Email == "user1@test.com");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("user1@test.com", result[0].Email);
    }

    [TestMethod]
    public void Add_ValidUser_PersistsInDatabase()
    {
        var user = new User
        {
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass@1Ab!xyz",
            Role = UserRole.Client,
        };

        _repository.Add(user);

        var saved = _context.Users.FirstOrDefault(u => u.Email == "juan@test.com");
        Assert.IsNotNull(saved);
        Assert.IsTrue(saved.Id > 0);
        Assert.AreEqual("Juan", saved.FirstName);
        Assert.AreEqual("Garcia", saved.LastName);
        Assert.AreEqual(UserRole.Client, saved.Role);
    }
}
