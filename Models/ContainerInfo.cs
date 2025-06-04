using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockyCLI.Models
{
    public class ContainerInfo
    {   
        public string ContainerId { get; set; } = "";
        public string Image { get; set; } = "";
        public string Command { get; set; } = "";
        public string Created { get; set; } = "";
        public string Status { get; set; } = "";
        public string Ports { get; set; } = "";
        public string Names { get; set; } = "";
    }
}
