using MapsterMapper;
using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PomodoroSession;
using studeehub.Application.Extensions;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Pomodoros;
using System.Linq.Expressions;

namespace studeehub.Application.Services
{
	public class PomodoroSessionService : IPomodoroSessionService
	{
		private readonly IPomodoroSessionRepository _pomodoroSessionRepository;
		private readonly IPomodoroSettingService _pomodoroSettingService;
		private readonly IMapper _mapper;

		public PomodoroSessionService(
			IPomodoroSessionRepository pomodoroSessionRepository,
			IMapper mapper,
			IPomodoroSettingService pomodoroSettingService)
		{
			_pomodoroSessionRepository = pomodoroSessionRepository;
			_mapper = mapper;
			_pomodoroSettingService = pomodoroSettingService;
		}

		#region Public Methods

		public async Task<PagedResponse<GetSessionResponse>> GetSessionsAsync(Guid userId, GetSessionsRequest request)
		{
			Expression<Func<PomodoroSession, bool>> filter = s => s.UserId == userId;

			if (request.Start.HasValue)
				filter = filter.AndAlso(s => s.Start >= request.Start.Value);

			if (request.End.HasValue)
				filter = filter.AndAlso(s => s.End.HasValue && s.End <= request.End.Value);

			if (request.Type.HasValue)
				filter = filter.AndAlso(s => s.Type == request.Type.Value);

			if (request.Status.HasValue)
				filter = filter.AndAlso(s => s.Status == request.Status.Value);

			var (pagedSessions, totalCount) = await _pomodoroSessionRepository.GetPagedAsync(
				filter: filter,
				include: null,
				orderBy: ps => ps.OrderByDescending(s => s.Start),
				pageNumber: request.PageNumber,
				pageSize: request.PageSize,
				asNoTracking: true);

			if (pagedSessions == null || !pagedSessions.Any())
			{
				return PagedResponse<GetSessionResponse>.Ok(new List<GetSessionResponse>(), 0, request.PageNumber, request.PageSize);
			}

			var response = _mapper.Map<List<GetSessionResponse>>(pagedSessions);
			return PagedResponse<GetSessionResponse>.Ok(response, totalCount, request.PageNumber, request.PageSize);
		}

		public async Task<PagedResponse<GetSessionHistoryResponse>> GetSessionsAndStatsAsync(Guid userId, GetSessionsRequest request)
		{
			Expression<Func<PomodoroSession, bool>> filter = s => s.UserId == userId;

			if (request.Start.HasValue)
				filter = filter.AndAlso(s => s.Start >= request.Start.Value);

			if (request.End.HasValue)
				filter = filter.AndAlso(s => s.End.HasValue && s.End <= request.End.Value);

			if (request.Type.HasValue)
				filter = filter.AndAlso(s => s.Type == request.Type.Value);

			if (request.Status.HasValue)
				filter = filter.AndAlso(s => s.Status == request.Status.Value);

			// Fetch paginated sessions
			var (pagedSessions, totalCount) = await _pomodoroSessionRepository.GetPagedAsync(
				filter: filter,
				include: null,
				orderBy: ps => ps.OrderByDescending(s => s.Start),
				pageNumber: request.PageNumber,
				pageSize: request.PageSize,
				asNoTracking: true);

			if (pagedSessions == null || !pagedSessions.Any())
			{
				var emptySummary = new GetSessionHistoryResponse
				{
					Sessions = new List<GetSessionResponse>(),
					TotalCount = 0,
					TotalFocusMinutes = 0,
					TotalBreakMinutes = 0,
					AverageFocusDuration = 0,
					CompletionRate = 0
				};
				// Wrap the summary in a list
				return PagedResponse<GetSessionHistoryResponse>.Ok(new List<GetSessionHistoryResponse> { emptySummary }, 0, request.PageNumber, request.PageSize);
			}

			// Map sessions
			var sessionResponses = _mapper.Map<List<GetSessionResponse>>(pagedSessions);

			// Calculate summary metrics (based only on this page)
			var workSessions = pagedSessions.Where(s => s.Type == PomodoroType.Work && s.Status == PomodoroStatus.Completed);
			var breakSessions = pagedSessions.Where(s => s.Type != PomodoroType.Work && s.Status == PomodoroStatus.Completed);

			double totalFocusMinutes = workSessions.Sum(s => s.Duration.TotalMinutes);
			double totalBreakMinutes = breakSessions.Sum(s => s.Duration.TotalMinutes);
			double averageFocusDuration = workSessions.Any() ? workSessions.Average(s => s.Duration.TotalMinutes) : 0;
			double completionRate = pagedSessions.Any()
				? Math.Round(pagedSessions.Count(s => s.Status == PomodoroStatus.Completed) * 100.0 / pagedSessions.Count(), 2)
				: 0;

			var summary = new GetSessionHistoryResponse
			{
				Sessions = sessionResponses,
				TotalCount = totalCount,
				TotalFocusMinutes = Math.Round(totalFocusMinutes, 2),
				TotalBreakMinutes = Math.Round(totalBreakMinutes, 2),
				AverageFocusDuration = Math.Round(averageFocusDuration, 2),
				CompletionRate = completionRate
			};

			// Wrap the summary in a list
			return PagedResponse<GetSessionHistoryResponse>.Ok(new List<GetSessionHistoryResponse> { summary }, totalCount, request.PageNumber, request.PageSize);
		}

		public async Task<BaseResponse<GetSessionResponse>> StartSessionAsync(Guid userId, PomodoroType type)
		{
			// STEP 1: Check for existing active session
			var active = await _pomodoroSessionRepository
				.GetByConditionAsync(ps => ps.UserId == userId && ps.Status == PomodoroStatus.Active);

			// STEP 2: If active exists
			if (active != null)
			{
				// Auto-complete if expired
				if (active.Start.HasValue && active.Start.Value.AddMinutes(active.Duration.TotalMinutes) <= DateTime.UtcNow)
				{
					active.Status = PomodoroStatus.Completed;
					active.End = active.Start.Value.AddMinutes(active.Duration.TotalMinutes);
					_pomodoroSessionRepository.Update(active);
					await _pomodoroSessionRepository.SaveChangesAsync();
					active = null; // proceed to create new
				}
				else if (active.Type != type)
				{
					// If type mismatch → mark incomplete, start new one
					active.Status = PomodoroStatus.Incomplete;
					active.End = DateTime.UtcNow;
					_pomodoroSessionRepository.Update(active);
					await _pomodoroSessionRepository.SaveChangesAsync();
					active = null; // proceed to create new
				}
				else
				{
					// If same type → resume
					var response = _mapper.Map<GetSessionResponse>(active);
					return BaseResponse<GetSessionResponse>.Ok(response, "Resumed existing active Pomodoro session.");
				}
			}

			// STEP 3: Create new session
			var settings = await _pomodoroSettingService.GetForUserAsync(userId);
			if (settings == null)
				return BaseResponse<GetSessionResponse>.Fail("Pomodoro settings not found. Please set them up first.", ErrorType.NotFound);

			var duration = GetDurationFromType(type, settings);
			var newSession = CreateSession(userId, type, duration, cycleIndex: 1, isAutoGenerated: false);

			await _pomodoroSessionRepository.AddAsync(newSession);
			await _pomodoroSessionRepository.SaveChangesAsync();

			var newResponse = _mapper.Map<GetSessionResponse>(newSession);
			return BaseResponse<GetSessionResponse>.Ok(newResponse, "New Pomodoro session started.");
		}

		public async Task<BaseResponse<GetSessionResponse>> CompleteSessionAsync(Guid userId)
		{
			var activeSession = await _pomodoroSessionRepository
				.GetByConditionAsync(ps => ps.UserId == userId && ps.Status == PomodoroStatus.Active);

			if (activeSession == null)
				return BaseResponse<GetSessionResponse>.Fail("No active Pomodoro session found.", ErrorType.NotFound);

			activeSession.Status = PomodoroStatus.Completed;
			activeSession.End = DateTime.UtcNow;

			_pomodoroSessionRepository.Update(activeSession);
			await _pomodoroSessionRepository.SaveChangesAsync();

			var settings = await _pomodoroSettingService.GetForUserAsync(userId);
			if (settings == null)
				return BaseResponse<GetSessionResponse>.Ok(_mapper.Map<GetSessionResponse>(activeSession), "Session completed.");

			// Only stop if user disabled auto-start
			if (!settings.AutoStartNext)
				return BaseResponse<GetSessionResponse>.Ok(_mapper.Map<GetSessionResponse>(activeSession), "Session completed.");

			// Otherwise start next automatically
			var (nextType, nextCycleIndex, duration) = GetNextAutoCycle(activeSession, settings);
			var nextSession = CreateSession(userId, nextType, duration, nextCycleIndex, isAutoGenerated: settings.AutoStartNext);

			await _pomodoroSessionRepository.AddAsync(nextSession);
			await _pomodoroSessionRepository.SaveChangesAsync();

			var response = _mapper.Map<GetSessionResponse>(nextSession);
			return BaseResponse<GetSessionResponse>.Ok(response, "Session completed. Next one started automatically.");
		}

		public async Task<BaseResponse<GetSessionResponse>> SkipSessionAsync(Guid userId)
		{
			var activeSession = await _pomodoroSessionRepository
				.GetByConditionAsync(ps => ps.UserId == userId && ps.Status == PomodoroStatus.Active);

			if (activeSession == null)
				return BaseResponse<GetSessionResponse>.Fail("No active session to skip.", ErrorType.Validation);

			activeSession.Status = PomodoroStatus.Skipped;
			activeSession.End = DateTime.UtcNow;
			_pomodoroSessionRepository.Update(activeSession);
			await _pomodoroSessionRepository.SaveChangesAsync();

			var settings = await _pomodoroSettingService.GetForUserAsync(userId);
            // Only stop if user disabled auto-start
            if (!settings.AutoStartNext)
                return BaseResponse<GetSessionResponse>.Ok(_mapper.Map<GetSessionResponse>(activeSession), "Session completed.");

            // Auto → move to next cycle
            var (nextType, nextCycleIndex, duration) = GetNextAutoCycle(activeSession, settings);
			var nextSession = CreateSession(userId, nextType, duration, nextCycleIndex, isAutoGenerated: settings.AutoStartNext);

			await _pomodoroSessionRepository.AddAsync(nextSession);
			await _pomodoroSessionRepository.SaveChangesAsync();

			var response = _mapper.Map<GetSessionResponse>(nextSession);
			return BaseResponse<GetSessionResponse>.Ok(response, "Session skipped. Next one started automatically.");
		}

		#endregion

		#region Private Helpers

		private PomodoroSession CreateSession(Guid userId, PomodoroType type, TimeSpan duration, int cycleIndex, bool isAutoGenerated)
		{
			return new PomodoroSession
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				Type = type,
				Duration = duration,
				Start = DateTime.UtcNow,
				End = DateTime.UtcNow.Add(duration),
				Status = PomodoroStatus.Active,
				CycleIndex = cycleIndex,
				IsAutoGenerated = isAutoGenerated
			};
		}

		private (PomodoroType Type, int CycleIndex, TimeSpan Duration) GetNextAutoCycle(PomodoroSession lastSession, PomodoroSetting settings)
		{
			PomodoroType nextType;
			int nextCycleIndex;

			if (lastSession.Type == PomodoroType.Work)
			{
				nextCycleIndex = lastSession.CycleIndex;
				nextType = (nextCycleIndex % settings.LongBreakInterval == 0)
					? PomodoroType.LongBreak
					: PomodoroType.ShortBreak;
			}
			else
			{
				// After any break → start Work again
				nextCycleIndex = (lastSession.Type == PomodoroType.LongBreak)
					? 1
					: lastSession.CycleIndex + 1;
				nextType = PomodoroType.Work;
			}

			var duration = GetDurationFromType(nextType, settings);
			return (nextType, nextCycleIndex, duration);
		}

		private TimeSpan GetDurationFromType(PomodoroType type, PomodoroSetting settings)
		{
			return type switch
			{
				PomodoroType.Work => TimeSpan.FromMinutes(settings.WorkDuration),
				PomodoroType.ShortBreak => TimeSpan.FromMinutes(settings.ShortBreakDuration),
				PomodoroType.LongBreak => TimeSpan.FromMinutes(settings.LongBreakDuration),
				_ => TimeSpan.FromMinutes(settings.WorkDuration)
			};
		}

		#endregion
	}
}
