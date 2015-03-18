using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateForge.Examples.CallManager
{
    public interface IManagementAction
    {
        void Start();
        void Stop();
    }

    public interface IManagementEvent
    {
        void Started();
        void Stopped();
    }

}
