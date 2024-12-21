namespace EShopOnContainers.WebMvc.Domain;

public class PaginationInfo
{
    public int TotalItems { get; set; }
    public int ItemsPerPage { get; set; }
    public int ActualPage { get; set; }
    public int TotalPage { get; set; }
    public string Previous {  get; set; }
    public string Next { get; set; }
}
