const getDataUrl = 'http://localhost:5253/api/DataHandler/getsurveyresponse';
const getSurveyStatusUrl = 'http://localhost:5253/api/DataHandler/getSurveyStatus';
const getphotosUrl = 'http://localhost:5253/api/DataHandler/getPhotos';
const uploadPhotoUrl = 'http://localhost:5253/api/DataHandler/uploadPhotos';

let data = []; // Declare data outside the fetch chain


function openPhotosModal() {
    document.getElementById('photos-modal').style.display = 'block';
}

function closePhotosModal() {
    document.getElementById('photos-modal').style.display = 'none';
}

function fetchAndDisplayPhotos(id) {
    fetch(`${getphotosUrl}?Id=${id}`)
        .then(response => response.json())
        .then(photoData => {
            const photoListContainer = document.getElementById('photos-list');
            photoListContainer.innerHTML = '';

            // Display photos in the photo list container
            photoData.forEach(photoUrl => {
                const photoLink = document.createElement('a');
                photoLink.href = photoUrl;
                photoLink.textContent = photoUrl; // Display the URL as text for now
                photoListContainer.appendChild(photoLink);
                photoListContainer.appendChild(document.createElement('br')); // Line break between links
            });

            openPhotosModal(); // Open the modal after fetching and displaying photos
        })
        .catch(error => {
            console.error('Error fetching photos:', error);
        });
}

function uploadPhoto() {
    const fileInput = document.getElementById('photo-file');
    const file = fileInput.files[0];

    const formData = new FormData();
    formData.append('photo', file);

    fetch(uploadPhotoUrl, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.ok) {
            console.log('Photo uploaded successfully');
            closePhotosModal(); // Close the modal after uploading photo
        } else {
            throw new Error('Failed to upload photo');
        }
    })
    .catch(error => {
        console.error('Error uploading photo:', error);
    });
}

document.addEventListener('DOMContentLoaded', function () {
    var urlParams = new URLSearchParams(window.location.search);
    var pplid = urlParams.get('pplId');
    var electionid = urlParams.get('electionId');

    // Fetch election status
    fetch(`${getSurveyStatusUrl}?PPLID=${pplid}&ElectionID=${electionid}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch election status');
            }
            return response.json();
        })
        .then(electionStatusData => {
            const surveyStatusDropdown = document.getElementById('survey-status');
            const selectedStatus = electionStatusData.status; // Assuming the response contains the status value

            // Loop through each option in the dropdown
            for (let i = 0; i < surveyStatusDropdown.options.length; i++) {
                // Check if the option value matches the selected status
                if (surveyStatusDropdown.options[i].value === selectedStatus) {
                    // Set the selected attribute for the matching option
                    surveyStatusDropdown.options[i].selected = true;
                    break; // Exit the loop after setting the selected option
                }
            }
        })
        .catch(error => {
            console.error('Error fetching election status:', error);
        });

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
                            <td rowspan="${rowspan}">${item.questionText}</td>
                            <td rowspan="${rowspan}">${item.questionTypeID !== null ? item.questionTypeID : ''}</td>
                            <td>${item.answerTexts && item.answerTexts.length > 0 ? item.answerTexts[0] : ''}</td>
                            <td>${item.questionAnswerFindingText && item.questionAnswerFindingText.length > 0 ? item.questionAnswerFindingText[0] : ''}</td>
                            <td rowspan="${rowspan}">${item.additionalInfo !== null ? item.additionalInfo : ''}</td>
                            <td rowspan="${rowspan}">
                                <button onclick="fetchAndDisplayPhotos(${item.id})">View Photos</button>
                                <div id="photos-${item.id}"></div>
                            </td>
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

