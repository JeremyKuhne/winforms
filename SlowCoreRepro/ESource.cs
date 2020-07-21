using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slow471Repro
{
    [EventSource(Name = "TestEventSource")]
    public class TestEventSource : EventSource
    {
        public static TestEventSource Log = new TestEventSource();

        [Event(1)]
        public void TestStart() { WriteEvent(1); }
        [Event(2)]
        public void TestStop() { WriteEvent(2); }
    }
}
