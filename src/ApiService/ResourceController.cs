using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace VictorFrye.SimpleCrud.ApiService;

[ApiController]
[Route("api/resources")]
public class ResourceController(ResourceDbContext context) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CreateResource(Resource resource, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        context.Resources.Add(resource);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(ReadResourceById), new { id = resource.Id }, resource);
    }

    [HttpGet]
    [OutputCache(Duration = 5)]
    public async Task<IActionResult> ReadResources()
    {
        var resources = await context.Resources.ToListAsync();

        return Ok(resources);
    }

    [HttpGet("{id:guid}")]
    [OutputCache(Duration = 5)]
    public async Task<IActionResult> ReadResourceById(Guid id, CancellationToken cancellationToken)
    {
        var resource = await context.Resources.FindAsync([id], cancellationToken);

        return resource is null ? NotFound(id) : Ok(resource);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateResource(Guid id, Resource resource, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingResource = await context.Resources.FindAsync([id], cancellationToken);

        if (existingResource is null)
        {
            existingResource = new Resource
            {
                Id = id,
                Name = resource.Name,
                Count = resource.Count
            };

            context.Resources.Add(existingResource);
        }
        else
        {
            existingResource.Name = resource.Name;
            existingResource.Count = resource.Count;
            context.Resources.Update(existingResource);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Ok(existingResource);
    }

    [HttpPatch("{id:guid}/count/{count:int}")]
    public async Task<IActionResult> UpdateResourceCount(Guid id, int count, CancellationToken cancellationToken)
    {
        var resource = await context.Resources.FindAsync([id], cancellationToken);

        if (resource is null)
        {
            return NotFound(id);
        }

        resource.Count = count;
        context.Resources.Update(resource);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(resource);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteResource(Guid id, CancellationToken cancellationToken)
    {
        var resource = await context.Resources.FindAsync([id], cancellationToken);

        if (resource is null)
        {
            return NotFound(id);
        }

        context.Resources.Remove(resource);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
