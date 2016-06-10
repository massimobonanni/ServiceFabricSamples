namespace WebApi.Core.Requests
{
    public class OrderingField
    {
        public OrderingField(string field, OrderDirection direction = OrderDirection.Ascending)
        {
            Field = field;
            Direction = direction;
        }
        public string Field { get; private set; }
        public OrderDirection Direction { get; private set; }

        public string ToOrderByString()
        {
            return $"{Field} {Direction}";
        }

    }

    public enum OrderDirection
    {
        Descending,
        Ascending
    }
}
