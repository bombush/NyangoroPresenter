using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nyangoro.Interfaces;
using System.ComponentModel.Composition;

namespace Nyangoro.Services
{
    public class MockService : IService
    {
        string whoami = "MockService";
    }
}
