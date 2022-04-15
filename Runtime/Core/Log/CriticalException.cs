using System;
using System.Text;
using System.Collections.Generic;

namespace Edger.Unity {
    public class CriticalException : Exception {
        public CriticalException(string msg)
                    : base(msg) {
        }
        public CriticalException(string format, params object[] values)
                    : base(Log.GetMsg(format, values)) {
        }

        public CriticalException(Exception innerException,
                                string msg)
                    : base(msg, innerException) {
        }

        public CriticalException(Exception innerException,
                                string format, params object[] values)
                    : base(Log.GetMsg(format, values), innerException) {
        }
    }
}
