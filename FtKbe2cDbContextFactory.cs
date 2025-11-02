using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ft_kbe2c;
public class FtKbe2cDbContextFactory
    : IDesignTimeDbContextFactory<FtKbe2cDbContext>
{
    public FtKbe2cDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var builder = new DbContextOptionsBuilder<FtKbe2cDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            return new FtKbe2cDbContext(builder.Options);
        }
}