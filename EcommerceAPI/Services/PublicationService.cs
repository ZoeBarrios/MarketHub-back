﻿using AutoMapper;
using EcommerceAPI.Models.Publication;
using EcommerceAPI.Models.Publication.Dto;
using EcommerceAPI.Repositories;
using System.Net;
using System.Web.Http;

namespace EcommerceAPI.Services
{
    public class PublicationService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IMapper _mapper;
        private string host;

        public PublicationService(IConfiguration config,IPublicationRepository publicationRepository, IMapper mapper)
        {
            host = config.GetSection("hostUrl:url").Value;
            _publicationRepository = publicationRepository;
            _mapper = mapper;
        }


        public async Task<List<PublicationsDto>> GetAll()
        {
            var lista = await _publicationRepository.GetAll(); 
            var filteredList = lista.Where(p => !p.IsPaused).ToList(); 
            return _mapper.Map<List<PublicationsDto>>(filteredList);
        }



        public async Task<List<PublicationsDto>> GetAllByName(string name)
        {
            var lista = await _publicationRepository.GetAll();
            var filteredList = lista.Where(p =>
            p.Name.ToLower().Contains(name.ToLower()) && !p.IsPaused
            ).ToList();
            return _mapper.Map<List<PublicationsDto>>(filteredList);
        }

        public async Task<List<PublicationsDto>> GetAllByCategory(int idCategory)
        {
            var lista = await _publicationRepository.GetAll(p => p.CategoryId==idCategory);
            return _mapper.Map<List<PublicationsDto>>(lista);
        }

        public async Task<List<PublicationsDto>> GetAllByUserId(int id){
            var lista = await _publicationRepository.GetAll(p => p.UserId==id);
            return _mapper.Map<List<PublicationsDto>>(lista);
        }



        public async Task<PublicationDto> GetById(int id)
        {
            Publication publication = await _publicationRepository.GetOne(u => u.Id == id);

            if (publication == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }


            var mapped = _mapper.Map<PublicationDto>(publication);

            return mapped;
        }

        public async Task<PublicationDto> Create(CreatePublicationDto createPublicationDto)
        {
            if (createPublicationDto.Image != null)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + createPublicationDto.Image.FileName;
                string baseUrl = host;
                string imageUrl = baseUrl + "Images/" + uniqueFileName;

                string imagePath = Path.Combine("Images", uniqueFileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await createPublicationDto.Image.CopyToAsync(stream);
                }

                createPublicationDto.ImageUrl = imageUrl;
            }
            var publication = _mapper.Map<Publication>(createPublicationDto);


            await _publicationRepository.Add(publication);

            return _mapper.Map<PublicationDto>(publication);
        }

        public async Task<PublicationDto> UpdateById(int id, UpdatePublicationDto updatePublicationDto)
        {
            var publication = await _publicationRepository.GetOne(p => p.Id == id);

            if (publication == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var updated = _mapper.Map(updatePublicationDto, publication);

            return _mapper.Map<PublicationDto>(await _publicationRepository.Update(updated));
        }

        public async Task DeleteById(int id)
        {
            Publication publication = await _publicationRepository.GetOne(p => p.Id == id);

            if (publication == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            publication.IsPaused = true;
            await _publicationRepository.Update(publication);
        }

        public async Task<List<Publication>> GetPublicationsByIds(List<int> publicationsIdS)
        {
            if (publicationsIdS == null || publicationsIdS.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var publications = await _publicationRepository.GetAll(p => publicationsIdS.Contains(p.Id));
            return publications.ToList();
        }








    }
}
