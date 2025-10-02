using FluentValidation;
using MapsterMapper;
using studeehub.Application.DTOs.Requests.WorkSpace;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class WorkSpaceService : IWorkSpaceService
	{
		private readonly IGenericRepository<WorkSpace> _repository;
		private readonly IValidator<CreateWorkSpaceRequest> _validator;
		private readonly IWorkSpaceRepository _workSpaceService;
		private readonly IMapper _mapper;

		public WorkSpaceService(IGenericRepository<WorkSpace> repository, IMapper mapper, IWorkSpaceRepository workSpaceService, IValidator<CreateWorkSpaceRequest> validator)
		{
			_repository = repository;
			_mapper = mapper;
			_workSpaceService = workSpaceService;
			_validator = validator;
		}

		public async Task<BaseResponse<string>> CreateWorkSpaceAsync(CreateWorkSpaceRequest requests)
		{
			var validationResult = await _validator.ValidateAsync(requests);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors);
			}

			var workSpace = _mapper.Map<WorkSpace>(requests);
			workSpace.CreatedAt = DateTime.UtcNow;

			if (string.IsNullOrWhiteSpace(requests.Name))
			{
				workSpace.Name = await _workSpaceService.GenerateUniqueWorkspaceNameAsync(requests.OwnerId);
			}

			await _repository.AddAsync(workSpace);
			var result = await _repository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("WorkSpace created successfully")
				: BaseResponse<string>.Fail("Failed to create WorkSpace");
		}
	}
}
