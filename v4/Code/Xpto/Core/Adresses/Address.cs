using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpto.Core.Adresses
{
    public class Address
    {
        public Guid Id { get; set; }
        public int CustomerCode { get; set; }
        public string Type { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Note { get; set; }

        public Address()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return string.Join(" ", Street, Number, Complement, District, City, State, ZipCode);
        }


    }
    public class AddressParams
    {
        [JsonProperty("logradouro")]
        public string Street { get; set; } = null!;
        public string Number { get; set; } = null!;

        [JsonProperty("complemento")]
        public string? Complement { get; set; }

        [JsonProperty("bairro")]
        public string District { get; set; } = null!;

        [JsonProperty("localidade")]
        public string City { get; set; } = null!;

        [JsonProperty("uf")]
        public string State { get; set; } = null!;

        [JsonProperty("cep")]
        public string ZipCode { get; set; } = null!;
    }

}
