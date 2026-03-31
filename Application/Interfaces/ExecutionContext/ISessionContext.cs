using System;

namespace Biobrain.Application.Interfaces.ExecutionContext
{
    public interface ISessionContext
    {
        bool IsUserAuthenticated { get; }
        Guid GetUserId();
        bool IsUserInRole(string role);
        bool IsFromSchool(Guid schoolId);
    }
}
