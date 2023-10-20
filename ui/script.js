$(document).ready(function(){
    // Function to handle question type change
    $('#questionType').change(function(){
        var type = $(this).val();
        if (type == 'single' || type == 'multiple') {
            $('#choices').show();
        } else {
            $('#choices').hide();
        }
    });

    // Function to add more choices
    $('#choices').on('click', '.addChoice', function(){
        var newChoiceGroup = $('<div class="choice-group">');
        newChoiceGroup.append('<input type="text" class="choice" placeholder="Enter choice">');
        newChoiceGroup.append('<button class="addChoice">Add Choice</button>');
        newChoiceGroup.append('<button class="remove-button removeChoice">Remove</button>');
        $('#choices').append(newChoiceGroup);
    });

    // Function to add more findings
    $('#findings').on('click', '.addFinding', function(){
        var newFindingGroup = $('<div class="finding-group">');
        newFindingGroup.append('<input type="text" class="finding" placeholder="Enter finding">');
        newFindingGroup.append('<button class="addFinding">Add Finding</button>');
        newFindingGroup.append('<button class="remove-button removeFinding">Remove</button>');
        $('#findings').append(newFindingGroup);
    });

    // Function to add more recommendations
    $('#recommendations').on('click', '.addRecommendation', function(){
        var newRecommendationGroup = $('<div class="recommendation-group">');
        newRecommendationGroup.append('<input type="text" class="recommendation" placeholder="Enter recommendation">');
        newRecommendationGroup.append('<button class="addRecommendation">Add Recommendation</button>');
        newRecommendationGroup.append('<button class="remove-button removeRecommendation">Remove</button>');
        $('#recommendations').append(newRecommendationGroup);
    });

    // Function to remove choices
    $('#choices').on('click', '.removeChoice', function(){
        $(this).parent('.choice-group').remove();
    });

    // Function to remove findings
    $('#findings').on('click', '.removeFinding', function(){
        $(this).parent('.finding-group').remove();
    });

    // Function to remove recommendations
    $('#recommendations').on('click', '.removeRecommendation', function(){
        $(this).parent('.recommendation-group').remove();
    });

    // Function to save the question
    $('#saveQuestion').click(function(){
        var question = $('#question').val();
        var type = $('#questionType').val();
        var choices = [];
        var findings = [];
        var recommendations = [];

        // Get choices
        $('.choice').each(function(){
            choices.push($(this).val());
        });

        // Get findings
        $('.finding').each(function(){
            findings.push($(this).val());
        });

        // Get recommendations
        $('.recommendation').each(function(){
            recommendations.push($(this).val());
        });

        var data = {
            question: question,
            type: type,
            choices: choices,
            findings: findings,
            recommendations: recommendations
        };

        // Send data to the API
        $.ajax({
            url: 'http://localhost:5253/',
            type: 'POST',
            data: data,
            success: function(response) {
                alert('Question saved successfully!');
            },
            error: function(error) {
                alert('Error saving question');
            }
        });
    });
});
