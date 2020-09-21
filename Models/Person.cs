using System;
using System.Collections.Generic;

namespace AzureBlobCMS.Models
{
    public partial class Person
    {
        public Person()
        {
            PersonLogin = new HashSet<PersonLogin>();
        }

        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long? Mobile { get; set; }
        public string Gender { get; set; }

        public virtual ICollection<PersonLogin> PersonLogin { get; set; }
    }
}
