using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Payments.AddScheduledPayment;
using Biobrain.Application.Payments.AddTrialScheduledPayment;
using Biobrain.Application.Payments.AddVoucherScheduledPayment;
using Biobrain.Application.Payments.CancelScheduledPayment;
using Biobrain.Application.Payments.CancelScheduledPaymentCourse;
using Biobrain.Application.Payments.GetCardToken;
using Biobrain.Application.Payments.GetPaymentMethods;
using Biobrain.Application.Payments.GetPromoCodeByCode;
using Biobrain.Application.Payments.GetScheduledPayments;
using Biobrain.Application.Payments.GetSubscriptionParameters;
using Biobrain.Application.Payments.UpdatePaymentDetails;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class PaymentController : Controller
	{
		private readonly IMediator _mediator;

		public PaymentController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
		public Task<ActionResult<List<GetPaymentMethodsQuery.Result>>> GetPaymentMethods(
			[FromQuery] GetPaymentMethodsQuery command)
			=> _mediator.Send(command).ToActionResult();

		[HttpPost]
		public Task<ActionResult<GetCardTokenCommand.Result>> GetCardToken(GetCardTokenCommand command)
			=> _mediator.Send(command).ToActionResult();

		[HttpGet]
		public Task<ActionResult<GetSubscriptionParametersQuery.Result>> GetSubscriptionParameters(
			[FromQuery] GetSubscriptionParametersQuery command)
			=> _mediator.Send(command).ToActionResult();

		[HttpGet]
		public Task<ActionResult<List<GetScheduledPaymentsQuery.Result>>> GetScheduledPayments(
			[FromQuery] GetScheduledPaymentsQuery command)
			=> _mediator.Send(command).ToActionResult();

		[HttpPost]
		public Task<ActionResult<SaveScheduledPaymentAndCardCommand.Result>> SaveScheduledPaymentAndCard(
			SaveScheduledPaymentAndCardCommand command)
		{
			if (Request.HttpContext.Connection.RemoteIpAddress != null)
				command.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

			return _mediator.Send(command).ToActionResult();
		}

		[HttpPost]
		public Task<ActionResult<AddScheduledPaymentCommand.Result>> AddScheduledPayment(
			AddScheduledPaymentCommand command) => _mediator.Send(command).ToActionResult();

		[HttpPost]
		public Task<ActionResult<UpdatePaymentDetailsCommand.Result>> UpdatePaymentDetails(
			UpdatePaymentDetailsCommand command)
		{
			if (Request.HttpContext.Connection.RemoteIpAddress != null)
				command.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

			return _mediator.Send(command).ToActionResult();
		}

        [HttpPost]
        public Task<ActionResult<AddTrialScheduledPaymentCommand.Result>> AddTrialScheduledPayment(
            AddTrialScheduledPaymentCommand command) => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<AddVoucherScheduledPaymentCommand.Result>> AddVoucherScheduledPayment(
            AddVoucherScheduledPaymentCommand command) => _mediator.Send(command).ToActionResult();

		[HttpPost]
		public Task<ActionResult<CancelScheduledPaymentCommand.Result>> CancelScheduledPayment(CancelScheduledPaymentCommand command) 
			=> _mediator.Send(command).ToActionResult();

		[HttpPost]
		public Task<ActionResult<CancelScheduledPaymentCourseCommand.Result>> CancelScheduledPaymentSubjects(CancelScheduledPaymentCourseCommand command) 
			=> _mediator.Send(command).ToActionResult();

		[HttpPost]
		public Task<ActionResult<GetPromoCodeByCodeCommand.Result>> GetPromoCodeByCode(GetPromoCodeByCodeCommand command) 
			=> _mediator.Send(command).ToActionResult();

	}
}
