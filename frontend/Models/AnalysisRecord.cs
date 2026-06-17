using System.ComponentModel.DataAnnotations;

namespace DataAnalizer.Models;

public class AnalysisRecord
{
    [Key]
    public int Id { get; set; }

 
    public int BackendDatasetId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; }

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

  
    public string AnalysisJson { get; set; } = string.Empty;
}