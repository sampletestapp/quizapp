let data = []; // Declare data outside the fetch chain
document.addEventListener('DOMContentLoaded', function() {
  var urlParams = new URLSearchParams(window.location.search);
  var pplid = urlParams.get('pplId');
  var electionid = urlParams.get('electionId');

  fetch(`http://localhost:5253/api/DataHandler/get-records?PPLID=${pplid}&ElectionID=${electionid}`)
  .then(response => response.json())
  .then(apiData => {
    data = apiData; // Populate data with the fetched API data
    const tbody = document.getElementById('survey-data');

    data.forEach(item => {
      const row = document.createElement('tr');
      row.innerHTML = `
        <td>${item.id}</td>
        <td>${item.surveyID}</td>
        <td>${item.questionID}</td>
        <td>${item.questionNumber}</td>
        <td>${item.answerID !== null ? item.answerID : '-'}</td>
        <td>${item.answerText !== null ? item.answerText : '-'}</td>
        <td><button class="edit-button" onclick="editRecord(${item.id})">Edit</button></td>
      `;
      tbody.appendChild(row);
    });
  })
  .catch(error => {
    console.error('Error fetching data:', error);
  });

});
let currentRecord = null;

function editRecord(id) {
  // Find the record in the data array
  currentRecord = data.find(item => item.id === id);

  // Make AJAX call to fetch question details
  $.ajax({
    url: 'http://localhost:5253/api/Questions/' + currentRecord.questionID,
    type: 'GET',
    success: function (questionDetails) {
      // Show or hide the answer text area based on the answer ID
      var answerTextArea = document.getElementById('answertext');
      if (!currentRecord.answerID) {
        answerTextArea.style.display = 'block';
      } else {
        answerTextArea.style.display = 'none';
      }


      // Clear previous question inputs
      var questionInputs = document.getElementById('question-inputs');
      questionInputs.innerHTML = '';

      // Create question text element
      var questionText = document.createElement('p');
      questionText.textContent = questionDetails.questionText;
      questionInputs.appendChild(questionText);

      // Create input elements based on question type
      if (questionDetails.questionTypeId === 2) { // Single selection
        questionDetails.questionAnswers.forEach(answer => {
          var radioButton = document.createElement('input');
          radioButton.type = 'radio';
          radioButton.name = 'answer';
          radioButton.value = answer.id;
          radioButton.checked = currentRecord.answerID === answer.id;

          var label = document.createElement('label');
          label.textContent = answer.questionAnswerText;

          questionInputs.appendChild(radioButton);
          questionInputs.appendChild(label);
          questionInputs.appendChild(document.createElement('br'));
        });
      } else if (questionDetails.questionTypeId === 3) { // Multiple selection
        $.ajax({
          url: 'http://localhost:5253/api/DataHandler/get-answer-ids?questionId=' + currentRecord.questionID,
          type: 'GET',
          success: function (answerIds) {
            var answerIdsArray = Array.isArray(answerIds) ? answerIds : [answerIds]; // Ensure answerIds is an array
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
          },
          error: function (error) {
            console.error('Error fetching question details', error);
          }
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
  // Update the currentRecord with the new answer
  currentRecord.answerText = document.getElementById('answer').value;


  // Update the currentRecord with the selected answer ID based on the question type
  var questionInputs = document.getElementsByName('answer');
  if (questionInputs.length > 0) {
    var selectedAnswers = Array.from(questionInputs)
      .filter(input => input.checked)
      .map(input => parseInt(input.value));
    currentRecord.answerID = selectedAnswers.length > 0 ? selectedAnswers : null;
    // }
  }
  console.log(currentRecord);
  $.ajax({
    url: 'http://localhost:5253/api/DataHandler/update-questionanswer',
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

