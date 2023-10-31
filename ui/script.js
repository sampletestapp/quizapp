$(document).ready(function () {
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
                      <button type="button" class="addFinding">Add Finding</button>
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
                  <button type="button" class="addFinding" data-answer-id="${answerId}">Add Finding</button>
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

  $(document).on("click", ".addFinding", function () {
    var answerId = $(this).data("answer-id");
    var findingId = Date.now(); // Generate a unique ID for the finding
    $(this).before(`
        <input type="text" name="findings" class="findings" data-answer-id="${answerId}" data-finding-id="${findingId}">
        <button type="button" class="removeFinding" data-answer-id="${answerId}" data-finding-id="${findingId}">❌Remove Finding</button>
    `);
  });

  $(document).on("click", ".removeRecommendation", function () {
    var answerId = $(this).data("answer-id");
    var recommendationId = $(this).data("recommendation-id");
    $(`input.recommendations[data-answer-id="${answerId}"][data-recommendation-id="${recommendationId}"]`).remove();
    $(this).remove();
  });

  $(document).on("click", ".removeFinding", function () {
    var answerId = $(this).data("answer-id");
    var findingId = $(this).data("finding-id");
    $(`input.findings[data-answer-id="${answerId}"][data-finding-id="${findingId}"]`).remove();
    $(this).remove();
  });

  $(document).on("click", ".removeAnswer", function () {
    $(this).parent().remove();
  });

  //on submitting form
  $("#saveQuestion").click(function (e) {
    e.preventDefault();

    var question = {
      questionId: $("#questionId").val(),
      text: $("#question").val(),
      questionTypeId: $("#questionType").val(),
      section: $("#section").val(),
      zones: $("#zones").val(),
      questionSeverityId: $("#questionSeverity").val(),
      answers: []
    };
    if (question.questionTypeId === "blank") {
      var answerText = $("input[name='blankAnswer']").val();
      var recommendations = $("input.recommendations").map(function () {
        return $(this).val();
      }).get();
      var findings = $("input.findings").map(function () {
        return $(this).val();
      }).get();

      var answer = {
        answerText: answerText,
        recommendations: recommendations,
        findings: findings
      };

      question.answers.push(answer);
    }
    else {
      $(".answer").each(function () {
        var answerText = $(this).find('input[name$="Answer"]').val();
        var answerId = $(this).data("answer-id");

        if (answerText !== "") {
          var answer = {
            answerText: answerText,
            recommendations: [],
            findings: []
          };

          // Get associated recommendations
          var recommendations = $(`input.recommendations[data-answer-id="${answerId}"]`);
          recommendations.each(function () {
            var recommendation = $(this).val();
            if (recommendation !== "") {
              answer.recommendations.push(recommendation);
            }
          });

          // Get associated findings
          var findings = $(`input.findings[data-answer-id="${answerId}"]`);
          findings.each(function () {
            var finding = $(this).val();
            if (finding !== "") {
              answer.findings.push(finding);
            }
          });

          question.answers.push(answer);
        }
      });
    }

    // Remove empty recommendation and finding arrays
    question.answers = question.answers.filter(
      (answer) =>
        answer.recommendations.length > 0 || answer.findings.length > 0
    );

    $("#questionList").html(
      '<div class="questionList">' + JSON.stringify(question) + "</div>"
    );
  });
});
