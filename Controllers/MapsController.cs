using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Map>> GetAll()
        {
            return Ok(MapDataAccess.GetMaps());
        }

        [HttpGet("{id}")]
        public ActionResult<Map> GetById(int id)
        {
            var map = MapDataAccess.GetMapById(id);
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
            MapDataAccess.AddMap(newMap);
            return CreatedAtAction(nameof(GetById), new { id = newMap.Id }, newMap);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Map updatedMap)
        {
            if (updatedMap.Rows < 2 || updatedMap.Rows > 100 || updatedMap.Columns < 2 || updatedMap.Columns > 100)
                return BadRequest("Rows and columns must be between 2 and 100.");

            updatedMap.ModifiedDate = DateTime.Now;
            bool updated = MapDataAccess.UpdateMap(id, updatedMap);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool deleted = MapDataAccess.DeleteMap(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/check-coordinate/{row}/{column}")]
        public ActionResult<bool> CheckCoordinate(int id, int row, int column)
        {
            var map = MapDataAccess.GetMapById(id);
            if (map == null)
                return NotFound();

            bool isInside = row >= 0 && column >= 0 && row < map.Rows && column < map.Columns;
            return Ok(isInside);
        }
    }
}