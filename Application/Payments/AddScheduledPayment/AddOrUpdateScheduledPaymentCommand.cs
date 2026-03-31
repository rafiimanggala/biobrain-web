using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Security;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Domain.Entities.SiteIdentity;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.AddScheduledPayment
{
    [PublicAPI]
    public class SaveScheduledPaymentAndCardCommand : ICommand<SaveScheduledPaymentAndCardCommand.Result>
    {
	    public PaymentPeriods? Period { get; set; }
	    public List<Guid> Courses { get; set; }
		public Guid UserId { get; set; }
        public string CardToken { get; init; }
        public string IpAddress { get; set; }
        public Guid? PromoCodeId { get; set; }

        [PublicAPI]
        public class Result
        {
            
        }
		

        internal class Validator : ValidatorBase<SaveScheduledPaymentAndCardCommand>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.UserId).ExistsInTable(Db.Users);
            }
        }

		internal class PermissionCheck : PermissionCheckBase<SaveScheduledPaymentAndCardCommand>
        {
	        public PermissionCheck(ISecurityService securityService) : base(securityService)
	        {
	        }

	        protected override bool CanExecute(SaveScheduledPaymentAndCardCommand request, IUserSecurityInfo user)
	        {
		        if (!user.IsStudent()) return false;
		        return true;
	        }
        }


        internal class Handler : CommandHandlerBase<SaveScheduledPaymentAndCardCommand, Result>
        {
	        private readonly IScheduledPaymentService _scheduledPaymentService;
	        private readonly IPaymentService _paymentService;
            public Handler(IDb db, IScheduledPaymentService scheduledPaymentService, IPaymentService paymentService) : base(db)
            {
	            _scheduledPaymentService = scheduledPaymentService;
	            _paymentService = paymentService;
            }

            public override async Task<Result> Handle(SaveScheduledPaymentAndCardCommand request, CancellationToken cancellationToken)
            {
	            var student = await Db.Users.Where(x => x.Id == request.UserId).Include(x => x.Student).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                
				await AddOrUpdateUserPayment(student, request.CardToken, request.IpAddress);

				if (request.Period == null || request.Courses == null || request.Courses.Count < 1)
					return new Result();

				await DeleteAllInactiveScheduledPayments(request.UserId);
				//var schedulePayment = await HandleScheduledPayments(request.UserId);

				//if (schedulePayment == null)
				//{
				var scheduledPaymentId = Guid.NewGuid();
				ScheduledPaymentEntity scheduledPaymentEntity = await _scheduledPaymentService.AddScheduledPaymentAndPayAsync(new ScheduledPaymentEntity
				{
					ScheduledPaymentId = scheduledPaymentId,
					Status = ScheduledPaymentStatus.Created,
					Period = request.Period.Value,
                    Type = ScheduledPaymentType.Recurring,
					ScheduledPaymentCourses = request.Courses.Select(_ => new ScheduledPaymentCourseEntity
					{
						ScheduledPaymentCourseId = Guid.NewGuid(),
						CourseId = _,
						ScheduledPaymentId = scheduledPaymentId,
						Status = ScheduledPaymentCourseStatus.Active
					}).ToList(),
					UserId = request.UserId
				}, request.UserId, student.Student?.Country, request.PromoCodeId, cancellationToken);
				//}
				//else
				//{
				//	if (schedulePayment.Status == ScheduledPaymentStatus.PaymentFailed ||
				//	    schedulePayment.Status == ScheduledPaymentStatus.Inactive)
				//	{
				//		var result = await _scheduledPaymentService.UpdateScheduledPaymentAndPayAsync(new ScheduledPaymentEntity
				//		{
				//			ScheduledPaymentId = schedulePayment.ScheduledPaymentId,
				//			Status = schedulePayment.Status,
				//			Period = request.Period.Value,
				//			ScheduledPaymentCourses = request.Courses.Select(_ => new ScheduledPaymentCourseEntity
				//			{
				//				ScheduledPaymentCourseId = Guid.NewGuid(),
				//				CourseId = _,
				//				ScheduledPaymentId = schedulePayment.ScheduledPaymentId
				//			}).ToList(),
				//			UserId = request.UserId
				//		}, request.UserId, student.Student?.Country, cancellationToken);
				//	}
				//	else
				//		throw new ValidationException("Subscription edit not available yet");
				//}

				return new Result();
            }


            private async Task DeleteAllInactiveScheduledPayments(Guid userId)
            {
	            var entities = await Db.ScheduledPayment.Where(x => x.UserId == userId && x.DeletedAt == null && x.Status != ScheduledPaymentStatus.Success)
		            .ToListAsync();

				entities.ForEach(x => x.DeletedAt = DateTime.UtcNow);
				await Db.SaveChangesAsync();
			}

			//        private async Task<ScheduledPaymentEntity> HandleScheduledPayments(Guid userId)
			//        {
			//         var entities = await Db.ScheduledPayment.Where(x => x.UserId == userId && x.DeletedAt == null)
			//          .ToListAsync();

			//switch (entities.Count)
			//         {
			//                case 0: return null;
			//                case 1: return entities.First();
			//                default:
			//                {
			//                 var scheduledPayment = entities.OrderByDescending(x => x.CreatedAt).First();
			//                 foreach (var entity in entities)
			//                 {
			//                  if (entity == scheduledPayment) continue;
			//                  entity.DeletedAt = DateTime.UtcNow;
			//                 }

			//                 await Db.SaveChangesAsync();

			//                 return scheduledPayment;
			//                }
			//         }
			//        }

			private async Task AddOrUpdateUserPayment(UserEntity user, string cardToken, string ipAddress)
            {
	            var userPayment = await Db.UserPaymentDetails
		            .Where(x => x.UserId == user.Id && x.PaymentMethod == PaymentMethods.PinPayments)
		            .FirstOrDefaultAsync();

                if (userPayment == null)
                {
	                if (string.IsNullOrEmpty(cardToken))
		                throw new ValidationException("No payment details");

		            var customerToken =
			            await _paymentService.PostCustomerAsync(user.Email, cardToken, PaymentMethods.PinPayments);
		            Db.UserPaymentDetails.Add(new UserPaymentDetailsEntity
		            {
                        IpAddress = ipAddress,
                        PaymentMethod = PaymentMethods.PinPayments,
                        PinPaymentCustomerRefId = customerToken,
                        UserId = user.Id,
                        UserPaymentId = Guid.NewGuid()
                    });
	            }
                else
                {
	                userPayment.IpAddress = ipAddress;
	                await _paymentService.PutCustomerAsync(userPayment.PinPaymentCustomerRefId, cardToken, user.Email, PaymentMethods.PinPayments);
                }

                await Db.SaveChangesAsync();

            }
        }
    }
}