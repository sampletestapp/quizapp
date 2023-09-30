using AutoMapper;
using quizapp.api.Models;

namespace quizapp.api.Mappers
{
    public class EntityMapper : Profile
    {
        public EntityMapper()
        {
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));


            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
                

            CreateMap<Answer, AnswerDto>()
                .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
                .ForMember(dest => dest.Findings, opt => opt.MapFrom(src => src.Findings));
            CreateMap<AnswerDto, Answer>()
                .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.Recommendations))
                .ForMember(dest => dest.Findings, opt => opt.MapFrom(src => src.Findings));

            CreateMap<Recommendation, RecommendationDto>();
            CreateMap<RecommendationDto, Recommendation>();

            CreateMap<Finding, FindingDto>();
            CreateMap<FindingDto, Finding>();

        }
    }
}
