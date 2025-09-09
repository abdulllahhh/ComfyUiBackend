using infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models.Dtos.Request;
using Models.Entities;
using Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Service
{
    public class WorkflowService(ApplicationDbContext context, UserManager<AppUser> userManager, IModelService modelService) : IWorkflowService
    {

        public async Task<object> RunModelAsync(int userId, WorkflowRequest request)
        {
            var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UnauthorizedAccessException("User not found");
            if (user.Credits <= 0)
                throw new InvalidOperationException("Insufficient credits");

            // Decrement first
            user.Credits -= 1;
            await context.SaveChangesAsync();

            try
            {
                var result = await modelService.RunWorkflowAsync(request);
                return result;
            }
            catch
            {
                // Refund if failed
                user.Credits += 1;
                await context.SaveChangesAsync();
                throw;
            }
        }
    }
}
