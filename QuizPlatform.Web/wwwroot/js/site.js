function AddQuiz() {
    var form = $('#NewQuizForm')[0]; 
    var formData = new FormData(form);

    $.ajax({
        url: '/Quiz/Create',
        type: 'POST',
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (response) {
            if (response && response.statusCode === 200) {
                $('#addQuizModal').modal('hide');
                toastr.success(response.message || 'Quiz Added Successfully', 'Success');
                setTimeout(function () {
                    location.reload();
                }, 1000);
            } else {
                toastr.error(response.message, 'Wrong');
            }
        },
        error: function (xhr, status, error) {
            toastr.error('Something Wrong Happened', 'خطأ');
        }
    });
}
function DeleteQuiz(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Quiz/Delete/' + id,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.statusCode === 200) {
                        toastr.success(response.message || 'Deleted successfully!');
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    } else {
                        toastr.error(response.message || 'Failed to delete.');
                    }
                },
                error: function () {
                    toastr.error('Something went wrong.');
                }
            });
        }
    });
}
function openEditQuizModal(element) {
    const id = $(element).data('id');
    const name = $(element).data('name');
    const description = $(element).data('description');
    const image = $(element).data('image');

    $('#editQuizId').val(id);
    $('#editQuizName').val(name);
    $('#editQuizDescription').val(description);

    if (image) {
        $('#editQuizImagePreview').attr('src', image).show();
    } else {
        $('#editQuizImagePreview').hide();
    }

    var modal = new bootstrap.Modal(document.getElementById('editQuizModal'));
    modal.show();
}

function UpdateQuiz() {
 
    var form = $('#EditQuizForm')[0];
    var formData = new FormData(form);

    $.ajax({
        url: '/Quiz/Edit',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.statusCode === 200) {
                $('#editQuizModal').modal('hide');
                toastr.success(response.message || 'Quiz updated!');
                setTimeout(() => location.reload(), 1000);
            } else {
                toastr.error(response.message || 'Update failed.');
            }
        },
        error: function () {
            toastr.error('Something went wrong.');
        }
    });
}
function saveTextQuestion() {
    var formData = $('#textForm').serialize();

    $.ajax({
        url: '/Question/CreateText',
        type: 'POST',
        data: formData,
        success: function (response) {
            toastr.success(response.message || 'Text Question Saved!');
            $('#textQuestionModal').modal('hide');

            setTimeout(() => location.reload(), 500);
        },
        error: function (xhr) {
            toastr.error('Something went wrong!');
        }
    });
}
function saveOptionsQuestion() {
    var formData = $('#optionsForm').serialize();

    $.ajax({
        url: '/Question/CreateOptions',
        type: 'POST',
        data: formData,
        success: function (response) {
            $('#optionsQuestionModal').modal('hide');

            toastr.success(response.message || 'Options Question Saved!');
            setTimeout(() => location.reload(), 500);
        },
        error: function (xhr) {
            toastr.error('Something went wrong!');
        }
    });
}
function deleteQuestion(questionId, quizId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This question will be permanently deleted!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/Question/Delete`,
                type: 'POST',
                data: {
                    questionId: questionId,
                    quizId: quizId,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.statusCode === 200) {
                        $('#addQuizModal').modal('hide');

                        toastr.success(response.message || 'Deleted successfully!');
                        setTimeout(function () {
                            location.reload();
                        }, 500);
                    } else {
                        toastr.error(response.message || 'Failed to delete.');
                    }
                },
                error: function () {
                    toastr.error('Something went wrong.');
                }
            });
        }
    });
}
function UpdateTextQuestion() {
    var formData = $('#optionsForm').serialize();

    $.ajax({
        url: '/Question/UpdateTextQuestion',
        type: 'POST',
        data: formData,
        success: function (response) {
            $('#optionsQuestionModal').modal('hide');

            toastr.success(response.message || 'Options Question Saved!');
            setTimeout(() => location.reload(), 500);
        },
        error: function (xhr) {
            toastr.error('Something went wrong!');
        }
    });
}
function filterQuizzes() {
    var searchTerm = $('#searchInput').val();
    $.ajax({
        url: '/Quiz/Index',
        type: 'GET',
        data: { name: searchTerm },
        success: function (result) {
            $('#quizContainer').html(result);
        },
        error: function () {
            alert('Error while filtering.');
        }
    });
}
function submitQuizAnswers() {
    var answers = [];

    $('.quiz-question-block').each(function () {
        var $block = $(this);
        var questionId = $block.data('question-id');
        var answerText = $block.find('input[type="text"]').val();
        var selectedOption = $block.find('input[type="radio"]:checked').val();

        answers.push({
            questionId: questionId,
            answerText: answerText || null,
            selectedChoiceId: selectedOption || null
        });
    });

    var quizId = $('#QuizId').val();

    var data = {
        quizId: quizId,  
        answers: answers
    };


    $.ajax({
        url: '/Quiz/SubmitQuizAnswers',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            checkUserScore(quizId);
            setTimeout(() => location.reload(), 3000);


        },
        error: function () {
            toastr.error('Something went wrong!');
        }
    });
}
function checkUserScore(quizId) {
    $.ajax({
        url: `/Quiz/UserScore?quizId=${quizId}`,
        type: 'POST',
        success: function (res) {
            if (res.statusCode === 200) {
                Swal.fire(`You got ${res.data.correctAnswers} out of ${res.data.totalQuestions} correct!`);
            }
        }
    });
}

function submitQuestion(questionId, questionType) {
    if (questionType === 'Text') {
        EditQuestionText(questionId);
    } else if (questionType === 'Choices') {
        EditChoicesQuestion(questionId);
    } else {
        toastr.error('Unknown question type!');
    }
}

function EditQuestionText(questionId) {
    var form = $('#editQuestionForm-' + questionId);

    var dto = {
        Id: questionId,
        QuizId: form.find('input[name="QuizId"]').val(),
        Text: form.find('textarea[name="Text"]').val(),
        CorrectAnswerText: form.find('textarea[name="CorrectAnswerText"]').val()
    };

    $.ajax({
        url: '/Question/UpdateTextQuestion',
        method: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: function (res) {
            toastr.success('Question updated successfully!');
            $('#editQuestionModal-' + questionId).modal('hide');
            setTimeout(() => location.reload(), 1000);
        },
        error: function (err) {
            toastr.error('Something went wrong!');
        }
    });
}

function EditChoicesQuestion(questionId) {
    var form = $('#editQuestionForm-' + questionId);

    var dto = {
        Id: questionId,
        QuizId: form.find('input[name="QuizId"]').val(),
        Text: form.find('textarea[name="Text"]').val(),
        CorrectOptionIndex: parseInt(form.find('input[name="CorrectOptionIndex"]:checked').val()),
        Options: []
    };

    // لفي على كل الـ Options وخدي النص والـ Id بتاعها
    form.find('input[name^="Options"][name$=".Id"]').each(function (index) {
        var optionId = $(this).val();
        var optionText = form.find('input[name="Options[' + index + '].OptionText"]').val();

        dto.Options.push({
            Id: optionId,
            OptionText: optionText
        });
    });

    $.ajax({
        url: '/Question/UpdateChoicesQuestion',
        method: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: function (res) {
            toastr.success('Question updated successfully!');
            $('#editQuestionModal-' + questionId).modal('hide');
            setTimeout(() => location.reload(), 1000);
        },
        error: function (err) {
            toastr.error('Something went wrong!');
        }
    });
}
