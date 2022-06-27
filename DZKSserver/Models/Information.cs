namespace DZKSserver.Models
{
    public class Letter
    {
        public int id { get; set; }
        public string lastName { get; set; } = string.Empty; 
        public string name { get; set; } = string.Empty;
        public string computerName { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string date { get; set; } = string.Empty;
        public int levelUrgency { get; set; } = 1;
        public bool status { get; set; } = true;
        public bool inspect { get; set; } = false;
    }
}
