using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note_Backend.Models.DTOs;
using Note_Backend.Repositories;

namespace Note_Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var notes = await _noteRepository.GetAllAsync(userId);
            return Ok(notes);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var note = await _noteRepository.GetByIdAsync(id, userId);
            if (note == null) return NotFound();
            else return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(AddNoteDTO dto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var note = await _noteRepository.AddAsync(dto, userId);
            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateNote(int id, UpdateNoteDTO dto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var find = await _noteRepository.GetByIdAsync(id, userId);
            if (find == null) return NotFound();
            else
            {
                var updatedNote = await _noteRepository.UpdateAsync(id, dto, userId);
                return Ok(updatedNote);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var find = await _noteRepository.GetByIdAsync(id, userId);
            if (find == null) return NotFound();
            else
            {
                await _noteRepository.DeleteAsync(id, userId);
                return Ok();
            }
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdValue, out userId);
        }
    }
}
