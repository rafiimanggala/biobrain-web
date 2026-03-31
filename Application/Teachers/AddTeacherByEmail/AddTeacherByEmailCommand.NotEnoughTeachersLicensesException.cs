using System;
using Biobrain.Application.Common.Exceptions;

namespace Biobrain.Application.Teachers.AddTeacherByEmail
{
    public partial class AddTeacherByEmailCommand
    {
        public class NotEnoughTeachersLicensesException : BusinessLogicException
        {
            public override Guid ErrorCode => new("58C88E79-7332-47E5-AF56-2309E208228B");
        }
    }
}