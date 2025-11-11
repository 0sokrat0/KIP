namespace pr8.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PickupPointId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}

