$(document).ready(function () {
  $("#questionType").change(function () {
    var selectedType = $(this).val();
    $(".answer-container").remove(); // Remove all existing answer containers

    if (selectedType === "trueFalse") {
      $("#questionType").after(`
              <div class="answer-container" id="trueFalseAnswer">
                  <label>Select</label><br />
                  <input type="radio" name="trueFalseAnswer" value="true">True<br />
                  <input type="radio" name="trueFalseAnswer" value="false">False<br />
              </div>
          `);
    } else if (selectedType === "blank") {
      $("#questionType").after(`
              <div class="answer-container" id="blankAnswer">
                  <label>Answer</label><br />
                  <input type="text" name="blankAnswer" /><br />
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
    // Use answerId to associate recommendation with the corresponding answer
  });

  $(document).on("click", ".addFinding", function () {
    var answerId = $(this).data("answer-id");
    // Use answerId to associate finding with the corresponding answer
  });

  $(document).on("click", ".removeAnswer", function () {
    $(this).parent().remove();
  });

  //on submitting form
  $("#saveQuestion").click(function (e) {
    e.preventDefault();

    var question = {
      text: $("#question").val(),
      questionTypeId: $("#questionType").val(),
      section: $("#section").val(),
      questionSeverityId: $("#questionSeverity").val(),
      answers: []
    };

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

    // Remove empty recommendation and finding arrays
    question.answers = question.answers.filter(
      (answer) =>
        answer.recommendations.length > 0 || answer.findings.length > 0
    );

    $("#questionList").append(
      '<div class="questionList">' + JSON.stringify(question) + "</div>"
    );
  });
});
