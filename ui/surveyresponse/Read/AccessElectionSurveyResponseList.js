const getDataUrl = 'http://localhost:5253/api/DataHandler/getsurveyresponse';

let data = []; // Declare data outside the fetch chain

document.addEventListener('DOMContentLoaded', function () {
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
                var rowspan = item.answerTexts && item.answerTexts.length > 1 ? item.answerTexts.length : 1;

                row.innerHTML = `
                            <td style="display: none;" rowspan="${rowspan}">${item.id}</td>
                            <td rowspan="${rowspan}">${item.surveyID}</td>
                            <td rowspan="${rowspan}">${item.questionNumber}</td>
                            <td rowspan="${rowspan}">${item.questionText}</td>
                            <td rowspan="${rowspan}">${item.questionTypeID !== null ? item.questionTypeID : ''}</td>
                            <td rowspan="${rowspan}">${item.answers !== null ? item.answers : '-'}</td>
                            <td>${item.answerTexts && item.answerTexts.length > 0 ? item.answerTexts[0] : ''}</td>
                            <td>${item.questionAnswerFindingText && item.questionAnswerFindingText.length > 0 ? item.questionAnswerFindingText[0] : ''}</td>
                            <td rowspan="${rowspan}">${item.additionalInfo !== null ? item.additionalInfo : ''}</td>
                            <td rowspan="${rowspan}"><input type="checkbox" class="dashboard-checkbox" data-id="${item.id}" ${item.availableForDashboard ? 'checked' : ''}></td>
                            <td rowspan="${rowspan}"><button class="edit-button" onclick="editRecord(${item.id})">Edit</button></td>
                        `;


                tbody.appendChild(row);

                if (rowspan > 1) {
                    for (let i = 1; i < rowspan; i++) {
                        const subRow = document.createElement('tr');
                        subRow.innerHTML = `<td>${item.answerTexts[i]}</td>
                    <td>${item.questionAnswerFindingText && item.questionAnswerFindingText.length > i ? item.questionAnswerFindingText[i] : ''}</td>`;
                        tbody.appendChild(subRow);
                    }
                }
            });
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });

});

