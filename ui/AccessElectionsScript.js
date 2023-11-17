$(document).ready(function () {
  var sectionDropdown = $("#section");
  for (var i = 1; i <= 26; i++) {
    var letter = String.fromCharCode(64 + i); // Convert number to corresponding ASCII character
    sectionDropdown.append(`<option value="${i}">${letter}</option>`);
  }

  $("#questionType").change(function () {
    var selectedType = $(this).val();
    $(".answer-container").remove();
    $(".answer").remove(); // Remove all existing answer containers
    $("#questionList").empty();

    if (selectedType === "blank") {
      $("#questionType").after(`
              <div class="answer" id="blankAnswer">
                  <label>Answer</label><br />
                  <input type="text" name="blankAnswer" /><br />
                  <div class="recomendationsContainer">
                      <label for="recommendations">Recommendations:</label>
                      <input type="text" name="recommendations" class="recommendations">
                      <button type="button" class="addRecommendation">Add Recommendation</button>
                  </div>
                  <div class="rfindingsContainer">
                      <label for="findings">Findings:</label>
                      <input type="text" name="findings" class="findings">
                  </div>
              </div>
          `);
    } else {
      $("#questionType").after(`
              <div class="answer-container" id="${selectedType}Answers">
                  <label>Answers:</label><br />
                  <div id="${selectedType}Options">
                  </div>
                  <button type="button" class="addAnswer" data-type="${selectedType}">Add Answer</button>
              </div>
          `);
    }
  });

  $("#questionType").trigger("change");

  $(document).on("click", ".addAnswer", function () {
    var type = $(this).data("type");
    var optionsContainer = $("#" + type + "Options");

    var answerId = Date.now(); // Generate a unique ID for the answer

    optionsContainer.append(`
          <div class="answer" data-answer-id="${answerId}">
              <input type="text" name="${type}Answer">
              <div class="recomendationsContainer">
                  <label for="recommendations">Recommendations:</label>
                  <input type="text" name="recommendations" class="recommendations" data-answer-id="${answerId}">
                  <button type="button" class="addRecommendation" data-answer-id="${answerId}">Add Recommendation</button>
              </div>
              <div class="rfindingsContainer">
                  <label for="findings">Findings:</label>
                  <input type="text" name="findings" class="findings" data-answer-id="${answerId}">
              </div>
              <button type="button" class="removeAnswer">❌Remove Answer</button>
          </div>
      `);
  });

  $(document).on("click", ".addRecommendation", function () {
    var answerId = $(this).data("answer-id");
    var recommendationId = Date.now(); // Generate a unique ID for the recommendation
    $(this).before(`
        <input type="text" name="recommendations" class="recommendations" data-answer-id="${answerId}" data-recommendation-id="${recommendationId}">
        <button type="button" class="removeRecommendation" data-answer-id="${answerId}" data-recommendation-id="${recommendationId}">❌Remove Recommendation</button>
    `);
  });


  $(document).on("click", ".removeRecommendation", function () {
    var answerId = $(this).data("answer-id");
    var recommendationId = $(this).data("recommendation-id");
    $(`input.recommendations[data-answer-id="${answerId}"][data-recommendation-id="${recommendationId}"]`).remove();
    $(this).remove();
  });

  $(document).on("click", ".removeAnswer", function () {
    $(this).parent().remove();
  });

  //on submitting form
  $("#questionForm").submit(function (e) {
    e.preventDefault();

    var questionTypeValue = $("#questionType").val();

    var questionTypeId;

    // Assign ids based on the selected value
    if (questionTypeValue === "singleSelection") {
      questionTypeId = 2;
    } else if (questionTypeValue === "multipleChoiceSelection") {
      questionTypeId = 3;
    } else {
      questionTypeId = 1;
    }

    // Now, questionTypeId will contain the appropriate integer value.


    //questionDto Object
    var questionDto = {
      QuestionText: $("#question").val(),
      QuestionTypeId: questionTypeId,  // An integer value
      QuestionNumber: $("#questionNumber").val(),
      SectionId: parseInt($("#section").val()),
      ZoneId: parseInt($("#zones").val()),
      QuestionSeverityId: parseInt($("#questionSeverity").val()), // An integer value
      QuestionAnswers: []  // An empty array or provide answers if needed
    };

    var answer = {
      questionAnswerText: '',
      recommendations: [],
      findings: ''
    };

    recomendation = {
      questionAnswerRecommendationText: ""
    }

    if (questionTypeValue === "blank") {
      var answerText = $("input[name='blankAnswer']").val();
      // Initialize an empty array to hold the recommendations
      var recommendationsList = [];
      // Assuming you have multiple input elements with class "recommendations"
      $("input.recommendations").each(function () {
        var recommendationText = $(this).val();
        // Create a recommendation object with the text
        var recommendation = {
          questionAnswerRecommendationText: recommendationText
        };
        // Add the recommendation to the list
        recommendationsList.push(recommendation);
      });
      var findings = $("input.findings").val();
      answer.questionAnswerText = answerText;
      answer.recommendations= recommendationsList;
      answer.findings = {
        questionAnswerFindingText: findings
      };

      questionDto.QuestionAnswers.push(answer);
    }
    else {
      $(".answer").each(function () {
        var answerText = $(this).find('input[name$="Answer"]').val();
        var answerId = $(this).data("answer-id");

        if (answerText !== "") {
          var answer = {
            questionAnswerText: answerText,
            recommendations: [],
            findings: ""
          };

          // Initialize an empty array to hold the recommendations
          var recommendationsList = [];
          // Assuming you have multiple input elements with class "recommendations"
          $(`input.recommendations[data-answer-id="${answerId}"]`).each(function () {
            var recommendationText = $(this).val();
            // Create a recommendation object with the text
            var recommendation = {
              questionAnswerRecommendationText: recommendationText
            };
            // Add the recommendation to the list
            recommendationsList.push(recommendation);
          });

          // Get associated findings
          var finding = $(`input.findings[data-answer-id="${answerId}"]`).val();
          answer.findings = {
            questionAnswerFindingText: finding
          };
          answer.recommendations = recommendationsList;
          questionDto.QuestionAnswers.push(answer);
        }
      });
    }

    $.ajax({
      url: 'http://localhost:5253/api/Questions',  // Replace with your actual endpoint
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(questionDto),
      success: function (result) {
        console.log('Question created successfully', result);
        // Handle success here
      },
      error: function (error) {
        console.error('Error creating question', error);
        // Handle error here
      }
    });

    $("#questionList").html(
      '<div class="questionList">' + JSON.stringify(questionDto) + "</div>"
    );
  });
});
