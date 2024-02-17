let data = []; // Declare data outside the fetch chain

fetch('http://localhost:5253/api/DataHandler/get-records?PPLID=70002709&ElectionID=123')
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

let currentRecord = null;

function editRecord(id) {
  // Find the record in the data array
  currentRecord = data.find(item => item.id === id);

  $.ajax({
    url: 'http://localhost:5253/api/Questions/' +  currentRecord.questionID,
    type: 'GET',
    success: function (questionDetails) {
       console.log(questionDetails)
    },
    error: function (error) {
        console.error('Error fetching question details', error);
    }
  });

  // Open the modal and populate the data
  document.getElementById('edit-modal').style.display = 'block';
  document.getElementById('question-number').textContent = currentRecord.questionNumber;
  document.getElementById('answer').value = currentRecord.answerText || '';
}

function closeEditModal() {
  document.getElementById('edit-modal').style.display = 'none';
}

function saveRecord() {
  currentRecord.answerText = document.getElementById('answer').value;
  closeEditModal();
}
