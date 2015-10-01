namespace Jagi.Database
{
    /// <summary>
    /// 對應 SQL Server 的 Row version, unique Id
    /// </summary>
    public interface IVersionable
    {
        byte[] RowVersion { get; set; }
    }
}