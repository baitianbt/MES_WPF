using Microsoft.EntityFrameworkCore;
using MES_WPF.Core.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Reflection;
using MES_WPF.Data.EntityConfigurations;
using MES_WPF.Model.BasicInformation;
using MES_WPF.Model.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;

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

        // 基础信息模块
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<BOM> BOMs { get; set; } = null!;
        public DbSet<BOMItem> BOMItems { get; set; } = null!;
        public DbSet<ProcessRoute> ProcessRoutes { get; set; } = null!;
        public DbSet<Operation> Operations { get; set; } = null!;
        public DbSet<RouteStep> RouteSteps { get; set; } = null!;
        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<Equipment> Equipment { get; set; } = null!;
        
        // 设备管理模块
        public DbSet<EquipmentMaintenancePlan> EquipmentMaintenancePlans { get; set; } = null!;
        public DbSet<MaintenanceItem> MaintenanceItems { get; set; } = null!;
        public DbSet<MaintenanceOrder> MaintenanceOrders { get; set; } = null!;
        public DbSet<MaintenanceExecution> MaintenanceExecutions { get; set; } = null!;
        public DbSet<MaintenanceItemExecution> MaintenanceItemExecutions { get; set; } = null!;
        public DbSet<Spare> Spares { get; set; } = null!;
        public DbSet<SpareUsage> SpareUsages { get; set; } = null!;
        public DbSet<EquipmentParameterLog> EquipmentParameterLogs { get; set; } = null!;

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