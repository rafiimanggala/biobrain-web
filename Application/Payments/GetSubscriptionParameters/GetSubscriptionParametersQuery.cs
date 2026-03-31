using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Courses;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Payments.Models;
using Biobrain.Application.Security;
using Biobrain.Application.Values;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.Payment;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Payments.GetSubscriptionParameters
{
    [PublicAPI]
    public class GetSubscriptionParametersQuery : ICommand<GetSubscriptionParametersQuery.Result>
    {
        public Guid StudentId { get; init; }
        public Guid? SubscriptionId { get; set; }

        [PublicAPI]
        public class Result
        {
            //public List<Product> Products { get; set; }
            //public List<Product> AdditionalProducts { get; set; }
            public List<Price> Prices { get; set; }
            public List<Subject> Subjects { get; set; }
            public List<Subject> AdditionalSubjects { get; set; }
            public List<Curriculum> Curricula { get; set; }
            public PaymentPeriods? SelectedPaymentPeriod { get; set; }
            public int? UserCurriculumCode { get; set; }
            public string Currency { get; set; }
            public string Country { get; set; }
            public Guid? VoucherId { get; set; }
            public double? VoucherAmount { get; set; } 
        }

        [PublicAPI]
        public record Curriculum
        {
            public int CurriculumCode { get; set; }
            public string Name { get; set; }
            public bool IsGeneric { get; set; }
            public List<int> Years { get; set; }
        }

        [PublicAPI]
        public record Product
        {
            public Guid ProductId { get; init; }
            public string Name { get; init; }
            public int CurriculumCode { get; init; }
            public int? Year { get; init; }
            public int SubjectCode { get; init; }
            public bool IsComingSoon { get; set; }
        }

        [PublicAPI]
        public record Subject
        {
            public int SubjectCode { get; init; }
            public string Name { get; init; }
            public bool IsSelected { get; init; }
            public bool IsNeedYearSelection { get; init; }
            //public int? Year { get; init; }
            public int CurriculumCode { get; init; }
            public List<Product> Products { get; set; }
            public Product SelectedProduct { get; set; }
        }

        [PublicAPI]
        public record Price
        {
            public int SubjectsNumber { get; init; }
            public PaymentPeriods Period { get; init; }
            public double Value { get; init; }
            public bool IsDisplayed { get; set; }
        }


        internal class Validator : ValidatorBase<GetSubscriptionParametersQuery>
        {
            public Validator(IDb db) : base(db) => RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
        }


        internal class PermissionCheck(ISecurityService securityService) : PermissionCheckBase<GetSubscriptionParametersQuery>(securityService)
        {
            protected override bool CanExecute(GetSubscriptionParametersQuery request, IUserSecurityInfo user)
            {
                if (!user.IsStudent()) return false;
                if (!user.IsAccountOwner(request.StudentId)) return false;
                return true;
            }
        }


        internal class Handler(IDb db, IScheduledPaymentService scheduledPaymentService)
            : CommandHandlerBase<GetSubscriptionParametersQuery, Result>(db)
        {
            private readonly IScheduledPaymentService _scheduledPaymentService = scheduledPaymentService;

            public override async Task<Result> Handle(GetSubscriptionParametersQuery request, CancellationToken cancellationToken)
            {
                var student = await Db.Students.Where(x => x.StudentId == request.StudentId)
                    .FirstOrDefaultAsync(cancellationToken);
                ScheduledPaymentEntity currentSubscription = null;
                if (request.SubscriptionId != null)
                    currentSubscription = await Db.ScheduledPayment.AsNoTracking()
                        .Where(_ => _.UserId == student.StudentId)
                        .Include(_ => _.ScheduledPaymentCourses).ThenInclude(_ => _.Course)
                        .FirstOrDefaultAsync(cancellationToken);
                var courses = await Db.Courses.AsNoTracking()
                    .Where(x => x.IsForSell)
                    .Include(x => x.Subject).Include(x => x.Curriculum)
                    .ToListAsync(cancellationToken);
                var subjects = await Db.Subjects.AsNoTracking().ToListAsync(cancellationToken);
                var products = _scheduledPaymentService.GetProductsByCurriculumAvailability(student.CurriculumCode ?? 0);

                //var products = FilterCoursesByAvailability(courses, student);
                var currency = CountryCurrency.Get(student.Country);

                var isAdditionalProductsAvailable = _scheduledPaymentService.GetYear10CoursesAvailability(student.Country, student.CurriculumCode ?? 0);
                var userVoucher = await Db.UserVouchers.AsNoTracking().Include(_ => _.Voucher)
                    .Where(_ => _.UserId == student.StudentId && _.Voucher.RedeemExpiryDateUtc > DateTime.UtcNow)
                    .FirstOrDefaultAsync(cancellationToken);

                return new Result
                {
                    //Products = products.Where(_ => _.Group == CourseGroup.Main).Select(x =>
                    //    new Product
                    //    {
                    //        Name = CourseHelper.GetCourseName(x),
                    //        ProductId = x.CourseId,
                    //        CurriculumCode = x.CurriculumCode,
                    //        Year = x.Year,
                    //        SubjectCode = x.SubjectCode
                    //    }).ToList(),
                    //AdditionalProducts = isAdditionalProductsAvailable
                    //    ? products.Where(_ => _.Group == CourseGroup.Additional).Select(x =>
                    //    new Product
                    //    {
                    //        Name = CourseHelper.GetCourseName(x),
                    //        ProductId = x.CourseId,
                    //        CurriculumCode = x.CurriculumCode,
                    //        Year = x.Year,
                    //        SubjectCode = x.SubjectCode
                    //    }).ToList()
                    //: new List<Product>(),

                    Prices = [.._scheduledPaymentService.GetPrices()
                                                        .Where(x => x.Currency == currency.Key)
                                                        .Select(x => new Price
                                                                     {
                                                                         Value = x.Value,
                                                                         Period = x.Period,
                                                                         SubjectsNumber = x.SubjectsNumber,
                                                                         IsDisplayed = x.IsDisplayed
                                                                     })],

                    Subjects = [..products.Select(_ => new {model = _, entity = subjects.First(s => s.SubjectCode == _.SubjectCode)})
                                          .Select(_ => new Subject {
                                                                       Name = _.entity.Name,
                                                                       SubjectCode = _.entity.SubjectCode,
                                                                       IsSelected = IsSubjectSelected(_.entity.SubjectCode, currentSubscription),
                                                                       //Year = GetYear(_.Subject.SubjectCode, currentSubscription),
                                                                       IsNeedYearSelection = _.model.Courses.Count > 1 || student.CurriculumCode == Constant.Curriculum.Ap,
                                                                       CurriculumCode = student.CurriculumCode ?? 0,
                                                                       Products = GetProducts(courses, _.model.Courses),
                                                                       SelectedProduct = GetSelectedProduct(currentSubscription, _.model.Courses)
                                                                   })],

                    AdditionalSubjects = isAdditionalProductsAvailable
                    ? [..courses.Where(_ => _.Group == CourseGroup.Additional)
                             .DistinctBy(_ => _.Subject.SubjectCode)
                             .Select(_ => new Subject {
                                                          Name = _.Subject.Name,
                                                          SubjectCode = _.SubjectCode,
                                                          IsSelected = IsSubjectSelected(_.SubjectCode, currentSubscription),
                                                          //Year = GetYear(_.SubjectCode, currentSubscription),
                                                          IsNeedYearSelection = false,
                                                          CurriculumCode = _.CurriculumCode,
                                                          Products = new List<Product>{
                                                                                          new ()
                                                                                          {
                                                                                              ProductId = _.CourseId,
                                                                                              Name = CourseHelper.GetCourseName(_),
                                                                                              CurriculumCode = _.CurriculumCode,
                                                                                              SubjectCode = _.SubjectCode,
                                                                                              Year = _.Year
                                                                                          }
                                                                                      },
                                                          SelectedProduct = GetSelectedProduct(
                                                              currentSubscription,
                                                              [new() { CourseId = _.CourseId, CourseName = CourseHelper.GetCourseName(_) }])
                                                      })]
                    : [],

                    Curricula = await Db.Curricula.Select(_ => new Curriculum
                    {
                        CurriculumCode = _.CurriculumCode,
                        Name = _.Name,
                        Years = GetAvailableYears(_.CurriculumCode),
                        IsGeneric = _.IsGeneric
                    }).ToListAsync(cancellationToken),

                    UserCurriculumCode = student.CurriculumCode,
                    SelectedPaymentPeriod = currentSubscription?.Period,
                    Currency = currency.Value,
                    Country = student.Country,
                    VoucherId = userVoucher?.VoucherId,
                    VoucherAmount = userVoucher == null ? null : (userVoucher.Voucher.TotalAmount - userVoucher.Voucher.AmountUsed)
                };
            }

            private Product GetSelectedProduct(ScheduledPaymentEntity currentSubscription, List<CourseProductModel> courses)
            {
                var selectedCourse = currentSubscription?.ScheduledPaymentCourses
                    .FirstOrDefault(_ => courses.Any(c => c.CourseId ==_.Course.CourseId))?.Course;

                if (selectedCourse == null) return null;
                var course = courses.First(_ => _.CourseId == selectedCourse.CourseId);

                return new Product
                {
                    CurriculumCode = selectedCourse.CurriculumCode,
                    ProductId = selectedCourse.CourseId,
                    Name = course.CourseName,
                    SubjectCode = selectedCourse.SubjectCode,

                };
            }

            private static List<Product> GetProducts(List<CourseEntity> courses, List<CourseProductModel> courseProductModels)
            {
                var products = new List<Product>();
                foreach (var productModel in courseProductModels)
                {
                    var course = courses.FirstOrDefault(_ => _.CourseId == productModel.CourseId);
                    if(course == null && !productModel.IsComingSoon) continue;

                    products.Add(new Product
                    {
                        CurriculumCode = course?.CurriculumCode ?? -1,
                        ProductId = productModel.CourseId,
                        Name = productModel.CourseName,
                        SubjectCode = course?.SubjectCode ?? -1,
                        IsComingSoon = productModel.IsComingSoon,
                    });
                }
                return products;
            }

            private bool IsSubjectSelected(int subjectCode, ScheduledPaymentEntity subscription) =>
                subscription?.ScheduledPaymentCourses.Any(_ => _.Course.SubjectCode == subjectCode) ?? false;

            //private bool IsNeedYearSelection(int curriculumCode)
            //{
            //    switch (curriculumCode)
            //    {
            //        case 1:
            //        case 2: return true;
            //        default: return false;
            //    }
            //}

            //private int? GetYear(int subjectCode, ScheduledPaymentEntity subscription)
            //{
            //    var year = subscription?.ScheduledPaymentCourses.FirstOrDefault(_ => _.Course.SubjectCode == subjectCode)?.Course
            //        .Year;
            //    return year == 0 ? null : year;
            //}

            //private List<CourseEntity> FilterCoursesByAvailability(List<CourseEntity> courses, StudentEntity student)
            //{
            //    var coursesAvailability = _scheduledPaymentService.GetCoursesAvailability();
            //    var filtered = courses.Where(x =>
            //    {
            //        var courseAvailability = coursesAvailability.FirstOrDefault(ca => ca.CourseId == x.CourseId);
            //        // If not in the list then available for all
            //        if (courseAvailability == null) return true;

            //        var country = courseAvailability.Countries.FirstOrDefault(_ => _.CountryName == student.Country);
            //        if (country != null)
            //        {
            //            // If no regions restricted then available for all regions
            //            if (country.Regions == null || country.Regions.Count == 0) return true;

            //            if (country.Regions.Any(_ => _ == student.State)) return true;
            //        }

            //        return false;
            //    }).ToList();
            //    return filtered;
            //}

            //private IEnumerable<CourseEntity> FilterCourseForCurriculum(IEnumerable<CourseEntity> courses, int? userCurriculumCode)
            //{
            //    if (userCurriculumCode == null) return courses.Where(_ => _.IsBase);
            //    var result = new List<CourseEntity>();

            //    var coursesBySubject = courses.GroupBy(_ => _.SubjectCode);
            //    foreach (var subjectCourses in coursesBySubject)
            //    {
            //        if (!subjectCourses.Any()) continue;
            //        var courseForCurriculum =
            //            subjectCourses.FirstOrDefault(_ => _.CurriculumCode == userCurriculumCode) ?? subjectCourses.First();

            //        result.Add(courseForCurriculum);
            //    }

            //    return result;
            //}

            private static List<int> GetAvailableYears(int curriculumCode)
            {
                return curriculumCode switch
                {
                    1 => [11, 12],
                    2 => [11, 12],
                    _ => null
                };
            }
        }
    }
}