using System;
using System.Text;
using System.Collections.Generic;

namespace Edger.Unity {
    public class EdgerException : Exception {
        public EdgerException(string msg)
                    : base(msg) {
        }

        public EdgerException(string format, params object[] values)
                    : base(Log.GetMsg(format, values)) {
        }

        public EdgerException(Exception innerException,
                                string msg)
                    : base(msg, innerException) {
        }

        public EdgerException(Exception innerException,
                                string format, params object[] values)
                    : base(Log.GetMsg(format, values), innerException) {
        }
    }

    public class CriticalException : EdgerException {
        public CriticalException(string msg)
                    : base(msg) {
        }
    }
}
