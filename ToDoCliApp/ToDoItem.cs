using System;

public class ToDoItem
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool CompleteFlag { get; set; }
    public required string Notes { get; set; }
}
