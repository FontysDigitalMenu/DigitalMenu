using DigitalMenu_30_DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/seeder")]
[ApiController]
public class SeederController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task Seed()
    {
        await new SeedData(dbContext).ResetDatabaseAndSeed();
    }
}