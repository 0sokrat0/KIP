namespace pr7.Models;

public class WarehouseItem
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public DateTime? DeliveryDate { get; set; }
}

