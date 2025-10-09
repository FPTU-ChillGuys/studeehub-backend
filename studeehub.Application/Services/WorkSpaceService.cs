using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.WorkSpace;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class WorkSpaceService : IWorkSpaceService
	{
		private readonly IGenericRepository<WorkSpace> _repository;
		private readonly IValidator<CreateWorkSpaceRequest> _createValidator;
		private readonly IValidator<UpdateWorkSpaceRequest> _updateValidator;
		private readonly IWorkSpaceRepository _workSpaceRepository;
		private readonly ISupabaseStorageService _supabaseStorageService;
		private readonly IMapper _mapper;

		public WorkSpaceService(
			IGenericRepository<WorkSpace> repository,
			IMapper mapper,
			IWorkSpaceRepository workSpaceRepository,
			IValidator<CreateWorkSpaceRequest> createValidator,
			IValidator<UpdateWorkSpaceRequest> updateValidator,
			ISupabaseStorageService supabaseStorageService)
		{
			_repository = repository;
			_mapper = mapper;
			_workSpaceRepository = workSpaceRepository;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
			_supabaseStorageService = supabaseStorageService;
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
				workSpace.Name = await _workSpaceRepository.GenerateUniqueWorkspaceNameAsync(requests.OwnerId);
			}

			await _repository.AddAsync(workSpace);
			var result = await _repository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("WorkSpace created successfully")
				: BaseResponse<string>.Fail("Failed to create WorkSpace", Domain.Enums.ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteWorkSpaceAsync(Guid id)
		{
			var existingWorkSpace = await _repository.GetByConditionAsync(
				ws => ws.Id == id,
				include: i => i.Include(ws => ws.Documents));

			if (existingWorkSpace == null)
			{
				return BaseResponse<string>.Fail("WorkSpace not found", Domain.Enums.ErrorType.NotFound);
			}

			var storageDeletionErrors = new List<string>();

			foreach (var doc in existingWorkSpace.Documents ?? Enumerable.Empty<Document>())
			{
				if (string.IsNullOrWhiteSpace(doc.FilePath))
					continue;

				try
				{
					var filePath = doc.FilePath;
					if (Uri.IsWellFormedUriString(filePath, UriKind.Absolute))
					{
						try
						{
							var extracted = await _supabaseStorageService.ExtractFilePathFromUrl(filePath);
							if (!string.IsNullOrWhiteSpace(extracted))
								filePath = extracted;
						}
						catch
						{
						}
					}

					var deleted = await _supabaseStorageService.DeleteFileAsync(filePath);
					if (!deleted)
					{
						storageDeletionErrors.Add($"Failed to delete storage file for document {doc.Id} (path: {filePath}).");
					}
				}
				catch (Exception ex)
				{
					storageDeletionErrors.Add($"Error deleting storage file for document {doc.Id}: {ex.Message}");
				}
			}

			_repository.Remove(existingWorkSpace);
			var dbResult = await _repository.SaveChangesAsync();

			if (!dbResult)
			{
				var message = "Failed to delete WorkSpace";
				if (storageDeletionErrors.Any())
					message += $"; storage warnings: {string.Join(" | ", storageDeletionErrors)}";
				return BaseResponse<string>.Fail(message, Domain.Enums.ErrorType.ServerError);
			}

			if (storageDeletionErrors.Any())
			{
				var warning = $"WorkSpace deleted, but some files could not be removed from storage: {string.Join(" | ", storageDeletionErrors)}";
				return BaseResponse<string>.Ok(warning);
			}

			return BaseResponse<string>.Ok("WorkSpace deleted successfully");
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
