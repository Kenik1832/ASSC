using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ASSC.Models;

namespace ASSC.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var context =
                serviceProvider.GetRequiredService<ApplicationDbContext>();

            var roles = new[] { "Admin", "Contractor", "Supplier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            await EnsureUserAsync(
                userManager,
                "kenik_m@mail.ru",
                "Po4cx3h5!",
                "Admin");

            var contractorUser = await EnsureUserAsync(
                userManager,
                "pod@mail.ru",
                "Po4cx3h5!",
                "Contractor");
            
            var supplierUser = await EnsureUserAsync(
                userManager,
                "pos@mail.ru",
                "Po4cx3h5!", 
                "Supplier");

            if (!context.Suppliers.Any())
            {
                context.Suppliers.Add(new Supplier
                {
                    Name = "Поставщик 1",
                    INN = "1234567890",
                    ContactInfo = "contact@supplier.local"
                });
            }

            if (!context.Contractors.Any())
            {
                context.Contractors.Add(new Contractor
                {
                    Name = "Подрядчик 1",
                    INN = "0987654321",
                    ContactInfo = "contact@contractor.local"
                });
            }

            await context.SaveChangesAsync();

            var supplier = await context.Suppliers.FirstAsync();
            var contractor = await context.Contractors.FirstAsync();

            if (!context.Contracts.Any())
            {
                context.Contracts.Add(new Contract
                {
                    Number = "C-001",
                    StartDate = DateTime.Today.AddMonths(-1),
                    EndDate = DateTime.Today.AddMonths(11),
                    SupplierId = supplier.Id,
                    ContractorId = contractor.Id
                });
            }

            await context.SaveChangesAsync();

            if (!context.Invoices.Any())
            {
                var contract = await context.Contracts.FirstAsync();

                context.Invoices.Add(new Invoice
                {
                    ContractId = contract.Id,
                    Amount = 100000M,
                    IssueDate = DateTime.Today.AddDays(-10),
                    DueDate = DateTime.Today.AddDays(20),
                    Status = "Unpaid"
                });
            }

            await context.SaveChangesAsync();

            if (contractorUser != null && contractorUser.ContractorId == null)
            {
                contractorUser.ContractorId = contractor.Id;
                await userManager.UpdateAsync(contractorUser);
            }
            if (supplierUser != null && supplierUser.SupplierId == null)
            {
                supplierUser.SupplierId = supplier.Id;
                await userManager.UpdateAsync(supplierUser);
            }
        }

        private static async Task<ApplicationUser?> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }

                return user;
            }

            user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                Console.WriteLine($"Не удалось создать пользователя {email}:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }

                return null;
            }

            await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}