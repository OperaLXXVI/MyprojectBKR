public class AdminContractViewModel
{
    public int Id { get; set; }
    public string TemplateName { get; set; } = "";
    public string ContractNumber { get; set; } = "";
    public string ClientAddress { get; set; } = "";
    public string ClientName { get; set; } = "";
    public string ClientPhone { get; set; } = "";
    public string CreatedByUserName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
