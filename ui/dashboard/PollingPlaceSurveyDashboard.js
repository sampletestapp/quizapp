const getFileExportStatsUrl = 'http://localhost:5253/api/DataHandler/pollingPlaceSurveyDetails';
const downloadscriptUrl = 'http://localhost:5253/api/DataHandler/downloadScript';
let data = [];

document.addEventListener('DOMContentLoaded', function () {
    fetch(getFileExportStatsUrl)
        .then(response => response.json())
        .then(data => {
            const tbody = document.getElementById('polling-place-survey-details');

            data.forEach(item => {
                const row = document.createElement('tr');
                row.innerHTML = `
              <td><button onclick="downloadFile('${item.filename}')">Download</button></td>
              <td>${item.filename}</td>
              <td>${item['noOfTimesFileExported']}</td>
              <td>${item['noOfFilesStatusCompleted']}</td>
              
            `;
                tbody.appendChild(row);
            });
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });
});

function downloadFile(filename) {
    // Call the API to download the file
    fetch(`${downloadscriptUrl}?filename=${filename}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to download file');
            }
            return response.blob(); // Get the response as a blob
        })
        .then(blob => {
            // Create a temporary URL for the blob
            const url = window.URL.createObjectURL(blob);
            // Create a temporary link element to trigger the download
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a); // Append the link to the body
            a.click(); // Click the link to start the download
            document.body.removeChild(a); // Remove the link from the body
            window.URL.revokeObjectURL(url); // Revoke the URL object to free up memory
        })
        .catch(error => {
            console.error('Error downloading file:', error);
        });
}

