namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
        }

        public Connection(string userName, string connectionId)
        {
            ConnectionId = connectionId;
            UserName = userName;
            
        }

        public string UserName { get; set; }

        public string ConnectionId { get; set; }
    }
}