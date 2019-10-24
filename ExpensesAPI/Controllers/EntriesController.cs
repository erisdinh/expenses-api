using ExpensesAPI.Data;
using ExpensesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ExpensesAPI.Controllers
{
    // Opening our API for third party application -> If not config this, client cant call the api to get data
    // localhost:4200 is the port for angular that the request comes from
    // * means allow everything (header: everything, method: everything)
    [EnableCors("http://localhost:4200", "*", "*")]
    public class EntriesController : ApiController
    {
        // Get all entries from database using HttpGet
        [HttpGet]
        public IHttpActionResult GetEntries() {

            try
            {
                using (var context = new AppDbContext())
                {
                    var entries = context.Entries.ToList();
                    return Ok(entries);
                }
            }
            catch(Exception e) {
                return BadRequest(e.Message);
            }
        }

        // Insert an entry record into database using HttpPost
        // [FromBody] means we get entry from the request body
        [HttpPost]
        public IHttpActionResult PostEntry([FromBody]Entry entry) {
            try {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                using (var context = new AppDbContext()) {
                    context.Entries.Add(entry);
                    context.SaveChanges();
                    return Ok("Entry was created!");
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        // Update an existing entry
        [HttpPut]
        public IHttpActionResult UpdateEntry(int id, [FromBody]Entry entry) {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (id != entry.Id) return BadRequest();

                using (var context = new AppDbContext())
                {
                    var oldEntry = context.Entries.FirstOrDefault(n => n.Id == id);
                    if (oldEntry == null)
                    {
                        return NotFound();
                    }

                    oldEntry.Description = entry.Description;
                    oldEntry.IsExpense = entry.IsExpense;
                    oldEntry.Value = entry.Value;

                    context.SaveChanges();
                    return Ok("Entry is updated!");
                }
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteEntry(int id) {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                using (var context = new AppDbContext()) {
                    var entry = context.Entries.FirstOrDefault(n => n.Id == id);

                    if (entry == null) return NotFound();

                    context.Entries.Remove(entry);
                    context.SaveChanges();

                    return Ok("Entry is deleted!");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetSingleEntry(int id) {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                using (var context = new AppDbContext()) {

                    var entry = context.Entries.FirstOrDefault(n => n.Id == id);

                    if (entry == null) return NotFound();

                    return Ok(entry);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
