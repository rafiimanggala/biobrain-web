using System;

namespace Biobrain.Application.Common.Exceptions
{
    public class NotEnoughStudentsLicensesException : BusinessLogicException
    {
        public override Guid ErrorCode => new Guid("E6998AC0-A9A8-44D1-AC8A-4F3A6A7943BD");
    }
}