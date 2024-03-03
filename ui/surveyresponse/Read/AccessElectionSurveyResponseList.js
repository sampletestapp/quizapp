const getDataUrl = 'http://localhost:5253/api/DataHandler/getsurveyresponse';

let data = []; // Declare data outside the fetch chain

document.addEventListener('DOMContentLoaded', function() {
  var urlParams = new URLSearchParams(window.location.search);
  var pplid = urlParams.get('pplId');
  var electionid = urlParams.get('electionId');

  fetch(`${getDataUrl}?PPLID=${pplid}&ElectionID=${electionid}`)
    .then(response => response.json())
    .then(apiData => {
      data = apiData; // Populate data with the fetched API data
      const tbody = document.getElementById('survey-data');

      data.forEach(item => {
        const row = document.createElement('tr');
        row.innerHTML = `
          <td>${item.surveyID}</td>
          <td>${item.questionNumber}</td>
          <td>${item.questionTypeID !== null ? item.questionTypeID : ''}</td>
          <td>${item.answers !== null ? item.answers : '-'}</td>
          <td>${item.additionalInfo !== null ? item.additionalInfo : ''}</td>
          <td><button class="edit-button" onclick="editRecord(${item.id})">Edit</button></td>
        `;
        tbody.appendChild(row);
      });
    })
    .catch(error => {
      console.error('Error fetching data:', error);
    });

});

