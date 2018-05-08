using Newtonsoft.Json;

namespace yapp.Domain
{
    public class Person
    {
        [JsonIgnore]
        public int PersonId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Image { get; set; }

        [JsonIgnore]
        public byte[] Hash { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }
    }
}