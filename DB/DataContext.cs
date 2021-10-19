using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.DB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ClassModule> ClassModules { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<TraineeExam> TraineeExams { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationCategory> ApplicationCategories { get; set; }
        public DbSet<AdminFeedback> AdminFeedbacks { get; set; }
        public DbSet<TrainerFeedback> TrainerFeedbacks { get; set; }
        public DbSet<Cost> Costs { get; set; }
        public DbSet<CostType> CostTypes { get; set; }
        public DbSet<DeleteClassRequest> DeleteClassRequests { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<CompanyRequest> CompanyRequests { get; set; }
        public DbSet<CompanyRequestDetail> CompanyRequestDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto increase Identity column
            modelBuilder.UseSerialColumns();

            // Role table to other account entities
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Administrators)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Admins)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Trainers)
                .WithOne(t => t.Role)
                .HasForeignKey(t => t.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Trainees)
                .WithOne(t => t.Role)
                .HasForeignKey(t => t.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Companies)
                .WithOne(c => c.Role)
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Class relationship
            modelBuilder.Entity<Class>()
                .HasMany(c => c.Trainees)
                .WithOne(t => t.Class)
                .HasForeignKey(t => t.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.Classes)
                .WithOne(c => c.Room)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Classes)
                .WithOne(c => c.Admin)
                .HasForeignKey(c => c.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Trainer>()
                .HasMany(t => t.Classes)
                .WithOne(c => c.Trainer)
                .HasForeignKey(c => c.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Class>()
                .HasOne<DeleteClassRequest>(c => c.DeleteClassRequest)
                .WithOne(d => d.Class)
                .HasForeignKey<DeleteClassRequest>(d => d.ClassId);

            modelBuilder.Entity<Class>()
                .HasMany(c => c.Calendars)
                .WithOne(ca => ca.Class)
                .HasForeignKey(ca => ca.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Class>()
                .HasMany(c => c.Modules)
                .WithMany(m => m.Classes)
                .UsingEntity<ClassModule>(
                    j => j
                        .HasOne(cm => cm.Module)
                        .WithMany(m => m.ClassModules)
                        .HasForeignKey(cm => cm.ModuleId),
                    j => j
                        .HasOne(cm => cm.Class)
                        .WithMany(c => c.ClassModules)
                        .HasForeignKey(cm => cm.ClassId),
                    j =>
                    {
                        j.HasKey(cm => new { cm.ClassId, cm.ModuleId });
                    }
                );

            // Cost & Cost type
            modelBuilder.Entity<CostType>()
                .HasMany(ct => ct.Costs)
                .WithOne(c => c.CostType)
                .HasForeignKey(c => c.CostTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Admin relation
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.DeleteClassRequests)
                .WithOne(d => d.Admin)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Costs)
                .WithOne(c => c.Admin)
                .HasForeignKey(c => c.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // Feedback
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.AdminFeedbacks)
                .WithOne(af => af.Admin)
                .HasForeignKey(af => af.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Trainee>()
                .HasMany(t => t.AdminFeedbacks)
                .WithOne(af => af.Trainee)
                .HasForeignKey(af => af.TraineeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Trainer>()
                .HasMany(t => t.TrainerFeedbacks)
                .WithOne(tf => tf.Trainer)
                .HasForeignKey(tf => tf.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Trainee>()
                .HasMany(t => t.TrainerFeedbacks)
                .WithOne(tf => tf.Trainee)
                .HasForeignKey(tf => tf.TraineeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Application
            modelBuilder.Entity<ApplicationCategory>()
                .HasMany(ac => ac.Applications)
                .WithOne(a => a.ApplicationCategory)
                .HasForeignKey(a => a.ApplicationCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Trainee>()
                .HasMany(t => t.Applications)
                .WithOne(a => a.Trainee)
                .HasForeignKey(a => a.TraineeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Admin>()
                .HasMany(ad => ad.Applications)
                .WithOne(a => a.Admin)
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // Exam
            modelBuilder.Entity<Admin>()
                .HasMany(a => a.Exams)
                .WithOne(e => e.Admin)
                .HasForeignKey(e => e.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Module>()
                .HasMany(m => m.Exams)
                .WithOne(e => e.Module)
                .HasForeignKey(e => e.ModuleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Exam>()
                .HasMany(e => e.Trainees)
                .WithMany(t => t.Exams)
                .UsingEntity<TraineeExam>(
                    j => j
                        .HasOne(te => te.Trainee)
                        .WithMany(t => t.TraineeExams)
                        .HasForeignKey(te => te.TraineeId),
                    j => j
                        .HasOne(te => te.Exam)
                        .WithMany(e => e.TraineeExams)
                        .HasForeignKey(te => te.ExamId),
                    j =>
                    {
                        j.HasKey(te => new { te.TraineeId, te.ExamId });
                    }
                );

            // Mark & Certificate
            modelBuilder.Entity<Module>()
                .HasMany(m => m.TraineesMark)
                .WithMany(t => t.ModulesMarks)
                .UsingEntity<Mark>(
                    j => j
                        .HasOne(ma => ma.Trainee)
                        .WithMany(t => t.Marks)
                        .HasForeignKey(ma => ma.TraineeId),
                    j => j
                        .HasOne(ma => ma.Module)
                        .WithMany(t => t.Marks)
                        .HasForeignKey(ma => ma.ModuleId),
                    j =>
                    {
                        j.HasKey(ma => new { ma.ModuleId, ma.TraineeId });
                    }
                );

            modelBuilder.Entity<Module>()
                .HasMany(m => m.TraineesCertificate)
                .WithMany(t => t.ModulesCertificate)
                .UsingEntity<Certificate>(
                    j => j
                        .HasOne(ce => ce.Trainee)
                        .WithMany(t => t.Certificates)
                        .HasForeignKey(ce => ce.TraineeId),
                    j => j
                        .HasOne(ce => ce.Module)
                        .WithMany(t => t.Certificates)
                        .HasForeignKey(ce => ce.ModuleId),
                    j =>
                    {
                        j.HasKey(ce => new { ce.ModuleId, ce.TraineeId });
                    }
                );

            // Attendance
            modelBuilder.Entity<Attendance>().HasKey(at => new { at.CalendarId, at.TraineeId });

            modelBuilder.Entity<Attendance>()
                .HasOne<Calendar>(at => at.Calendar)
                .WithMany(c => c.Attendances)
                .HasForeignKey(at => at.CalendarId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Attendance>()
                .HasOne<Trainee>(at => at.Trainee)
                .WithMany(t => t.Attendances)
                .HasForeignKey(at => at.TraineeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Module>()
                .HasMany(m => m.Calendars)
                .WithOne(c => c.Module)
                .HasForeignKey(c => c.ModuleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Company request
            modelBuilder.Entity<Company>()
                .HasMany(c => c.CompanyRequests)
                .WithOne(cr => cr.Company)
                .HasForeignKey(cr => cr.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CompanyRequest>()
                .HasMany(cr => cr.Trainees)
                .WithMany(t => t.CompanyRequests)
                .UsingEntity<CompanyRequestDetail>(
                    j => j
                        .HasOne(crd => crd.Trainee)
                        .WithMany(t => t.CompanyRequestDetails)
                        .HasForeignKey(crd => crd.TraineeId),
                    j => j
                        .HasOne(crd => crd.CompanyRequest)
                        .WithMany(cr => cr.CompanyRequestDetails)
                        .HasForeignKey(crd => crd.CompanyRequestId),
                    j =>
                    {
                        j.HasKey(crd => new { crd.CompanyRequestId, crd.TraineeId });
                    }
                );

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Administrator"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = 3,
                    RoleName = "Trainer"
                },
                new Role
                {
                    RoleId = 4,
                    RoleName = "Trainee"
                },
                new Role
                {
                    RoleId = 5,
                    RoleName = "Company"
                }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    RoomId = 1,
                    RoomName = "B203"
                },
                new Room
                {
                    RoomId = 2,
                    RoomName = "B209"
                },
                new Room
                {
                    RoomId = 3,
                    RoomName = "B315"
                },
                new Room
                {
                    RoomId = 4,
                    RoomName = "G201"
                },
                new Room
                {
                    RoomId = 5,
                    RoomName = "G202"
                },
                new Room
                {
                    RoomId = 6,
                    RoomName = "G205"
                },
                new Room
                {
                    RoomId = 7,
                    RoomName = "G303"
                }
            );
        }
    }
}