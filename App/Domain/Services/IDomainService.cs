using App.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Domain.Services
{
    /// <summary>
    /// This interface must be implemented by all domain services to identify them by convention.
    /// </summary>
    public interface IDomainService : ITransientDependency
    {
    }
}
