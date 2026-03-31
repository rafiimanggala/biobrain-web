using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;


namespace Biobrain.Application.Accounts.Services
{
    internal sealed class CreateAccountService : ICreateAccountService
    {
        private readonly UserManager<UserEntity> _userManager;

        public CreateAccountService(UserManager<UserEntity> userManager) => _userManager = userManager;

        public async Task<UserEntity> Create(ICreateAccountService.Request request, CancellationToken token)
        {
            var passwordValidationResult = await _userManager.ValidatePassword(nameof(request.Password), request.Password, null);
            if (!passwordValidationResult.IsValid)
                throw new ValidationException(passwordValidationResult.Errors);

            var user = new UserEntity { UserName = request.Email, Email = request.Email };

            var userValidationResult = await _userManager.ValidateUser(user);
            if (!userValidationResult.IsValid)
                throw new ValidationException(userValidationResult.Errors);

            await _userManager.CreateUser(user, request.Password);
            await _userManager.AssignUserToRoles(user, request.Roles.ToArray());

            return user;
        }
    }
}
