using System.Threading.Tasks;
using Biobrain.Application.Accounts.Commands;
using Biobrain.Application.Accounts.Queries;
using Biobrain.Application.Students.JoinStudentToClass;
using Biobrain.Application.Students.SignUpStandaloneStudent;
using Biobrain.Application.Students.SignUpStandaloneStudentWithAccessCode;
using Biobrain.Application.Students.SignUpStandaloneStudentWithVoucher;
using Biobrain.Application.Students.SignUpStudent;
using Biobrain.Application.Students.UseAccessCode;
using Biobrain.Application.Teachers.SingUpTeacher;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<SignUpTeacherCommand.Result>> SignUpTeacher(SignUpTeacherCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<ResetSelfPasswordCommand.Result>> ResetSelfPassword(ResetSelfPasswordCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ResetPasswordCommand.Result>> ResetPassword(ResetPasswordCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SetPasswordCommand.Result>> SetPassword(SetPasswordCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ChangeSelfPasswordCommand.Result>> ChangeSelfPassword(ChangeSelfPasswordCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ChangePasswordCommand.Result>> ChangePassword(ChangePasswordCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SignUpStudentCommand.Result>> SignUpStudent(SignUpStudentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SignUpStandaloneStudentWithAccessCodeCommand.Result>> SignUpStudentWithAccessCode(SignUpStandaloneStudentWithAccessCodeCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SignUpStandaloneStudentCommand.Result>> SignUpStandaloneStudent(SignUpStandaloneStudentCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<SignUpStandaloneStudentWithVoucherCommand.Result>> SignUpStudentWithVoucher(SignUpStandaloneStudentWithVoucherCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetStudentAccountStateQuery.Result>> GetStudentAccountState([FromQuery] GetStudentAccountStateQuery command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<JoinStudentToClassCommand.Result>> JoinStudentToClass(JoinStudentToClassCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<UseAccessCodeCommand.Result>> UseAccessCode(UseAccessCodeCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ChangeEmailCommand.Result>> ChangeEmail(ChangeEmailCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<ChangeSelfEmailCommand.Result>> ChangeSelfEmail(ChangeSelfEmailCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteUserCommand.Result>> DeleteUser(DeleteUserCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<DeleteUserPermanentCommand.Result>> DeleteUserPermanent(DeleteUserPermanentCommand command)
	        => _mediator.Send(command).ToActionResult();

        [HttpPost]
        [Authorize]
        public Task<ActionResult<SaveUserLogCommand.Result>> SaveUserLog(SaveUserLogCommand command)
	        => _mediator.Send(command).ToActionResult();
    }
}
