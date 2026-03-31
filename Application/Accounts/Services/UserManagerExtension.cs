using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Exceptions;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Services
{
    internal static class UserManagerExtension
    {
        public static async Task<UserEntity> GetUserById(this UserManager<UserEntity> userManager, Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ObjectWasNotFoundException(nameof(UserEntity), id);

            return user;
        }
        
        public static async Task<UserEntity> GetUserByLoginName(this UserManager<UserEntity> userManager, string loginName)
        {
            var user = await userManager.FindUserByLoginName(loginName);
            if (user == null)
                throw new ObjectWasNotFoundException(nameof(UserEntity), null);

            return user;
        }

        public static async Task<UserEntity> FindUserByLoginName(this UserManager<UserEntity> userManager, string loginName) => await userManager.FindByNameAsync(loginName);

        public static async Task<ValidationResult> ValidatePassword(this UserManager<UserEntity> userManager, string propertyName, string password, UserEntity user)
        {
            var failures = new List<ValidationFailure>();

            foreach (var validator in userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(userManager, user, password);
                if (validationResult.Succeeded) continue;
                failures.AddRange(validationResult.Errors.Select(error => new ValidationFailure(propertyName, error.Description) { ErrorCode = error.Code }));
            }

            return new ValidationResult(failures);
        }

        public static async Task<ValidationResult> ValidateUser(this UserManager<UserEntity> userManager, UserEntity user)
        {
            var failures = new List<ValidationFailure>();
            
            foreach (var validator in userManager.UserValidators)
            {
                var validationResult = await validator.ValidateAsync(userManager, user);
                if (validationResult.Succeeded) continue;
                failures.AddRange(validationResult.Errors.Select(error => new ValidationFailure(string.Empty, error.Description) { ErrorCode = error.Code }));
            }

            return new ValidationResult(failures);
        }

        public static async Task CreateUser(this UserManager<UserEntity> userManager, UserEntity user)
        {
            var createUserResult = await userManager.CreateAsync(user);

            if (createUserResult.Errors.Any())
                throw new CanNotCreateUserException(createUserResult.Errors);
        }

        public static async Task CreateUser(this UserManager<UserEntity> userManager, UserEntity user, string password)
        {
            var createUserResult = await userManager.CreateAsync(user, password);
            if (!createUserResult.Succeeded)
                throw new CanNotCreateUserException(createUserResult.Errors);
        }

        public static async Task AssignUserToRoles(this UserManager<UserEntity> userManager, UserEntity user, params string[] roles)
        {
            var addRoleResult = await userManager.AddToRolesAsync(user, roles);
            if (addRoleResult.Errors.Any())
                throw new CanNotAddRoleToUserException(addRoleResult.Errors);
        }
    }
}