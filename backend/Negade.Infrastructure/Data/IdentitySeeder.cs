using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Negade.Domain.Entities;
using Negade.Domain.Security;

namespace Negade.Infrastructure.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<AppUser> userManager,
        IConfiguration configuration)
    {
        foreach (var role in new[] { AppRoles.Admin, AppRoles.Trader })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var seed = configuration.GetSection("Identity:SeedAdmin");
        var phoneNumber = seed["PhoneNumber"];
        var userName = seed["UserName"] ?? phoneNumber;
        var password = seed["Password"];

        if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var user = userManager.Users.FirstOrDefault(candidate => candidate.PhoneNumber == phoneNumber);
        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                FullName = seed["FullName"] ?? "Negade Administrator",
                UserName = userName,
                PhoneNumber = phoneNumber,
                Email = seed["Email"],
                EmailConfirmed = !string.IsNullOrWhiteSpace(seed["Email"]),
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not seed admin user: {string.Join("; ", createResult.Errors.Select(error => error.Description))}");
            }
        }
        else if (!string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase))
        {
            var existingNamedUser = await userManager.FindByNameAsync(userName);
            if (existingNamedUser is null)
            {
                user.UserName = userName;
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not update seeded admin username: {string.Join("; ", updateResult.Errors.Select(error => error.Description))}");
                }
            }
        }

        if (!await userManager.IsInRoleAsync(user, AppRoles.Admin))
        {
            await userManager.AddToRoleAsync(user, AppRoles.Admin);
        }
    }
}
