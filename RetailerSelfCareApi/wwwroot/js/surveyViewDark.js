"use strict";

(function ($) {
    $(function () {

        var rtCode = $("#rtCode").val();
        var surveyId = $("#surveyId").val();
        var isValidSurvey = $("#isValid").val();
        var fileUploadLimit = parseInt($("#fileUploadLimit").val());
        var surveyQuestions = {};
        var answerList = [];
        var isFormValid = false;

        const mapObj = new Map();
        const gridMapObj = new Map();

        if (isValidSurvey == "1") {
            getSurveyByID();
        }
        else if (isValidSurvey == "2") {
            $("#surveyBody").append('<center style="color: red;"><h2>This Survey is now Inactive.</h2></center>');
        }
        else if (isValidSurvey == "3") {
            $("#surveyBody").append('<center style="color: red;"><h2>This Survey is already expired.</h2></center>');
        }
        else if (isValidSurvey == "4") {
            $("#surveyBody").append('<center style="color: green;"><h2>You already responded in this Survey.</h2></center>');
        }
        else {
            $("#surveyBody").append('<center style="color: red;"><h2>Something went wrong!</h2></center>');
        }

        function getSurveyByID() {

            var jsonData = {
                id: surveyId,
                rtCode: rtCode
            };

            $.ajax({
                url: "GetSurveyQuestionsByID",
                data: jsonData,
                dataType: 'json',
                type: "GET",
                success: function (res) {
                    $('#surveyTitle').hide();

                    if (!res.isError) {
                        surveyQuestions = res.data;
                        generateQuestions();
                    }
                    else {
                        $("#surveyBody").append('<center><h2>' + res.message + '</h2></center>');
                    }
                },
                error: function (response) { }
            });
        }

        function generateQuestions() {
            if (surveyQuestions.length > 0) {

                for (var i = 0; i < surveyQuestions.length; i++) {
                    var qnObj = surveyQuestions[i];
                    addUpdateMapObj(qnObj);

                    switch (qnObj.input_type) {
                        case "text":
                            textFieldBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", "");
                            continue;

                        case "paragraph":
                            textAreaBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", "");
                            continue;

                        case "checkbox-single-select":
                            checkboxSingleBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", qnObj.optionsStr);
                            continue;

                        case "checkbox-multiple-select":
                            checkboxMultipleBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", qnObj.optionsStr);
                            continue;

                        case "radio":
                            radioBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", qnObj.optionsStr);
                            continue;

                        case "date-time":
                            dateTimeBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", "");
                            continue;

                        //case "file":
                        //    fileInputBuilder(qnObj);
                        //    updateMapProperty(qnObj.id, "quesOptions", "");
                        //    continue;

                        case "dropdown":
                            dropdownBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", qnObj.optionsStr);
                            continue;

                        case "linear-scale":
                            linearScaleBuilder(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", qnObj.linearScale);
                            continue;

                        case "multiple-choice-grid":
                            multipleChoiceGrid(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", (qnObj.rowStr + "|" + qnObj.columnStr));
                            continue;

                        case "checkbox-grid":
                            multipleCheckboxGrid(qnObj);
                            updateMapProperty(qnObj.id, "quesOptions", (qnObj.rowStr + "|" + qnObj.columnStr));
                            continue;
                    }
                }


                var submitSection = '<div class="card mb-2 text-right" style="background-color: #231F20 !important;">' +
                    '<div class="mt-4 mb-4 lrPad" style="text-align: right;">' +

                    '<button type="button" id="submitSurvey" class="btn btn-primary" style="background: #F36F21;">Submit</button>' +
                    '</div>' +
                    '</div>';

                $("#surveyBody").append(submitSection);

            }
        }

        function addUpdateMapObj(obj) {
            if (!mapObj.has(obj.id)) {
                let objVal = {
                    isRequired: obj.isRequired,
                    quesOrder: obj.quesOrder,
                    ques_descrip: obj.ques_descrip,
                    surveyId: surveyId,
                    questionType: obj.input_type,
                    questionId: obj.id,
                    quesOptions: "",
                    answer: "",
                    file: null,
                    fileMimeType: ""
                };

                mapObj.set(obj.id, objVal);

                if (obj.input_type == "multiple-choice-grid" || obj.input_type == "checkbox-grid") {
                    obj.rows.forEach(function (row, i) {
                        let mapKey = obj.id + row.name;
                        if (!gridMapObj.has(mapKey)) {
                            gridMapObj.set(mapKey, {
                                QuestionId: obj.id,
                                QuestionRow: row.name,
                                QnRowAnswer: ""
                            });
                        }
                    });
                }
            }
        }

        function updateMapProperty(id, proName, value) {
            var exist = mapObj.get(id);
            exist[proName] = value;

            mapObj.set(id, exist);
        }

        function updateGridMapProperty(key, value) {
            var exist = gridMapObj.get(key);
            exist.QnRowAnswer = value;

            gridMapObj.set(key, exist);
        }

        /*=========== Answer Parse Section Start ===========*/


        $('body').on('click', '#submitSurvey', function (event) {
            event.preventDefault();

            answerList = [];

            if (mapObj.size > 0) {
                answerList = [];

                for (let [key, val] of mapObj) {
                    var isValid = true;

                    switch (val.questionType) {

                        case "text":
                            isValid = parseTextInput(val);
                            answerList.push(val);
                            break;

                        case "paragraph":
                            isValid = parseTextAreaInput(val);
                            answerList.push(val);
                            break;

                        case "checkbox-single-select":
                            isValid = parseCheckboxSingleSelect(val);
                            answerList.push(val);
                            break;

                        case "checkbox-multiple-select":
                            isValid = parseCheckboxMultipleSelect(val);
                            answerList.push(val);
                            break;

                        case "radio":
                            isValid = parseRadioSelect(val);
                            answerList.push(val);
                            break;

                        case "date-time":
                            isValid = parseDateTime(val);
                            answerList.push(val);
                            break;

                        case "file":
                            isValid = parseFileInput(val);
                            answerList.push(val);
                            break;

                        case "dropdown":
                            isValid = parseDropDown(val);
                            answerList.push(val);
                            break;

                        case "linear-scale":
                            isValid = parseLinearSelect(val);
                            answerList.push(val);
                            break;

                        case "multiple-choice-grid":
                            isValid = parseMultipleChoiceGrid(val);
                            var gridAnswers = getQnGridAnswers(key);
                            val.qnGridAnswers = gridAnswers;
                            answerList.push(val);
                            break;

                        case "checkbox-grid":
                            isValid = parseMultipleCheckboxGrid(val);
                            var gridAnswers = getQnGridAnswers(key);
                            val.qnGridAnswers = gridAnswers;
                            answerList.push(val);
                            break;
                    }

                    if (!isValid) break;
                }

            }


            //Submit Answer
            if (isFormValid) {
                var responseData = {
                    retailerCode: rtCode,
                    surveyAnswers: JSON.stringify(answerList)
                };

                $.ajax({
                    url: "SubmitSurveyResponse",
                    type: "POST",
                    data: responseData,
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    dataType: 'json',
                    success: function (res) {
                        if (!res.isError) {
                            var infoModal = $("#successModalDark");
                            infoModal.modal('show');
                        }
                        else {
                            toastr.error(res.message, "ERROR!!");
                            return;
                        }
                    },
                    error: function (response) {
                        console.log(response);
                    }
                });
            }
        });


        function getQnGridAnswers(qnId) {
            let gridAnswers = [];
            for (let [k, v] of gridMapObj) {
                if (k.startsWith(qnId)) {
                    gridAnswers.push(v);
                }
            }

            return gridAnswers;
        }


        $('body').on('click', "#btnOk", function () {
            $("#rtCode").val("");
            $("#surveyId").val("");
            $("#isValid").val("0");
            $("#fileUploadLimit").val("0");
            surveyQuestions = {};
            answerList = [];
            isFormValid = false;
            mapObj.clear();
            gridMapObj.clear();

            $("#successModalDark").modal('hide');

            window.location.reload(true);
        });


        function isValidInput(userInput) {
            if (typeof (userInput) != 'undefined') {
                return userInput;
            }
            else {
                return '';
            }
        }

        function isValidInputWithComma(userInput) {
            if (typeof (userInput) != 'undefined') {
                return userInput + ', ';
            }
            else {
                return '';
            }
        }

        function parseTextInput(obj) {
            var isValid = true;

            var text = $('#' + obj.questionId + '> div:eq(1)  > input').val();

            if (obj.isRequired) {
                if (text) {
                    var trimmed = text.trim();

                    if (trimmed) {

                        $('#' + obj.questionId).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');
                        updateMapProperty(obj.questionId, "answer", trimmed);
                        isFormValid = true;
                    }
                    else {
                        $('#' + obj.questionId).addClass('notValid');
                        $('#' + obj.questionId + '_error').removeClass('hideError');
                        isValid = false;
                        isFormValid = false;
                        toastr.warning("Please insert Question " + obj.quesOrder + ".", "WARNING!!");
                    }
                } else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please insert Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", text);
                isFormValid = true;
            }

            return isValid;
        }

        function parseTextAreaInput(obj) {
            var isValid = true;

            var text = $('#' + obj.questionId + '> div:eq(1)  > textarea').val();

            if (obj.isRequired) {
                if (text) {
                    var trimmed = text.trim();

                    if (trimmed) {
                        $('#' + obj.questionId).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');
                        updateMapProperty(obj.questionId, "answer", trimmed);
                        isFormValid = true;
                    }
                    else {
                        $('#' + obj.questionId).addClass('notValid');
                        $('#' + obj.questionId + '_error').removeClass('hideError');
                        isValid = false;
                        isFormValid = false;
                        toastr.warning("Please insert Question " + obj.quesOrder + ".", "WARNING!!");
                    }
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please insert Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", text);
                isFormValid = true;
            }

            return isValid;
        }

        function parseCheckboxSingleSelect(obj) {
            var isValid = true;

            var text = '';

            $('#' + obj.questionId + ' > div > input').each(function () {

                if ($(this).is(':checked')) {
                    text = $(this).val();
                }
            });

            if (obj.isRequired) {

                if (text) {

                    $('#' + obj.questionId).removeClass('notValid');
                    $('#' + obj.questionId + '_error').addClass('hideError');
                    updateMapProperty(obj.questionId, "answer", text);

                    isFormValid = true;
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", text);

                isFormValid = true;
            }

            return isValid;
        }

        function parseCheckboxMultipleSelect(obj) {
            var isValid = true;

            var text = '';

            $('#' + obj.questionId + ' > div > input').each(function () {

                if ($(this).is(':checked')) {
                    text += $(this).val() + ', ';
                }
            });

            text = text.slice(0, text.length - 2);

            if (obj.isRequired) {

                if (text) {

                    $('#' + obj.questionId).removeClass('notValid');
                    $('#' + obj.questionId + '_error').addClass('hideError');

                    updateMapProperty(obj.questionId, "answer", text);

                    isFormValid = true;
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", text);

                isFormValid = true;
            }

            return isValid;
        }

        function parseRadioSelect(obj) {
            var isValid = true;
            var text = '';

            $('#' + obj.questionId + '> div > input').each(function () {
                text = $('input[name="' + obj.questionId + '"]:checked').val();

            });

            if (obj.isRequired) {
                if (text) {

                    $('#' + obj.questionId).removeClass('notValid');
                    $('#' + obj.questionId + '_error').addClass('hideError');

                    updateMapProperty(obj.questionId, "answer", text);

                    isFormValid = true;
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", isValidInput(text));

                isFormValid = true;
            }

            return isValid;
        }

        function parseDateTime(obj) {
            var isValid = true;

            var text = '';

            var dateId = '#' + obj.questionId + 'date';
            var timeId = '#' + obj.questionId + 'time';

            text = $(dateId).datepicker({ dateFormat: 'dd,MM,yyyy' }).val();

            text += ' ' + $(timeId).val();

            if (obj.isRequired) {
                var trimmed = text.trim();

                if (trimmed) {

                    $('#' + obj.questionId).removeClass('notValid');
                    $('#' + obj.questionId + '_error').addClass('hideError');

                    updateMapProperty(obj.questionId, "answer", trimmed);

                    isFormValid = true;
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please select Date-Time in Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", text);

                isFormValid = true;
            }

            return isValid;
        }

        function parseFileInput(obj) {
            var isValid = true;

            var _file = $('#' + obj.questionId + 'file')[0].files[0];

            if (_file && _file.size != 0 && _file.error != 0) {

                var _fileStr = $("#" + obj.questionId + "fileStr").val();
                let _fileMimeType = _fileStr.match(/[^:]\w+\/[\w-+\d.]+(?=;|,)/)[0];

                updateMapProperty(obj.questionId, "file", _fileStr);
                updateMapProperty(obj.questionId, "fileMimeType", _fileMimeType);

                isFormValid = true;
            }

            if (obj.isRequired && !_file) {

                $('#' + fileDivid).addClass('notValid');
                $('#' + fileDivid + '_error').removeClass('hideError');
                isValid = false;
                isFormValid = false;
                toastr.warning("Please upload file in Question " + obj.quesOrder + ".", "WARNING!!");
            }

            return isValid;
        }

        function parseDropDown(obj) {
            var isValid = true;

            var text = "";

            var fieldId = "ques" + obj.questionId.toString();
            text = $("#" + fieldId).val();

            if (obj.isRequired) {
                if (text) {

                    $('#' + obj.questionId).removeClass('notValid');
                    $('#' + obj.questionId + '_error').addClass('hideError');

                    updateMapProperty(obj.questionId, "answer", text);
                    isFormValid = true;
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please select option from Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", text);

                isFormValid = true;
            }

            return isValid;
        }

        function parseLinearSelect(obj) {
            var isValid = true;

            var text = '';

            $('#' + obj.questionId + '> div > table > tbody > tr').each(function () {
                text = $('input[name="' + obj.questionId + '"]:checked').val();

            });


            if (obj.isRequired) {
                if (text) {
                    var trimmed = text.trim();

                    if (trimmed) {

                        $('#' + obj.questionId).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');

                        updateMapProperty(obj.questionId, "answer", trimmed);
                        isFormValid = true;
                    }
                    else {
                        $('#' + obj.questionId).addClass('notValid');
                        $('#' + obj.questionId + '_error').removeClass('hideError');
                        isValid = false;
                        isFormValid = false;
                        toastr.warning("Please select linear scale from Question " + obj.quesOrder + ".", "WARNING!!");
                    }
                }
                else {
                    $('#' + obj.questionId).addClass('notValid');
                    $('#' + obj.questionId + '_error').removeClass('hideError');
                    isValid = false;
                    isFormValid = false;
                    toastr.warning("Please select linear scale from Question " + obj.quesOrder + ".", "WARNING!!");
                }
            }
            else {
                updateMapProperty(obj.questionId, "answer", isValidInput(text));
                isFormValid = true;
            }

            return isValid;
        }


        function parseMultipleChoiceGrid(obj) {
            var isValid = true;

            var text = '';
            var validateList = [];
            var selectedRowCount = 0;

            $('#' + obj.questionId + '> div > table > tbody > tr.optionRow').each(function () {

                var selectedList = [];

                var rowName = $(this).attr("data-row-name");
                selectedList.push(rowName);

                var selectedOption = $('input[name="' + rowName + '"]:checked').val();

                if (typeof (selectedOption) !== 'undefined') {
                    selectedRowCount += 1;
                }
                selectedList.push(selectedOption);

                validateList.push(selectedList);
                let inputVal = isValidInput(selectedOption);

                text += rowName + ': ' + inputVal + ', ';
                updateGridMapProperty(obj.questionId + rowName, inputVal);
            });

            text = text.slice(0, text.length - 2);

            if (obj.isRequired) {

                if (validateList.length != selectedRowCount) {

                    for (var i = 0; i < validateList.length; i++) {

                        var optionRow = validateList[i][0].split(' ')[0];

                        if (typeof (validateList[i][1]) === 'undefined') {
                            $('#' + obj.questionId + optionRow).addClass('notValid');
                            $('#' + obj.questionId + '_error').removeClass('hideError');
                            isValid = false;
                            isFormValid = false;
                        }
                        else {
                            $('#' + obj.questionId + optionRow).removeClass('notValid');
                            $('#' + obj.questionId + '_error').addClass('hideError');
                        }
                    }

                    if (!isValid) {
                        toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                    }
                }
                else {

                    for (var i = 0; i < validateList.length; i++) {

                        var optionRow = validateList[i][0].split(' ')[0];

                        $('#' + obj.questionId + optionRow).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');
                    }

                    updateMapProperty(obj.questionId, "answer", text);
                    isFormValid = true;
                }

            }
            else {

                if (selectedRowCount == 0) {

                    for (var i = 0; i < validateList.length; i++) {

                        var optionRow = validateList[i][0].split(' ')[0];

                        $('#' + obj.questionId + optionRow).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');

                    }

                    updateMapProperty(obj.questionId, "answer", text);
                    isFormValid = true;
                }
                else {

                    if (validateList.length !== selectedRowCount) {

                        for (var i = 0; i < validateList.length; i++) {

                            if (typeof (validateList[i][1]) === 'undefined') {
                                var optionRow = validateList[i][0].split(' ')[0];

                                $('#' + obj.questionId + optionRow).addClass('notValid');
                                $('#' + obj.questionId + '_error').removeClass('hideError');
                                isValid = false;
                                isFormValid = false;
                            }
                        }

                        if (!isValid) {
                            toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                        }
                    }
                    else {

                        for (var i = 0; i < validateList.length; i++) {
                            var optionRow = validateList[i][0].split(' ')[0];

                            $('#' + obj.questionId + optionRow).removeClass('notValid');
                            $('#' + obj.questionId + '_error').addClass('hideError');
                        }

                        updateMapProperty(obj.questionId, "answer", text);
                        isFormValid = true;
                    }
                }
            }

            return isValid;
        }


        function parseMultipleCheckboxGrid(obj) {
            var isValid = true;

            var text = '';
            var validateGridList = [];
            var selectedRowCount = 0;

            $('#' + obj.questionId + '> div > table > tbody > tr.optionRow').each(function (i1, v1) {

                var selectedList = [];

                var selectedOptions = '';

                var rowName = $(this).attr("data-row-name");
                selectedList.push(rowName);

                var rowNo = "Row" + (i1 + 1);
                var rowId = obj.questionId + rowNo;
                $(`#${rowId}> td`).each(function (i2, v2) {

                    var _id = $(this).attr('data-input-name');
                    if (_id) {
                        var checkBoxName = _id + rowNo + "Column" + i2;
                        var checkboxVal = $('input[name="' + checkBoxName + '"]:checked').val();

                        selectedOptions += isValidInputWithComma(checkboxVal);
                    }
                });

                selectedOptions = selectedOptions.slice(0, selectedOptions.length - 2);

                if (selectedOptions.length !== 0) {
                    selectedRowCount += 1;
                }

                selectedList.push(selectedOptions);
                validateGridList.push(selectedList);
                let inputVal = isValidInput(selectedOptions);

                text += rowName + ': ' + inputVal + ', ';
                updateGridMapProperty(obj.questionId + rowName, inputVal);
            });

            if (obj.isRequired) {

                if (validateGridList.length != selectedRowCount) {

                    for (var i = 0; i < validateGridList.length; i++) {

                        var optionRow = validateGridList[i][0].split(' ')[0];

                        if (validateGridList[i][1].length === 0) {
                            $('#' + obj.questionId + optionRow).addClass('notValid');
                            $('#' + obj.questionId + '_error').removeClass('hideError');
                            isValid = false;
                            isFormValid = false;
                        }
                        else {
                            $('#' + obj.questionId + optionRow).removeClass('notValid');
                            $('#' + obj.questionId + '_error').addClass('hideError');
                        }
                    }

                    if (!isValid) {
                        toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                    }
                }
                else {

                    for (var i = 0; i < validateGridList.length; i++) {

                        var optionRow = validateGridList[i][0].split(' ')[0];

                        $('#' + obj.questionId + optionRow).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');
                    }

                    updateMapProperty(obj.questionId, "answer", text);
                    isFormValid = true;
                }
            }
            else {

                if (selectedRowCount === 0) {

                    for (var i = 0; i < validateGridList.length; i++) {
                        var optionRow = validateGridList[i][0].split(' ')[0];

                        $('#' + obj.questionId + optionRow).removeClass('notValid');
                        $('#' + obj.questionId + '_error').addClass('hideError');
                    }

                    updateMapProperty(obj.questionId, "answer", text);

                    isFormValid = true;
                }
                else {

                    if (validateGridList.length !== selectedRowCount) {

                        for (var i = 0; i < validateGridList.length; i++) {

                            if (validateGridList[i][1].length === 0) {
                                var optionRow = validateGridList[i][0].split(' ')[0];

                                $('#' + obj.questionId + optionRow).addClass('notValid');
                                $('#' + obj.questionId + '_error').removeClass('hideError');
                                isValid = false;
                                isFormValid = false;
                            }
                        }

                        if (!isValid) {
                            toastr.warning("Please select from Question " + obj.quesOrder + ".", "WARNING!!");
                        }
                    }
                    else {

                        for (var i = 0; i < validateGridList.length; i++) {
                            var optionRow = validateGridList[i][0].split(' ')[0];

                            $('#' + obj.questionId + optionRow).removeClass('notValid');
                            $('#' + obj.questionId + '_error').addClass('hideError');
                        }

                        updateMapProperty(obj.questionId, "answer", text);
                        isFormValid = true;
                    }
                }

            }

            return isValid;
        }

        /*=========== Answer Parse Section End ===========*/


        /*=========== Question Generate Section Start ===========*/


        function textFieldBuilder(question) {
            var literal = '<div class="card mb-2 question" id="' + question.id + '" data-question-id="' + question.id
                + '" data-question-type="' + question.input_type + '" data-is-required="' + question.isRequired
                + '" style="background-color: #231F20 !important;"><div class="d-flex align-content-start p-2">'
                + '<label class="form-label" id="' + question.id + '">' + question.quesOrder + '.' + question.ques_descrip
                + '</label></div><div class="mb-2 lrPad"><input type="text" class="form-control customInput"></div>'
                + '<div id="' + question.id + '_error" class="err hideError">'
                + '<svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">Please response to this question</span>'
                + '</div>'
                + '</div>';

            $("#surveyForm").append(literal);

            //if (question.isRequired) {
            //    $('#' + question.id + "lbl").addClass('astericreq');
            //}
        }

        function textAreaBuilder(question) {
            var literal = '<div class="card mb-2 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '" data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;">'
                + '<div class="d-flex align-content-start p-2">'
                + '<label class="form-label">' + question.quesOrder + '.' + question.ques_descrip + '</label>'
                + '</div>'
                + '<div class="mb-2 lrPad">'
                + '<textarea class="form-control customInput" ></textarea>'
                + '</div>'
                + '<div id="' + question.id + '_error" class="err hideError">'
                + '<svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>'
                + '<span style="padding-left: 3px;">Please response to this question</span></div>'
                + '</div>';

            $("#surveyForm").append(literal);
        }

        function checkboxSingleBuilder(question) {

            var checkOptions = '';

            for (var i = 0; i < question.options.length; i++) {
                checkOptions += '<div class="d-flex align-content-start mb-1 lrPad">' +
                    '<input class="form-check-input" type="checkbox" value="' + question.options[i].name + '" id="' + question.id + '0' + question.options[i].id + '" data-id="' + question.id + '0' + question.options[i].id + '" onclick="singleSelect(this,' + question.id + ')" name="' + question.id + '"><label class="form-check-label lrPad" for="' + question.id + '0' + question.options[i].id + '">' + question.options[i].name + '</label></div>';
            }

            var literal =
                '<div class="card mb-3 question singleCheckbox" id="' + question.id + '" data-question-id="' + question.id +
                '" data-question-type="' + question.input_type + '"data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;"><div class="d-flex align-content-start p-2"><label class="form-label" data-is-required="' + question.isRequired + '">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label ></div>' + checkOptions +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">Please response to this question</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);
        }

        function checkboxMultipleBuilder(question) {
            var checkOptions = '';

            for (var i = 0; i < question.options.length; i++) {
                checkOptions += '<div class="d-flex align-content-start mb-1 lrPad">' +
                    '<input class="form-check-input" type="checkbox" value="' + question.options[i].name + '" id="' + question.id + '0' + question.options[i].id + '" data-id="' + question.id + '0' + question.options[i].id + '" name="' + question.id + '0' + question.options[i].id + '" onclick="multiSelect(' + question.id + '0' + question.options[i].id + ')"><label class="form-check-label lrPad" for="' + question.id + '0' + question.options[i].id + '">' + question.options[i].name + '</label></div>';
            }

            var literal =
                '<div class="card mb-3 question" id="' + question.id + '" data-question-id="' + question.id +
                '" data-question-type="' + question.input_type + '"data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;"><div class="d-flex align-content-start p-2"><label class="form-label">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label ></div>' + checkOptions +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">Please response to this question</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);
        }

        function radioBuilder(question) {

            var radioOptions = '';

            for (var i = 0; i < question.options.length; i++) {
                radioOptions += '<div class="form-check d-flex align-content-start mb-1 lrPad checkboxPad">' +
                    '<input class="form-check-input" type="radio" name="' + question.id + '" id="' + question.id + '0' + question.options[i].id + '" value="' + question.options[i].name + '">'
                    + '<label class="form-check-label lrPad" for="' + question.id + '0' + question.options[i].id + '">'
                    + question.options[i].name +
                    '</label></div >';
            }


            var literal =
                '<div class="card mb-3 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '"data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;"><div class="d-flex align-content-start p-2"><label class="form-label">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label>' + '</div>' + radioOptions +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">Please response to this question</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);

        }

        function dateTimeBuilder(question) {

            var literal = '<div class="card mb-3 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '" data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;">'
                + '<div class="d-flex align-content-start p-2">'
                + '<label class="form-label">' + question.quesOrder + '.' + question.ques_descrip + '</label>' + '</div>'
                + '<div class="d-flex">'
                + '<div class="form-floating mb-3 col-md-3 lrPad flex">'
                + '<input type="text" class="form-control" id="' + question.id + 'date"  value="" class="calendar" readonly/>'
                + '<label for="' + question.id + 'date" style="padding-left: 2.2rem; color: #000000;">Date</label>'
                + '</div>'

                + '<div class="form-floating mb-3 col-md-3 lrPad">'
                + '<input type="text" class="form-control" id="' + question.id + 'time">'
                + '<label for="' + question.id + 'time" style="padding-left: 2.2rem; color: #000000;">Time</label>'
                + '</div>'
                + '</div>'
                + '<div id="' + question.id + '_error" class="err hideError">'
                + '<svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>'
                + '<span style="padding-left: 3px;">Please response to this question</span>'
                + '</div>'
                + '</div>';

            $("#surveyForm").append(literal);

            let datePickerId = question.id + "date";
            $('#' + datePickerId).datepicker({
                dateFormat: 'dd-M-yy',
                setDate: new Date()
            });

            $('#' + question.id + 'time').timepicker({
                timeFormat: 'g:i a',
                interval: 5,
                minTime: '00',
                maxTime: '11:59 pm',
                defaultTime: '00',
                startTime: '00:00',
                dynamic: false,
                dropdown: true,
                scrollbar: true
            });

        }

        function fileInputBuilder(question) {
            var literal = '<div class="card mb-2 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '" data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;">' +
                '<div class="d-flex align-content-start p-2">' +
                '<label class="form-label">' + question.quesOrder + '.' + question.ques_descrip + '</label>' +
                '</div>' +

                '<textarea style="display:none;" id="' + question.id + 'fileStr"></textarea>' +

                '<div class="lrPad">' +
                '<div class="mb-3">' +
                '<input class="form-control" type="file" class="' + question.id + '" id="' + question.id + 'file" onchange="isFileWithinLimit(' + question.id + ')">' + '</div>'

                + '</div>'
                + '<div id="' + question.id + '_error" class="err hideError">' +
                '<svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;" id="' + question.id + 'span">Please response to this question</span>' +
                '</div></div>';


            if (fileUploadLimit > 0) {

                $("#surveyForm").append(literal);
            }

        }

        function dropdownBuilder(question) {

            var dropOptions = '';

            for (var i = 0; i < question.options.length; i++) {
                dropOptions += '<option class="' + question.id + '" id="' + question.id + question.options[i].name + '" value="' + question.options[i].name + '" name="' + question.options[i].name + '">' + question.options[i].name + '</option>';
            }

            var literal =
                '<div class="card mb-3 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '" data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;"><div class="d-flex align-content-start p-2"><label class="form-label">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label>' + '</div>' +
                '<div class="' + question.id + 'selectedVal"></div>'
                +
                '<div class="input-group mb-3 lrPad"><select class="form-select" id="ques' + question.id + '" onchange="optionSelected(this,' + question.id + ')"><option value="">Plese select one option from below</option>' + dropOptions + '</select></div>' +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">Please response to this question</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);

        }

        function linearScaleBuilder(question) {
            var linearNumber = '';
            var linearCheckbox = '';

            for (var i = question.linear_start_value; i <= question.linear_end_value; i++) {
                linearNumber += '<td style="border: none;min-width:2rem;text-align:center;overflow-wrap: break-word; color: #FFFFFF !important;">' + i + '</td>';
                linearCheckbox += '<td style="border: none;min-width:2rem;text-align:center;"><input class="form-check-input" type="radio" id="' + question.id + '_' + i + '" value="' + i + '" name="' + question.id + '"></td>';
            }


            var literal = '<div class="card mb-2 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '"data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;">' +
                '<div class="d-flex align-content-start p-2">' +
                '<label class="form-label">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label>' +
                '</div>' +

                '<div class="lrPad" style="overflow-x: scroll !important;scrollbar-width: thin;overflow: visible;">' +

                '<table class="table">' +
                '<tbody>' +
                '<tr>' +
                '<td style="border: none;"></td>' +
                linearNumber +
                '<td style="border: none;"></td>' +
                '</tr>' +
                '<tr>' +
                '<td style="border: none;min-width:2rem;text-align:right;overflow-wrap: break-word; color: #FFFFFF !important;">' + question.linear_start_level_txt + '</td>' +
                linearCheckbox +
                '<td style="border: none;min-width:2rem;text-align:left;overflow-wrap: break-word; color: #FFFFFF !important;">' + question.linear_end_level_txt + '</td>' +
                '</tr></tbody></table></div>' +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">Please response to this question</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);

        }

        function multipleChoiceGrid(question) {
            var choiceOption = '';
            var choiceGrid = '';

            for (var i = 0; i < question.columns.length; i++) {
                choiceOption += '<td style="border: none;min-width: 2rem;text-align: center;overflow-wrap: break-word !important; color: #FFFFFF !important;" class="' + question.columns[i].id + '">' + question.columns[i].name + '</td>';
            }

            for (var i = 0; i < question.rows.length; i++) {

                choiceGrid += '<tr data-row-name="' + question.rows[i].name + '" id="' + question.id + "Row" + (i + 1) + '" class="optionRow"><td style="border: none;min-width: 2rem;text-align: left;overflow-wrap: break-word; color: #FFFFFF !important;">' + question.rows[i].name + '</td>';
                for (var j = 0; j < question.columns.length; j++) {
                    choiceGrid += '<td style="border: none;min-width: 2rem;text-align: center;overflow-wrap: break-word;" class="' + question.columns[j].id + '"><input class="form-check-input ' + question.columns[j].id + '" type="radio" value="' + question.columns[j].name + '" name="' + question.rows[i].name + '">' + '</td>';
                }

                choiceGrid += '</tr>';
            }

            var literal = '<div class="card mb-2 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '"data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;">' +
                '<div class="d-flex align-content-start p-2">' +
                '<label class="form-label">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label>' +
                '</div>' +

                '<div class="lrPad" style="overflow-x: scroll !important;scrollbar-width: thin;overflow: visible;">' +

                '<table class="table">' +
                '<tbody>' +
                '<tr>' +
                '<td style="border: none;"></td>' +

                choiceOption +
                '</tr>' +
                choiceGrid +
                '</tbody></table></div>' +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">This question requires one response per row</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);
        }

        function multipleCheckboxGrid(question) {
            var checkboxOption = '';
            var checkboxGrid = '';

            for (var i = 0; i < question.columns.length; i++) {
                checkboxOption += '<td style="border: none; min-width: 2rem;text-align: center;overflow-wrap: break-word; color: #FFFFFF !important;" class="' + question.columns[i].id + '">' + question.columns[i].name + '</td>';
            }

            for (var i = 0; i < question.rows.length; i++) {

                var rowNo = "Row" + (i + 1);
                checkboxGrid += '<tr data-row-name="' + question.rows[i].name + '" id="' + question.id + rowNo + '" class="optionRow"><td style="border: none;min-width: 3rem;text-align: left;overflow-wrap: break-word; color: #FFFFFF !important;">' + question.rows[i].name + '</td>';

                for (var j = 0; j < question.columns.length; j++) {
                    checkboxGrid += '<td style="border: none;min-width: 2rem;text-align: center;overflow-wrap: break-word;" data-input-name="' + question.columns[j].id + '"><input id="' + question.columns[j].id + '" class="form-check-input" type="checkbox" value="' + question.columns[j].name + '" name="' + question.columns[j].id + rowNo + "Column" + (j + 1) + '">' + '</td>';
                }

                checkboxGrid += '</tr>';
            }

            var literal = '<div class="card mb-2 question" id="' + question.id + '" data-question-id="' + question.id + '" data-question-type="' + question.input_type + '"data-is-required="' + question.isRequired + '" style="background-color: #231F20 !important;">' +
                '<div class="d-flex align-content-start p-2">' +
                '<label class="form-label">' +
                question.quesOrder + '. ' + question.ques_descrip + '</label>' +
                '</div>' +

                '<div class="lrPad" style="overflow-x: scroll !important;scrollbar-width: thin;overflow: visible;">' +

                '<table class="table">' +
                '<tbody>' +
                '<tr class=col-1>' +
                '<td style="border: none;"></td>' +

                checkboxOption +
                '</tr>' +
                checkboxGrid +
                '</tbody></table></div>' +
                '<div id="' + question.id + '_error" class="err hideError" style="padding-top: 3px;"><svg xmlns="http://www.w3.org/2000/svg" class="errCaution" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>' +
                '<span style="padding-left: 3px;">This question requires one response per row</span>' +
                '</div></div>';

            $("#surveyForm").append(literal);
        }

        /*=========== Question Generate Section Start ===========*/

    });

    // Run right away
})(jQuery);