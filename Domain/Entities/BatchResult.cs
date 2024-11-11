namespace App.Core.Entities
{
    public class BatchResult : Auditables
    {
        public required Guid ExaminationId { get; set; }
        public int NumberOfUploadedResults { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
        public required Examination Examination { get; set; }
    }
}
