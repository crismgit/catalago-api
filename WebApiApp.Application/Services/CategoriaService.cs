using AutoMapper;
using WebApiApp.Application.DTOs;
using WebApiApp.Application.Interfaces;
using WebApiApp.Domain.Entities;
using WebApiApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiApp.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private ICategoriaRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoriaService(ICategoriaRepository categoryRepository, 
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoriaDTO>> GetCategorias()
        {
            var categoriesEntity = await _categoryRepository.GetCategoriasAsync();
            return _mapper.Map<IEnumerable<CategoriaDTO>>(categoriesEntity);
        }

        public async Task<CategoriaDTO> GetById(int? id)
        {
            var categoryEntity = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoriaDTO>(categoryEntity);
        }

        public async Task Add(CategoriaDTO categoryDto)
        {
            var categoryEntity = _mapper.Map<Categoria>(categoryDto);
            await _categoryRepository.CreateAsync(categoryEntity);
        }

        public async Task Update(CategoriaDTO categoryDto)
        {
            var categoryEntity = _mapper.Map<Categoria>(categoryDto);
            await _categoryRepository.UpdateAsync(categoryEntity);
        }

        public async Task Remove(int? id)
        {
            // Usamos await em vez de .Result para evitar travamentos (deadlocks)
            var categoryEntity = await _categoryRepository.GetByIdAsync(id);

            // Validação de segurança: só remove se o registro realmente existir no banco
            if (categoryEntity != null)
            {
                await _categoryRepository.RemoveAsync(categoryEntity);
            }
        }
    }
}
