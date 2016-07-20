using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nyangoro.Interfaces;
using System.ComponentModel.Composition;

namespace Nyangoro.Services
{
    abstract public class MockService : IService
    {

    }

    public class MockServiceChild : MockService
    {
        string whoami = "MockServiceChild";
    }
}
