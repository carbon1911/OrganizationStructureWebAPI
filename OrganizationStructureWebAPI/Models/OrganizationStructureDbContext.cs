using Microsoft.EntityFrameworkCore;
using OrganizationStructureWebAPI.Models.OrganizationStructure;
using OrganizationStructureWebAPI.Exceptions;
using OrganizationStructureWebAPI.Models.Misc;

#pragma warning disable CS1591
namespace OrganizationStructureWebAPI.Models
{
	public class OrganizationStructureDbContext : DbContext
	{
		public OrganizationStructureDbContext(DbContextOptions<OrganizationStructureDbContext> options) 
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}

		public DbSet<Employee> Employees { get; set; } = default!;
		public DbSet<Company> Companies { get; set; } = default!;
		public DbSet<Division> Divisions { get; set; } = default!;
		public DbSet<Project> Projects { get; set; } = default!;
		public DbSet<Department> Departments { get; set; } = default!;

		public DbSet<T>? GetDbSet<T>() where T : OrganizationStructureNode
		{
			if (typeof(T) == typeof(Company))
			{
				return Companies as DbSet<T>;
			}
			if (typeof(T) == typeof(Division))
			{
				return Divisions as DbSet<T>;
			}
			if (typeof(T) == typeof(Project))
			{ 
				return Projects as DbSet<T>;
			}
			if (typeof(T) == typeof(Department))
			{
				return Departments as DbSet<T>;
			}
			if (typeof(T) == typeof(OrganizationStructureNodeNull))
			{
				return null;
			}
			throw new TypeArgumentException(nameof(T));
		}
	}
}
#pragma warning restore CS1591