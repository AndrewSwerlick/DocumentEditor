using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEditor.Core
{
    public class ConflictException : Exception
    {
        private readonly object[] _patchResult;

        public ConflictException(object[] patchResult)
            : base(patchResult[0].ToString())
        {
          
        }       
    }
}
