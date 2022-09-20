using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._mapper = mapper;
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var dbCharacters = await _context.Characters
                .Where(c => c.User.Id == GetUserId())
                .ToListAsync();
                response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                response.Data = _mapper.Map<GetCharacterDto>(await _context.Characters
                    .FirstAsync(c => c.Id == id && c.User.Id == GetUserId()));
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            try
            {
                _context.Characters.Add(character);
                await _context.SaveChangesAsync();
                response.Data = await _context.Characters
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c))
                    .ToListAsync();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return await Task.FromResult(response);
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacterDto)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {

                Character character = await _context.Characters
                    .Include(c => c.User)
                    .FirstAsync(c => c.Id == updateCharacterDto.Id);
                if (character.User.Id == GetUserId())
                {
                    _mapper.Map(updateCharacterDto, character);
                    await _context.SaveChangesAsync();
                    response.Data = _mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = await _context.Characters
                    .FirstAsync(c => c.Id == id && c.User.Id == GetUserId());
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                response.Data = _context.Characters
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return await Task.FromResult(response);
        }
    }
}