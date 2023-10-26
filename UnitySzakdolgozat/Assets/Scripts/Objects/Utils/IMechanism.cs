public interface IMechanism
{
   public bool Activated { get; set; }
   public void Action();

   public void AddTask(Task task);
}

