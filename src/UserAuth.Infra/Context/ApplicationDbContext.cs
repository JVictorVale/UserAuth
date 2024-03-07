using Microsoft.EntityFrameworkCore;

namespace UserAuth.Infra.Context;

public class ApplicationDbContext : BaseApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}