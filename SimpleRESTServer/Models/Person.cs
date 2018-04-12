using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleRESTServer.Models
{
    public class Person
    {
        public long ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string E_Mail { get; set; }
        public string Telephone { get; set; }
    }
}
