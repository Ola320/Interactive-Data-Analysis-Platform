using DataAnalizer.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;


namespace DataAnalizer.Data;

public class AnalysisRepository
{
    public async Task SaveOrUpdateAsync(
        int backendDatasetId,
        string fileName,
        DateTime uploadDate,
        string analysisJson)
    {
        await using var database = new AppDbContext();

        AnalysisRecord? existingRecord =
            await database.Analyses
                .SingleOrDefaultAsync(
                    x => x.BackendDatasetId == backendDatasetId);

        if (existingRecord is null)
        {
            var newRecord = new AnalysisRecord
            {
                BackendDatasetId = backendDatasetId,
                FileName = fileName,
                UploadDate = uploadDate,
                SavedAt = DateTime.UtcNow,
                AnalysisJson = analysisJson
            };

            database.Analyses.Add(newRecord);
        }
        else
        {
            existingRecord.FileName = fileName;
            existingRecord.UploadDate = uploadDate;
            existingRecord.SavedAt = DateTime.UtcNow;
            existingRecord.AnalysisJson = analysisJson;
        }

        await database.SaveChangesAsync();
    }

    public async Task<AnalysisRecord?> GetByBackendIdAsync(
        int backendDatasetId)
    {
        await using var database = new AppDbContext();

        return await database.Analyses
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.BackendDatasetId == backendDatasetId);
    }

    public async Task<List<AnalysisRecord>> GetAllAsync()
    {
        await using var database = new AppDbContext();

        return await database.Analyses
            .AsNoTracking()
            .OrderByDescending(x => x.UploadDate)
            .ToListAsync();
    }

    public async Task DeleteAsync(
        int backendDatasetId)
    {
        await using var database = new AppDbContext();

        AnalysisRecord? record =
            await database.Analyses
                .SingleOrDefaultAsync(
                    x => x.BackendDatasetId == backendDatasetId);

        if (record is null)
            return;

        database.Analyses.Remove(record);
        await database.SaveChangesAsync();
    }
}