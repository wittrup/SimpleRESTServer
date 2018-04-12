using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using SimpleRESTServer.Models;

namespace SimpleRESTServer.Controllers
{
    public class PersonController : ApiController
    {
        // GET: api/Person
        public ArrayList Get()
        {
            // TODO Implement Paging Mechanism
            PersonPersistence pp = new PersonPersistence();
            return pp.GetPersons();
        }

        // GET: api/Person/5
        public Person Get(int id)
        {
            PersonPersistence pp = new PersonPersistence();
            Person person = pp.GetPerson(id);

            return person;
        }

        // POST: api/Person
        public HttpResponseMessage Post([FromBody]Person value)
        {
            PersonPersistence pp = new PersonPersistence();
            long id;
            id = pp.savePerson(value);

            value.ID = 0;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("person/{0}", value.ID));
            return response;
        }

        // PUT: api/Person/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Person/5
        public void Delete(int id)
        {
        }
    }
}
