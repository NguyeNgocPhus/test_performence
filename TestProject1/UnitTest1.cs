using Microsoft.EntityFrameworkCore;
using Moq;
using test_peformance;
using test_peformance.Abstractions;
using test_peformance.Controllers;
using test_peformance.Entities;

namespace TestProject1;

public class UnitTest1
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // mỗi test 1 DB mới
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task StartJob_LastJobIsNull_BeforeStartJob()
    { 
        // Arrange
        var dbContext = GetDbContext();

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        
        mockDateTimeProvider.Setup(x => x.StartSync()).Returns(new TimeSpan(8,0,0)); // 17:17
        mockDateTimeProvider.Setup(x => x.EndSync()).Returns(new TimeSpan(19, 0, 0));
        mockDateTimeProvider.Setup(x => x.Duration()).Returns(300);

        mockDateTimeProvider.Setup(x => x.Now()).Returns(DateTime.Today.AddHours(7).AddMinutes(17)); // 17:17

        var controller = new CronjobController(dbContext, mockDateTimeProvider.Object);

        // Act
        var result = await controller.StartJob();
        // Assert
        var saved = await dbContext.SystemConfig.FirstOrDefaultAsync(x => x.Name == "Cronjob");
        Assert.NotNull(saved);
        Assert.True(saved.TimeChanged == DateTime.MinValue);
    }
    [Fact]
    public async Task StartJob_LastJobIsNull_AfterStartJob_NotRunning()
    { 
        // Arrange
        var dbContext = GetDbContext();

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        
        mockDateTimeProvider.Setup(x => x.StartSync()).Returns(new TimeSpan(8,0,0)); // 17:17
        mockDateTimeProvider.Setup(x => x.EndSync()).Returns(new TimeSpan(19, 0, 0));
        mockDateTimeProvider.Setup(x => x.Duration()).Returns(10);

        mockDateTimeProvider.Setup(x => x.Now()).Returns(DateTime.Today.AddHours(8).AddMinutes(5)); // 17:17

        var controller = new CronjobController(dbContext, mockDateTimeProvider.Object);

        // Act
        var result = await controller.StartJob();
        // Assert
        var saved = await dbContext.SystemConfig.FirstOrDefaultAsync(x => x.Name == "Cronjob");
        Assert.NotNull(saved);
        Assert.True(saved.TimeChanged == DateTime.Today.Add(new TimeSpan(8,0,0)));
    }
    [Fact]
    public async Task StartJob_LastJobIsNull_AfterStartJob_Running()
    { 
        // Arrange
        var dbContext = GetDbContext();

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        
        mockDateTimeProvider.Setup(x => x.StartSync()).Returns(new TimeSpan(8,0,0));
        mockDateTimeProvider.Setup(x => x.EndSync()).Returns(new TimeSpan(19, 0, 0));
        mockDateTimeProvider.Setup(x => x.Duration()).Returns(10);

        mockDateTimeProvider.Setup(x => x.Now()).Returns(DateTime.Today.AddHours(8).AddMinutes(11));

        var controller = new CronjobController(dbContext, mockDateTimeProvider.Object);

        // Act
        var result = await controller.StartJob();
        // Assert
        var saved = await dbContext.SystemConfig.FirstOrDefaultAsync(x => x.Name == "Cronjob");
        Assert.NotNull(saved);
        Assert.True(saved.TimeChanged == DateTime.Today.Add(new TimeSpan(8,10,0)));
    }
    
    [Fact]
    public async Task StartJob_LastJobNotNull_RunStartJob()
    { 
        // Arrange
        var dbContext = GetDbContext();
        dbContext.Add(new SystemConfig()
        {
            Name = "Cronjob",
            Value = "1",
            TimeChanged = DateTime.Today.AddDays(-1).AddHours(19).AddMinutes(1),
        });
        await dbContext.SaveChangesAsync();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        
        mockDateTimeProvider.Setup(x => x.StartSync()).Returns(new TimeSpan(8,0,0));
        mockDateTimeProvider.Setup(x => x.EndSync()).Returns(new TimeSpan(19, 0, 0));
        mockDateTimeProvider.Setup(x => x.Duration()).Returns(30);

        mockDateTimeProvider.Setup(x => x.Now()).Returns(DateTime.Today.AddHours(8).AddMinutes(0));

        var controller = new CronjobController(dbContext, mockDateTimeProvider.Object);

        // Act
        var result = await controller.StartJob();
        // Assert
        var saved = await dbContext.SystemConfig.FirstOrDefaultAsync(x => x.Name == "Cronjob");
        Assert.NotNull(saved);
        Assert.True(saved.TimeChanged == DateTime.Today.Add(new TimeSpan(8,0,0)));
    }
}