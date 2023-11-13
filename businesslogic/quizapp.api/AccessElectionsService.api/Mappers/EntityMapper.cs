using AutoMapper;
using AccessElectionsService.api.Models;

namespace AccessElectionsService.api.Mappers
{
    public class EntityMapper : Profile
    {
        public EntityMapper()
        {
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.QuestionAnswers, opt => opt.MapFrom(src => src.QuestionAnswers));


            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.QuestionAnswers, opt => opt.MapFrom(src => src.QuestionAnswers));
                

            CreateMap<QuestionAnswer, QuestionAnswerDto>()
                .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
                .ForMember(dest => dest.Findings, opt => opt.MapFrom(src => src.Findings));
            CreateMap<QuestionAnswerDto, QuestionAnswer>()
                .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
                .ForMember(dest => dest.Findings, opt => opt.MapFrom(src => src.Findings));

            CreateMap<QuestionAnswerRecommendation, QuestionAnswerRecommendationDto>();
            CreateMap<QuestionAnswerRecommendationDto, QuestionAnswerRecommendation>();

            CreateMap<QuestionAnswerFinding, QuestionAnswerFindingDto>();
            CreateMap<QuestionAnswerFindingDto, QuestionAnswerFinding>();

        }
    }
}
