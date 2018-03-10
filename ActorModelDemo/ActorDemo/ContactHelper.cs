using ActorDemo.Interfaces;

namespace ActorDemo
{
    public static class ContactHelper
    {
        public static Contact CreateRandomContact()
        {
            return new Contact()
            {
                LastName = Faker.Name.First(),
                FirstName = Faker.Name.Last(),
                Email = Faker.Internet.Email()
            };
        }
    }
}
