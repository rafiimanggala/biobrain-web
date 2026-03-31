using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SiteIdentity;
using BiobrainWebAPI.Values;
using BiobrainWebAPI.Values.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BiobrainWebAPI.Authorization
{
    public class DefaultUserDataSeed
    {
        private readonly string[] defaultRoles = {Constant.Roles.SystemAdministrator, Constant.Roles.SchoolAdministrator, Constant.Roles.Teacher, Constant.Roles.Student};

        private readonly ILogger<DefaultUserDataSeed> logger;
        private readonly RoleManager<RoleEntity> roleManager;
        private readonly UserManager<UserEntity> userManager;
        private readonly IConfiguration configuration;

        public DefaultUserDataSeed(ILogger<DefaultUserDataSeed> logger,
                                   RoleManager<RoleEntity> roleManager,
                                   UserManager<UserEntity> userManager,
                                   IConfiguration configuration)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public async Task Seed()
        {
            await SeedUserRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedUserRolesAsync()
        {
            var roles = defaultRoles;

            foreach (var role in roles)
            {
                try
                {
                    var isExist = await roleManager.RoleExistsAsync(role);
                    if (isExist) continue;

                    await roleManager.CreateAsync(new RoleEntity(role));
                    logger.LogInformation($"Created role {role}");
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            var adminOption = configuration.GetSection(ConfigurationSections.Admin).Get<UserOption>();
            if (adminOption != null)
                await SeedUser(adminOption, Constant.Roles.SystemAdministrator);

            var studentsOptions = configuration.GetSection(ConfigurationSections.Students).Get<List<UserOption>>();
            if (studentsOptions != null)
            {
                foreach (var studentsOption in studentsOptions)
                {
                    await SeedUser(studentsOption, Constant.Roles.Student);
                }
            }

            var teachersOptions = configuration.GetSection(ConfigurationSections.Teachers).Get<List<UserOption>>();
            if (teachersOptions != null)
            {
                foreach (var teacherOption in teachersOptions)
                {
                    await SeedUser(teacherOption, Constant.Roles.Teacher);
                }
            }
        }

        private async Task SeedUser(UserOption userOption, string role)
        {
            var existsUser = await userManager.FindByNameAsync(userOption.UserName);

            if (existsUser == null)
            {
                try
                {
                    var user = new UserEntity {UserName = userOption.UserName, Email = userOption.UserName};

                    var result = await userManager.CreateAsync(user, userOption.Password);
                    if (!result.Succeeded)
                        throw new Exception(result.ToString());

                    result = await userManager.AddToRoleAsync(user, role);
                    if (!result.Succeeded)
                        throw new Exception(result.ToString());

                    logger.LogInformation($"Created user with username {user.UserName} and role {role}");
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }
            }
        }
    }
}