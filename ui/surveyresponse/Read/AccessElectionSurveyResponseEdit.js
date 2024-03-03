const getQuestionUrl = 'http://localhost:5253/api/Questions/';
const getAnswerIdsUrl = 'http://localhost:5253/api/DataHandler/get-answer-ids';
const updateRecordUrl = 'http://localhost:5253/api/DataHandler/updatesurveyresponse';

let currentRecord = null;

function editRecord(id) {
  // Find the record in the data array
  currentRecord = data.find(item => item.id === id);

  // Make AJAX call to fetch question details
  $.ajax({
    url: `${getQuestionUrl}${currentRecord.questionID}`,
    type: 'GET',
    success: function (questionDetails) {
      // Show or hide the answer text area based on the answer ID
      var answers = document.getElementById('answers');
      if (!currentRecord.answerID) {
        answers.style.display = 'block';
      } else {
        answers.style.display = 'none';
      }

      // Clear previous question inputs
      var questionInputs = document.getElementById('question-inputs');
      questionInputs.innerHTML = '';

      // Create question text element
      var questionText = document.createElement('p');
      questionText.textContent = questionDetails.questionText;
      questionInputs.appendChild(questionText);

      var answerOrAdditionalInfo = document.getElementById('answerOrAdditionalInfo');
      // Create input elements based on question type
      if(questionDetails.questionTypeId === 1){
        answerOrAdditionalInfo.value = currentRecord.answers;
      }
      else if (questionDetails.questionTypeId === 2) { 
        answerOrAdditionalInfo.value = currentRecord.additionalInfo;
        questionDetails.questionAnswers.forEach(answer => {
          var radioButton = document.createElement('input');
          radioButton.type = 'radio';
          radioButton.name = 'answer';
          radioButton.value = answer.id;
          radioButton.checked = parseInt(currentRecord.answers) === answer.id;

          var label = document.createElement('label');
          label.textContent = answer.questionAnswerText;

          questionInputs.appendChild(radioButton);
          questionInputs.appendChild(label);
          questionInputs.appendChild(document.createElement('br'));
        });
      } else if (questionDetails.questionTypeId === 3) { // Multiple selection
        answerOrAdditionalInfo.value = currentRecord.additionalInfo;
        var answerIdsArray = currentRecord.answers.split(",").map(Number); // Ensure answerIds is an array
        questionDetails.questionAnswers.forEach(answer => {
          var checkbox = document.createElement('input');
          checkbox.type = 'checkbox';
          checkbox.name = 'answer';
          checkbox.value = answer.id;
          checkbox.checked = answerIdsArray.indexOf(answer.id) !== -1;

          var label = document.createElement('label');
          label.textContent = answer.questionAnswerText;

          questionInputs.appendChild(checkbox);
          questionInputs.appendChild(label);
          questionInputs.appendChild(document.createElement('br'));
        });

      }
      // Open the modal and populate the data
      document.getElementById('edit-modal').style.display = 'block';
      document.getElementById('question-number').textContent = currentRecord.questionNumber;
      document.getElementById('answer').value = currentRecord.answerText || '';
    },
    error: function (error) {
      console.error('Error fetching question details', error);
    }
  });
}


function closeEditModal() {
  document.getElementById('edit-modal').style.display = 'none';
}

function saveRecord() {
  // Update the currentRecord with the selected answer ID based on the question type
  var answerOrAdditionalInfo = document.getElementById('answerOrAdditionalInfo');
  if(currentRecord.questionTypeID === 1){
    currentRecord.answers = answerOrAdditionalInfo.value;
  }
  else
  {
    currentRecord.additionalInfo = answerOrAdditionalInfo.value;
  }
  var questionInputs = document.getElementsByName('answer');
  if (questionInputs.length > 0) {
    var selectedAnswers = Array.from(questionInputs)
      .filter(input => input.checked)
      .map(input => parseInt(input.value)).join(",");
    currentRecord.Answers = selectedAnswers.length > 0 ? selectedAnswers : null;
  }
  console.log(currentRecord);
  $.ajax({
    url: updateRecordUrl,
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify(currentRecord),
    success: function (response) {
      console.log(response);
      closeEditModal();
      location.reload();
    },
    error: function (error) {
      console.error('Error saving record:', error);
    }
  });
}
