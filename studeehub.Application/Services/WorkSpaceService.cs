using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using studeehub.Application.DTOs.Requests.Workspace;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.Workspace;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;

namespace studeehub.Application.Services
{
	public class WorkspaceService : IWorkspaceService
	{
		private readonly IGenericRepository<Workspace> _repository;
		private readonly IValidator<CreateWorkspaceRequest> _createValidator;
		private readonly IValidator<UpdateWorkspaceRequest> _updateValidator;
		private readonly IWorkSpaceRepository _workSpaceRepository;
		private readonly ISupabaseStorageService _supabaseStorageService;
		private readonly IMapper _mapper;

		public WorkspaceService(
			IGenericRepository<Workspace> repository,
			IMapper mapper,
			IWorkSpaceRepository workSpaceRepository,
			IValidator<CreateWorkspaceRequest> createValidator,
			IValidator<UpdateWorkspaceRequest> updateValidator,
			ISupabaseStorageService supabaseStorageService)
		{
			_repository = repository;
			_mapper = mapper;
			_workSpaceRepository = workSpaceRepository;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
			_supabaseStorageService = supabaseStorageService;
		}

		public async Task<BaseResponse<string>> CreateWorkspaceAsync(CreateWorkspaceRequest requests)
		{
			var validationResult = await _createValidator.ValidateAsync(requests);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, Domain.Enums.ErrorType.Validation);
			}

			var Workspace = _mapper.Map<Workspace>(requests);

			if (string.IsNullOrWhiteSpace(requests.Name))
			{
				Workspace.Name = await _workSpaceRepository.GenerateUniqueWorkspaceNameAsync(requests.OwnerId);
			}

			await _repository.AddAsync(Workspace);
			var result = await _repository.SaveChangesAsync();
			return result
				? BaseResponse<string>.Ok("Workspace created successfully")
				: BaseResponse<string>.Fail("Failed to create Workspace", Domain.Enums.ErrorType.ServerError);
		}

		public async Task<BaseResponse<string>> DeleteWorkspaceAsync(Guid id)
		{
			var existingWorkSpace = await _repository.GetByConditionAsync(
				ws => ws.Id == id,
				include: i => i.Include(ws => ws.Documents));

			if (existingWorkSpace == null)
			{
				return BaseResponse<string>.Fail("Workspace not found", Domain.Enums.ErrorType.NotFound);
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
				var message = "Failed to delete Workspace";
				if (storageDeletionErrors.Any())
					message += $"; storage warnings: {string.Join(" | ", storageDeletionErrors)}";
				return BaseResponse<string>.Fail(message, Domain.Enums.ErrorType.ServerError);
			}

			if (storageDeletionErrors.Any())
			{
				var warning = $"Workspace deleted, but some files could not be removed from storage: {string.Join(" | ", storageDeletionErrors)}";
				return BaseResponse<string>.Ok(warning);
			}

			return BaseResponse<string>.Ok("Workspace deleted successfully");
		}

		public async Task<BaseResponse<GetWorkspaceResponse>> GetWorkspaceByIdAsync(Guid id)
		{
			var Workspace = await _repository.GetByConditionAsync(ws => ws.Id == id,
															include: ws => ws.
																Include(ws => ws.Documents).
																Include(ws => ws.Notes).
																Include(ws => ws.Flashcards),
															asNoTracking: true);
			if (Workspace == null)
			{
				return BaseResponse<GetWorkspaceResponse>.Fail("Workspace not found", Domain.Enums.ErrorType.NotFound);
			}

			// Try Mapster mapping first; fall back to manual projection for related collections
			var response = _mapper.Map<GetWorkspaceResponse>(Workspace) ?? new GetWorkspaceResponse
			{
				Id = Workspace.Id,
				Name = Workspace.Name,
				Description = Workspace.Description,
				CreatedAt = Workspace.CreatedAt,
				UpdatedAt = Workspace.UpdatedAt
			};

			// Ensure related collections are populated even if Mapster isn't configured for them
			response.Documents = Workspace.Documents?
				.Select(d => new DocumentResponse
				{
					Id = d.Id,
					UserId = d.UserId,
					WorkSpaceId = d.WorkSpaceId,
					Name = d.Name,
					Description = d.Description,
					Type = d.Type,
					FilePath = d.FilePath
				})
				.ToList();

			response.Notes = Workspace.Notes?
				.Select(n => new NoteResponse
				{
					Id = n.Id,
					UserId = n.UserId,
					WorkSpaceId = n.WorkSpaceId,
					Title = n.Title,
					Content = n.Content
				})
				.ToList();

			response.Flashcards = Workspace.Flashcards?
				.Select(f => new FlashcardResponse
				{
					Id = f.Id,
					UserId = f.UserId,
					WorkSpaceId = f.WorkSpaceId,
					Question = f.Question,
					Answer = f.Answer
				})
				.ToList();

			return BaseResponse<GetWorkspaceResponse>.Ok(response);
		}

		public async Task<BaseResponse<List<GetWorkspaceResponse>>> GetWorkspacesByUserIdAsync(Guid userId)
		{
			var Workspaces = await _repository.GetAllAsync(ws => ws.UserId == userId, asNoTracking: true);
			if (Workspaces == null || !Workspaces.Any())
			{
				return BaseResponse<List<GetWorkspaceResponse>>.Fail("No Workspaces found for the user", Domain.Enums.ErrorType.NotFound);
			}
			var response = _mapper.Map<List<GetWorkspaceResponse>>(Workspaces);
			return BaseResponse<List<GetWorkspaceResponse>>.Ok(response);
		}

		public async Task<BaseResponse<string>> UpdateWorkspaceAsync(Guid id, UpdateWorkspaceRequest requests)
		{
			var validationResult = _updateValidator.Validate(requests);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BaseResponse<string>.Fail(errors, Domain.Enums.ErrorType.Validation);
			}

			var existingWorkspace = await _repository.GetByConditionAsync(ws => ws.Id == id);

			if (existingWorkspace == null)
			{
				return (BaseResponse<string>.Fail("Workspace not found", Domain.Enums.ErrorType.NotFound));
			}

			var Workspace = _mapper.Map(requests, existingWorkspace);
			_repository.Update(Workspace);
			var result = await _repository.SaveChangesAsync();

			return result
				? BaseResponse<string>.Ok("Workspace updated successfully")
				: BaseResponse<string>.Fail("Failed to update Workspace", Domain.Enums.ErrorType.ServerError);
		}
	}
}
