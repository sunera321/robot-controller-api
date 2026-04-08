using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private static List<Map> maps = new List<Map>
        {
            new Map
            {
                Id = 1,
                Name = "Default Map",
                Rows = 5,
                Columns = 5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            }
        };

        [HttpGet]
        public ActionResult<List<Map>> GetAll()
        {
            return Ok(maps);
        }

        [HttpGet("{id}")]
        public ActionResult<Map> GetById(int id)
        {
            var map = maps.FirstOrDefault(m => m.Id == id);

            if (map == null)
                return NotFound();

            return Ok(map);
        }

        [HttpPost]
        public ActionResult<Map> Create(Map newMap)
        {
            if (newMap.Rows < 2 || newMap.Rows > 100 || newMap.Columns < 2 || newMap.Columns > 100)
                return BadRequest("Rows and columns must be between 2 and 100.");

            newMap.CreatedDate = DateTime.Now;
            newMap.ModifiedDate = DateTime.Now;

            maps.Add(newMap);

            return CreatedAtAction(nameof(GetById), new { id = newMap.Id }, newMap);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Map updatedMap)
        {
            var existing = maps.FirstOrDefault(m => m.Id == id);

            if (existing == null)
                return NotFound();

            if (updatedMap.Rows < 2 || updatedMap.Rows > 100 || updatedMap.Columns < 2 || updatedMap.Columns > 100)
                return BadRequest("Rows and columns must be between 2 and 100.");

            existing.Name = updatedMap.Name;
            existing.Rows = updatedMap.Rows;
            existing.Columns = updatedMap.Columns;
            existing.ModifiedDate = DateTime.Now;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var map = maps.FirstOrDefault(m => m.Id == id);

            if (map == null)
                return NotFound();

            maps.Remove(map);
            return NoContent();
        }

        [HttpGet("{id}/check-coordinate/{row}/{column}")]
        public ActionResult<bool> CheckCoordinate(int id, int row, int column)
        {
            var map = maps.FirstOrDefault(m => m.Id == id);

            if (map == null)
                return NotFound();

            bool isInside = row >= 0 && column >= 0 && row < map.Rows && column < map.Columns;

            return Ok(isInside);
        }
    }
}