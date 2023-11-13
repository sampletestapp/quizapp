﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AccessElectionsService.api.Models
{
    public class QuestionAnswerRecommendation
    {
        public int Id { get; set; }
        public string QuestionAnswerRecommendationText { get; set; }
        public int QuestionAnswerId { get; set; }
        public QuestionAnswer QuestionAnswer { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
