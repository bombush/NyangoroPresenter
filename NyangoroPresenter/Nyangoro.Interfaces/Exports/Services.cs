using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Interfaces
{
    [InheritedExport(typeof(Nyangoro.Interfaces.IService))]
    public interface IService { }

    [InheritedExport(typeof(Nyangoro.Interfaces.IServiceHolder))]
    public interface IServiceHolder { 
        IService getByType(string type);
    }
}
