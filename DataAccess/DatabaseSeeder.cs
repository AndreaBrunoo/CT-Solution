using Sln.DataAccess.DataContext;
using DevExpress.Xpo;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.Services;

namespace Sln.DataAccess.DatabaseSeeder;
public static class DatabaseSeeder
{
    public static async Task SeedAsync(XpoDataContext ctx, CancellationToken ct)
    {
        //
        // ------------------------------------------------------------
        // 1ª TRANSAZIONE — CREA RUOLI, STATUS, CATEGORIE, COMPANY
        // ------------------------------------------------------------
        //
        await ctx.DoTranAsync(async uow =>
        {
            // RUOLI
            var roles = new[] { "Admin", "Manager", "User" };
            foreach (var name in roles)
            {
                if (!await uow.Query<XpoRole>().AnyAsync(r => r.Name == name, ct))
                    new XpoRole(uow) { Id = Guid.NewGuid(), Name = name };
            }

            // STATUS
            var statuses = new[] { "Bozza", "Inviato", "Approvato", "Rifiutato", "Fatturato" };
            foreach (var name in statuses)
            {
                if (!await uow.Query<XpoStatus>().AnyAsync(s => s.Name == name, ct))
                    new XpoStatus(uow) { Id = Guid.NewGuid(), Name = name };
            }

            // CATEGORIE
            var categories = new[] { "Sviluppo", "Analisi", "Bugfix", "Riunione" };
            foreach (var name in categories)
            {
                if (!await uow.Query<XpoCategory>().AnyAsync(c => c.Name == name, ct))
                    new XpoCategory(uow) { Id = Guid.NewGuid(), Name = name };
            }

            // COMPANY
            if (!await uow.Query<XpoCompany>().AnyAsync(c => c.Name == "Acme S.p.A.", ct))
            {
                new XpoCompany(uow)
                {
                    Id = Guid.NewGuid(),
                    Name = "Acme S.p.A.",
                    Email = "info@acme.it"
                };
            }

            return;
        });

        //
        // ------------------------------------------------------------
        // 2ª TRANSAZIONE — CREA PROJECT (ora la Company esiste davvero)
        // ------------------------------------------------------------
        //
        await ctx.DoTranAsync(async uow =>
        {
            var company = await uow.Query<XpoCompany>()
                .FirstOrDefaultAsync(c => c.Name == "Acme S.p.A.", ct);

            if (company == null)
            {
                // fallback: ricrea company
                company = new XpoCompany(uow)
                {
                    Id = Guid.NewGuid(),
                    Name = "Acme S.p.A.",
                    Email = "info@acme.it"
                };
            }

            if (!await uow.Query<XpoProject>().AnyAsync(p => p.Name == "Migrazione ERP", ct))
            {
                new XpoProject(uow)
                {
                    Id = Guid.NewGuid(),
                    Name = "Migrazione ERP",
                    HourlyRate = 10,
                    IdCompany = company.Id,
                    Company = company
                };
            }

            return;
        });

        //
        // ------------------------------------------------------------
        // 3ª TRANSAZIONE — CREA ADMIN + EMPLOYEE + WORKLOG DEMO
        // ------------------------------------------------------------
        //
        await ctx.DoTranAsync(async uow =>
        {
            // ADMIN USER
            var adminEmail = "admin@system.local";
            var admin = await uow.Query<XpoUser>()
                .FirstOrDefaultAsync(u => u.Email == adminEmail, ct);

            if (admin == null)
            {
                var passwordService = new PasswordService();
                var (hash, salt) = passwordService.HashPassword("Admin123!");

                admin = new XpoUser(uow)
                {
                    Id = Guid.NewGuid(),
                    Email = adminEmail,
                    PasswordHash = hash,
                    PasswordSalt = salt
                };
            }

            // RUOLO ADMIN
            var adminRole = await uow.Query<XpoRole>()
                .FirstOrDefaultAsync(r => r.Name == "Admin", ct);

            if (adminRole == null)
            {
                adminRole = new XpoRole(uow)
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin"
                };
            }

            if (!admin.Roles.Contains(adminRole))
                admin.Roles.Add(adminRole);

            // EMPLOYEE ADMIN
            var employee = await uow.Query<XpoEmployee>()
                .FirstOrDefaultAsync(e => e.UserName == "AdminUser", ct);

            if (employee == null)
            {
                employee = new XpoEmployee(uow)
                {
                    Id = Guid.NewGuid(),
                    UserName = "AdminUser",
                    User = admin
                };
            }

            // WORKLOG DEMO
            if (!await uow.Query<XpoWorkLog>().AnyAsync(w => w.Description == "Setup iniziale progetto", ct))
            {
                var category = await uow.Query<XpoCategory>()
                    .FirstOrDefaultAsync(c => c.Name == "Sviluppo", ct);

                var status = await uow.Query<XpoStatus>()
                    .FirstOrDefaultAsync(s => s.Name == "Bozza", ct);

                var project = await uow.Query<XpoProject>()
                    .FirstOrDefaultAsync(p => p.Name == "Migrazione ERP", ct);

                new XpoWorkLog(uow)
                {
                    Id = Guid.NewGuid(),
                    Name = "Worklog iniziale",
                    Description = "Setup iniziale progetto",
                    HoursCounter = 2,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IdProject = project.Id,
                    IdEmployee = employee.Id,
                    IdCategory = category.Id,
                    IdStatus = status.Id,
                    Project = project,
                    Employee = employee,
                    Category = category,
                    Status = status
                };
            }

            return;
        });
    }
}