using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateForge.Examples.CallManager
{
    interface ICallAction
    {
        void Setup(string from, string to, string token);
        void Answer(string token);
        void Connect(string token);
        void Release(string token);
    }
}
