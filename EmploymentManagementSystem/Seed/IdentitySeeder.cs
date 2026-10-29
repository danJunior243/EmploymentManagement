using EmploymentManagement.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmploymentManagementSystem.Seed
{
    public static class IdentitySeeder
    {
        /// <summary>
        /// Crée les rôles et le compte administrateur au démarrage, s'ils n'existent pas.
        /// Peut être exécutée plusieurs fois sans créer de doublons.
        /// </summary>
        /// 

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            // 1. Les rôles
            string[] roles = { "Admin", "Employe" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Les identifiants de l'admin viennent des secrets, jamais du code
            var email = config["Admin:Email"];
            var password = config["Admin:Password"];

            // S'ils sont absents (ex. serveur mal configuré), on sort sans créer de compte
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            // 3. Si l'admin existe déjà, il n'y a rien à faire
            var admin = await userManager.FindByEmailAsync(email);
            if (admin != null) return;

            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);

            // Si le mot de passe est refusé, on veut le savoir tout de suite
            if (!result.Succeeded)
            {
                var erreurs = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Création de l'admin impossible : {erreurs}");
            }

            await userManager.AddToRoleAsync(admin, "Admin");
        }

    }
}
