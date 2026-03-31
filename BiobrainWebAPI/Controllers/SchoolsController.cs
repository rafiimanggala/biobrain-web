using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Application.Schools.CreateSchool;
using Biobrain.Application.Schools.DeleteSchool;
using Biobrain.Application.Schools.GetSchoolById;
using Biobrain.Application.Schools.GetSchoolLicenseInfo;
using Biobrain.Application.Schools.GetSchoolListItems;
using Biobrain.Application.Schools.GetSchools;
using Biobrain.Application.Schools.UpdateSchoolAdmins;
using Biobrain.Application.Schools.UpdateSchoolDetails;
using Biobrain.Application.Schools.UpdateSchoolLicenses;
using BiobrainWebAPI.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SchoolsController : Controller
    {
        private readonly IMediator _mediator;

        public SchoolsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<ActionResult<CreateSchoolCommand.Result>> CreateSchool(CreateSchoolCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateSchoolDetailsCommand.Result>> UpdateSchoolDetails(UpdateSchoolDetailsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateSchoolAdminsCommand.Result>> UpdateSchoolAdmins(UpdateSchoolAdminsCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<UpdateSchoolLicensesCommand.Result>> UpdateSchoolLicenses(UpdateSchoolLicensesCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<DeleteSchoolCommand.Result>> DeleteSchool(DeleteSchoolCommand command)
            => _mediator.Send(command).ToActionResult();

        [HttpPost]
        public Task<ActionResult<List<GetSchoolListQuery.Result>>> Get(GetSchoolListQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetSchoolByIdQuery.Result>> GetById([FromQuery] GetSchoolByIdQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<List<GetSchoolListItemsQuery.Result>>> GetAsListItems([FromQuery] GetSchoolListItemsQuery query)
            => _mediator.Send(query).ToActionResult();

        [HttpGet]
        public Task<ActionResult<GetSchoolLicenseInfoQuery.Result>> GetSchoolLicenseInfo([FromQuery] GetSchoolLicenseInfoQuery query)
            => _mediator.Send(query).ToActionResult();
    }
}
