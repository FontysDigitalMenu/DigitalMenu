using DigitalMenu_30_DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class SeederController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public SeederController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task Seed()
    {
        await SeedData.ResetDatabaseAndSeed(_dbContext);
    }
}