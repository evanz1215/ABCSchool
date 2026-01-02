using Applocation.Features.Schools;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Schools;

public class SchoolService : ISchoolService
{
    private readonly ApplicationDbContext _context;

    public SchoolService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(School school)
    {
        await _context.Schools.AddAsync(school);
        await _context.SaveChangesAsync();

        return school.Id;
    }

    public async Task<int> DeleteAsync(School school)
    {
        _context.Schools.Remove(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<List<School>> GetAllAsync()
    {
        var result = await _context.Schools.ToListAsync();
        return result;
    }

    public async Task<School> GetByIdAsync(int schoolId)
    {
        var result = await _context.Schools.FirstOrDefaultAsync(x => x.Id == schoolId);

        return result;
    }

    public async Task<School> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var result = await _context.Schools.FirstOrDefaultAsync(x => x.Name == name);

        return result;

        // 不區分大小寫的搜尋範例 (效能較差，不建議使用)
        //    School中新增 public string NameNormalized { get; set; }
        //// DbContext 設定
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<School>()
        //        .HasIndex(s => s.NameNormalized); // 關鍵！一定要對這個欄位建索引
        //}

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    // 找出所有正在被 新增(Added) 或 修改(Modified) 的 School 實體
        //    var entries = ChangeTracker.Entries<School>()
        //        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        //    foreach (var entry in entries)
        //    {
        //        // 只要 Name 有值，就自動轉小寫填入 Normalized 欄位
        //        // 使用 ToLowerInvariant() 避免不同語系 Server 產生怪異結果 (如土耳其文 I)
        //        entry.Entity.NameNormalized = entry.Entity.Name?.ToLowerInvariant();
        //    }

        //    return base.SaveChangesAsync(cancellationToken);
        //}

        //public async Task<School> FindSchoolAsync(string userInput)
        //{
        //    if (string.IsNullOrEmpty(userInput)) return null;

        //    var term = userInput.ToLowerInvariant();

        //    // 這裡會產生 WHERE NameNormalized = '...'
        //    // 這是一個標準的 SARGable 查詢，絕對會走索引 (Index Seek)
        //    return await _context.Schools
        //        .FirstOrDefaultAsync(x => x.NameNormalized == term);
        //}

        //var normalizedName = name.ToLowerInvariant();

        //var result = await _context.Schools.FirstOrDefaultAsync(x => x.Name.ToLowerInvariant() == normalizedName);
    }

    public async Task<int> UpdateAsync(School school)
    {
        _context.Schools.Update(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }
}