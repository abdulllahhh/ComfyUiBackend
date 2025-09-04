using Models.Dtos.Request;
using Models.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interface
{
    public interface IModelService
    {
        Task<WorkflowResponse> RunWorkflowAsync(WorkflowRequest request);
    }
}
