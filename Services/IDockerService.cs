using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Services
{
    public interface IDockerService
    {
        (string output, string error) RunDockerCommand(string arguments);
    }
}
