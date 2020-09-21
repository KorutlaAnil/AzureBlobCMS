using System;
using System.Collections.Generic;

namespace AzureBlobCMS.Models
{
    public partial class PersonLogin
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual Person Person { get; set; }
    }
}
