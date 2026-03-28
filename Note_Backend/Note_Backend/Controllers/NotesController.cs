using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Note_Backend.Models.DTOs;
using Note_Backend.Repositories;

namespace Note_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _NoteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _NoteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var notes = await _NoteRepository.GetAllAsync();
            return Ok(notes);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var note = await _NoteRepository.GetByIdAsync(id);
            if (note == null) return NotFound();
            else return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(AddNoteDTO dto)
        {
            await _NoteRepository.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateNote(int id, UpdateNoteDTO dto)
        {
            var find = await _NoteRepository.GetByIdAsync(id);
            if (find == null) return NotFound();
            else
            {
                await _NoteRepository.UpdateAsync(id, dto);
                return Ok();
            }

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var find = await _NoteRepository.GetByIdAsync(id);
            if (find == null) return NotFound();
            else
            {
                await _NoteRepository.DeleteAsync(id);
                return Ok();
            }

        }
    }
}
