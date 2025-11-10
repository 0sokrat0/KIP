namespace pr7.Models;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int CarId { get; set; }
    public int BrokenPartId { get; set; }
    public decimal RepairCost { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum OrderStatus
{
    Pending,
    Accepted,
    Rejected,
    Completed,
    Failed
}

