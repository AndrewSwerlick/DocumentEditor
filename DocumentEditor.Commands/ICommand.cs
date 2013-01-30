using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;

namespace DocumentEditor.Commands
{
    public interface ICommand
    {
        IDocumentSession Session { get; set; }

        void Execute();
    }
}
