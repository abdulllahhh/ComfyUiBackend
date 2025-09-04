using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos.Request
{
    public class WorkflowRequest
    {
        public string Prompt { get; set; }
        public int Seed { get; set; } = 1234;
        public int Steps { get; set; } = 20;
        public int Cfg { get; set; } = 8;
        //public string FilenamePrefix { get; set; } = "run001";
    }
}
