using OrganizationStructureWebAPI.Models;
using OrganizationStructureWebAPI.Models.Misc;
using OrganizationStructureWebAPI.Models.OrganizationStructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace OrganizationStructureWebAPI.Controllers
{
	/// <summary>
	/// Base controller for all organization structure nodes' controllers.
	/// 
	/// </summary>
	/// <typeparam name="TParent">The type of the parent node in the organization structure. 
	/// Can be null (OrganizationStructureNodeNull), in which case <typeparamref name="TCurrent"/>
	/// is the topmost element in the hierarchy.</typeparam>
	/// 
	/// <typeparam name="TCurrent">The type of th current element in the organization structure 
	/// hierarchy. Must be instantiable, cannot be null (OrganizationStructureNodeNull).</typeparam>
	/// 
	/// <typeparam name="TChild">The type of the children node in the organization structure. 
	/// Can be null (OrganizationStructureNodeNull), in which case <typeparamref name="TCurrent"/>
	/// is the lowest element in the hierarchy.</typeparam>
	public abstract class OrganizationStructureBaseController<TParent, TCurrent, TChild> : ControllerBase
		where TParent : OrganizationStructureNode
		where TCurrent : OrganizationStructureNode, new()
		where TChild : OrganizationStructureNode
	{
		protected readonly DbSet<TParent>? _parents;
		protected readonly DbSet<TCurrent> _currents;
		protected readonly DbSet<TChild>? _children;
		protected readonly OrganizationStructureDbContext _context;
		protected readonly ILogger<OrganizationStructureBaseController<TParent, TCurrent, TChild>> _logger;

		protected OrganizationStructureBaseController(OrganizationStructureDbContext context, 
			ILogger<OrganizationStructureBaseController<TParent, TCurrent, TChild>> logger)
		{
			_currents = context.GetDbSet<TCurrent>() 
				?? throw new NullReferenceException(nameof(TCurrent));
			_children = context.GetDbSet<TChild>();
			_parents = context.GetDbSet<TParent>();

			_context = context;
			_logger = logger;
		}	

		private static string CurrentWithIdDoesNotExist(int id) => $"{typeof(TCurrent).Name} with ID {id} does not exist.";

		protected async Task<ActionResult<IEnumerable<TCurrent>>> Get() => await GetQuery().ToListAsync();

		protected async Task<ActionResult<TCurrent>> Get(int id)
		{
			var fetched = await GetSingle(id);
			return fetched switch
			{
				null => NotFound(CurrentWithIdDoesNotExist(id)),
				_ => fetched
			};
		}

		protected async Task<ActionResult<TCurrent>> Put(OrganizationStructureNodeDTO newData)
		{
			var fetched = await GetSingle(newData.Id);
			return fetched switch
			{
				null => NotFound(CurrentWithIdDoesNotExist(newData.Id)),
				_ => await Update(fetched, newData)
			};
		}

		protected async Task<ActionResult<TCurrent>> Put(int currentId, int leadId)
		{
			var fetchedParent = await GetSingle(currentId);
			var fetchedLead = await _context.Employees.FirstOrDefaultAsync(lead => lead.Id == leadId);
			return (fetchedParent, fetchedLead) switch
			{
				(null, _) => NotFound(CurrentWithIdDoesNotExist(currentId)),
				(_, null) => NotFound($"Employee with ID {leadId} does not exist."),
				(_, _) => await Update(fetchedParent, fetchedLead)
			};
		}

		protected async Task<ActionResult<TCurrent>> Put(int currentId, ImmutableList<int> childrenIds)
		{
			if (typeof(TChild) == typeof(OrganizationStructureNodeNull))
			{
				throw new NotSupportedException($"Operation not supported for {typeof(TCurrent).Name}.");
			}

			var fetched = await GetSingle(currentId);
			if (fetched == null)
			{
				return NotFound(CurrentWithIdDoesNotExist(currentId));
			}

			try
			{
				return await Update(fetched, childrenIds);
			}
			catch (KeyNotFoundException e)
			{
				return NotFound(e.Message);
			}
		}

		protected async Task<ActionResult<TCurrent>> Post(int leadId, OrganizationStructureNodeDTO newData, int? parentNodeId = null)
		{
			if (await GetSingle(newData.Id) != null)
			{
				return Conflict($"{typeof(TCurrent).Name} with ID {newData.Id} already exists.");
			}

			var fetchedLead = await _context.Employees.FirstOrDefaultAsync(e => e.Id == leadId);
			if (fetchedLead == null)
			{
				return NotFound($"Employee with ID {leadId} does not exist. {typeof(TCurrent).Name} will not be created.");
			}

			if (!parentNodeId.HasValue)
			{
				return await TryAdd(Create(fetchedLead, newData));
			}

			var fetchedParent = await GetParent(parentNodeId.Value);
			return fetchedParent switch
			{
				null => NotFound($"{typeof(TParent).Name} with ID {parentNodeId} does not exist. Project will not be created."),
				_ => await TryAdd(SetParentNode(Create(fetchedLead, newData), fetchedParent))
			};
		}

		protected async Task<ActionResult<TCurrent>> Delete(int id)
		{
			var fetched = await GetSingle(id);
			if (fetched == null)
			{
				return NotFound(CurrentWithIdDoesNotExist(id));
			}

			if (fetched.GetChildren() is IEnumerable<TChild> children && children.Any())
			{
				return Conflict($"Cannot delete {typeof(TCurrent).Name} that has some {typeof(TChild).Name} assigned. " +
					$"Delete children or change the owning {typeof(TCurrent).Name} first and then delete the {typeof(TCurrent).Name} please.");
			}

			_currents.Remove(fetched);
			await _context.SaveChangesAsync();
			return fetched;
		}

		private async Task<TCurrent?> GetSingle(int id) => await GetQuery().FirstOrDefaultAsync(x => x.Id == id);

		private static TChild? GetChild(TCurrent fetched, int childId) 
			=> fetched.GetChildren()?.FirstOrDefault(c => c.Id == childId) as TChild;

		private async Task<TCurrent> Update(TCurrent fetched, OrganizationStructureNodeDTO newData)
		{
			fetched.Code = newData.Code;
			fetched.Name = newData.Name;

			await _context.SaveChangesAsync();
			return fetched;
		}

		private async Task<TCurrent> Update(TCurrent fetchedNode, Employee fetchedEmployee)
		{
			SetLead(fetchedNode, fetchedEmployee);
			await _context.SaveChangesAsync();
			return fetchedNode;
		}

		private async Task<TCurrent> Update(TCurrent fetched, ImmutableList<int> newChildrenIds) =>
			await Update(fetched, newChildrenIds
				.Select(id => _children!.FirstOrDefault(c => c.Id == id)
				?? throw new KeyNotFoundException($"{typeof(TChild).Name} with ID {id} does not exist.")));

		// Credits for this method go here
		// https://github.com/thbst16/dotnet-blazor-crud/blob/master/Blazorcrud.Server/Models/PersonRepository.cs
		private async Task<TCurrent> Update(TCurrent fetched, IEnumerable<TChild> newChildren)
		{
			foreach (var existingChild in fetched.GetChildren()!)
			{
				// This is probably not too efficient but I wanted to go to sleep already. Will be fixed
				// in the next release :)
				if (_children!.FirstOrDefault(c => c.Id == existingChild.Id)?.GetChildren() is 
					IEnumerable<OrganizationStructureNode> grandchildren && grandchildren.Any())
				{
					_logger.LogInformation($"{typeof(TChild).Name} with ID {existingChild.Id} was not removed from " +
						$"{typeof(TCurrent).Name} with ID {fetched.Id}.");
					continue;
				}

				// remove children not contained in the new list
				if (!newChildren.Any(c => c.Id == existingChild.Id))
				{
					_logger.LogWarning($"{typeof(TChild).Name} with ID {existingChild.Id} will be removed from database.");
					_children!.Remove((TChild)existingChild);
				}
			}

			// add non existing children
			foreach (var updatedChild in newChildren)
			{
				if (GetChild(fetched, updatedChild.Id) == null)
				{
					AddChild(fetched, updatedChild);
				}
			}

			await _context.SaveChangesAsync();
			return fetched;
		}

		private TCurrent Create(Employee lead, OrganizationStructureNodeDTO data)
		{
			var node = new TCurrent
			{
				Id = data.Id,
				Name = data.Name,
				Code = data.Code
			};
			SetLead(node, lead);
			return node;
		}

		private async Task<ActionResult<TCurrent>> TryAdd(TCurrent maybeNewEntry)
		{
			ModelState.Clear();
			if (TryValidateModel(maybeNewEntry))
			{
				_currents.Add(maybeNewEntry);
				await _context.SaveChangesAsync();
				return CreatedAtAction($"Get{typeof(TCurrent).Name}", 
					new { id = maybeNewEntry, Entry = maybeNewEntry }, maybeNewEntry);
			}

			return this.CollectErrors();
		}

		protected abstract IQueryable<TCurrent> GetQuery();

		protected abstract IEnumerable<TChild> GetChildrenAsEnumerable();

		protected abstract Task<TParent?> GetParent(int id);

		protected abstract void SetLead(TCurrent node, Employee employee);

		protected abstract void AddChild(TCurrent parent, TChild child);

		protected abstract TCurrent SetParentNode(TCurrent node, TParent parent);
	}
}
