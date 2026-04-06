namespace bidtube.Api.Hubs.Data
{
    public class ConnectedUserData
    {
        public string connectionId { get; set; } = string.Empty;
        public List<string> groups { get; set; } = new List<string>();
    }
}
