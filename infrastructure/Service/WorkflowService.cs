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
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IModelService _modelService = modelService;
        public async Task<object> RunModelAsync(int userId, WorkflowRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UnauthorizedAccessException("User not found");
            if (user.Credits <= 0)
                throw new InvalidOperationException("Insufficient credits");

            // Decrement first
            user.Credits -= 1;
            await _context.SaveChangesAsync();

            try
            {
                var result = await _modelService.RunWorkflowAsync(request);
                return result;
            }
            catch
            {
                // Refund if failed
                user.Credits += 1;
                await _context.SaveChangesAsync();
                throw;
            }
        }
    }
}
