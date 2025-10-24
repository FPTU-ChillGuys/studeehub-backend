namespace studeehub.Application.DTOs.Responses.SubPlan
{
	public class GetSubPlanResponse
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty; // Display name
		public string? Description { get; set; }

		public decimal Price { get; set; } // VND or USD
		public string Currency { get; set; } = "VND";
		public int DurationInDays { get; set; } // e.g., 30 for monthly, 365 for yearly
		public float DiscountPercentage { get; set; } = 0.0f;

		// Features
		public int DocumentUploadLimitPerDay { get; set; } = 0;
		public int MaxStorageMB { get; set; } = 0;
		public int AIQueriesPerDay { get; set; } = 0;
		public int FlashcardCreationLimitPerDay { get; set; } = 0;
		public bool HasAIAnalysis { get; set; } = true;
	}
}
