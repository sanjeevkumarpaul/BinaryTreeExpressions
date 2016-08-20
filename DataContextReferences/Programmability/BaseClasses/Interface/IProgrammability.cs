using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.BaseClasses.Interface
{
    public interface IProgrammability
    {    
        object Execute(DbContext context);       
    }
}