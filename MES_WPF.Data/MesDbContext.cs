using Microsoft.EntityFrameworkCore;
using MES_WPF.Core.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Reflection;
using MES_WPF.Data.EntityConfigurations;

namespace MES_WPF.Data
{
    public class MesDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // 系统管理模块
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Dictionary> Dictionaries { get; set; } = null!;
        public DbSet<DictionaryItem> DictionaryItems { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<OperationLog> OperationLogs { get; set; } = null!;
        public DbSet<SystemConfig> SystemConfigs { get; set; } = null!;

        public MesDbContext(DbContextOptions<MesDbContext> options)
        : base(options)
        {
        }
        public MesDbContext(DbContextOptions<MesDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 默认使用SQLite作为本地数据库
                var connectionString = _configuration?.GetConnectionString("DefaultConnection")
                    ?? $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mes.db")}";

                optionsBuilder.UseSqlite(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 应用所有IEntityTypeConfiguration实现类
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}