$(document).ready(function () {
  $("#questionType").change(function () {
    var selectedType = $(this).val();
    $(".answer-container").hide();
    if (selectedType === "trueFalse") {
      $("#" + selectedType + "Answer").show();
      $('[name="trueFalseAnswer"]').prop("checked", false);
    } else if (selectedType === "blank") {
      $("#" + selectedType + "Answer").show();
    } else {
      $("#" + selectedType + "Answers").show();
      $("#" + selectedType + "Options").empty();
    }
  });

  //On page load trigger change event
  $("#questionType").trigger("change");

  $(".addAnswer").click(function () {
    var type = $(this).data("type");
    var optionsContainer = $("#" + type + "Options");
    optionsContainer.append(`<div><input type="text" name="${type}Answer">
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
              <button type="button" class="removeAnswer">❌Remove Answer</button></div>`);
  });

  $(document).on("click", ".removeAnswer", function () {
    $(this).parent().remove();
  });
  $(document).on("click", ".removeRecommendation", function () {
    $(this).prev().remove();
    $(this).remove();
  });
  $(document).on("click", ".removeFinding", function () {
    $(this).prev().remove();
    $(this).remove();
  });

  $(document).on("click", ".addRecommendation", function () {
    $(
      this
    ).before(`<input type="text" name="recommendations" class="recommendations">
          <button type="button" class="removeRecommendation">❌Remove Recomendation</button>`);
  });

  $(document).on("click", ".addFinding", function () {
    $(this).before(`<input type="text" name="findings" class="findings">
          <button type="button" class="removeFinding">❌Remove Finding</button>`);
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

    $(".answer-container").each(function () {
      var answerText = $(this).find('input[name$="Answer"]').val();

      if (answerText !== "") {
        var answer = {
          answerText: answerText,
          recommendations: [],
          findings: []
        };

        $(this)
          .find(".recomendationsContainer .recommendations")
          .each(function () {
            var recommendation = $(this).val();
            if (recommendation !== "") {
              answer.recommendations.push(recommendation);
            }
          });

        $(this)
          .find(".rfindingsContainer .findings")
          .each(function () {
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
