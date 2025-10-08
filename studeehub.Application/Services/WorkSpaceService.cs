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
		private readonly IValidator<CreateWorkSpaceRequest> _createValidator;
		private readonly IValidator<UpdateWorkSpaceRequest> _updateValidator;
		private readonly IWorkSpaceRepository _workSpaceService;
		private readonly IMapper _mapper;

		public WorkSpaceService(IGenericRepository<WorkSpace> repository, IMapper mapper, IWorkSpaceRepository workSpaceService, IValidator<CreateWorkSpaceRequest> createValidator, IValidator<UpdateWorkSpaceRequest> updateValidator)
		{
			_repository = repository;
			_mapper = mapper;
			_workSpaceService = workSpaceService;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
		}

		public async Task<BaseResponse<string>> CreateWorkSpaceAsync(CreateWorkSpaceRequest requests)
		{
			var validationResult = await _createValidator.ValidateAsync(requests);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, Domain.Enums.ErrorType.Validation);
			}

			var workSpace = _mapper.Map<WorkSpace>(requests);

			if (string.IsNullOrWhiteSpace(requests.Name))
			{
				workSpace.Name = await _workSpaceService.GenerateUniqueWorkspaceNameAsync(requests.OwnerId);
			}

			await _repository.AddAsync(workSpace);
			var result = await _repository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("WorkSpace created successfully")
				: BaseResponse<string>.Fail("Failed to create WorkSpace", Domain.Enums.ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> UpdateWorkSpaceAsync(Guid id, UpdateWorkSpaceRequest requests)
		{
			var validationResult = _updateValidator.Validate(requests);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, Domain.Enums.ErrorType.Validation);
			}

			var existingWorkSpace = await _repository.GetByConditionAsync(ws => ws.Id == id);

			if (existingWorkSpace == null)
			{
				return (BaseResponse<string>.Fail("WorkSpace not found", Domain.Enums.ErrorType.NotFound));
			}

			var workSpace = _mapper.Map(requests, existingWorkSpace);
			_repository.Update(workSpace);
			var result = await _repository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("WorkSpace updated successfully")
				: BaseResponse<string>.Fail("Failed to update WorkSpace", Domain.Enums.ErrorType.ServerError);
		}
	}
}
